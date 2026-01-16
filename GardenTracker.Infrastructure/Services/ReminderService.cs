using GardenTracker.Domain.Entities;
using GardenTracker.Domain.Enums;
using GardenTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GardenTracker.Infrastructure.Services;

public class ReminderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<ReminderService> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Check all active workflow steps and send reminders for those due
    /// Should be called daily
    /// </summary>
    public async Task ProcessDailyRemindersAsync()
    {
        var today = DateTime.UtcNow.Date;
        
        // Get all active steps that need reminders
        var stepsNeedingReminders = await _unitOfWork.ActiveWorkflowSteps
            .GetAllAsync();
        
        var stepsToNotify = stepsNeedingReminders
            .Where(step => ShouldSendReminder(step, today))
            .ToList();

        _logger.LogInformation("Processing {Count} reminders for today {Date}", 
            stepsToNotify.Count, today.ToString("yyyy-MM-dd"));

        foreach (var step in stepsToNotify)
        {
            try
            {
                await SendReminderForStepAsync(step);
                
                // Update last reminder sent date
                step.LastReminderSentDate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                
                _logger.LogInformation("Sent reminder for step {StepId} - {StepName}", 
                    step.Id, step.WorkflowStepDefinition.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder for step {StepId}", step.Id);
            }
        }
    }

    private bool ShouldSendReminder(ActiveWorkflowStep step, DateTime today)
    {
        // Only send reminders for not-started steps
        if (step.CurrentState != WorkflowStepState.NotStarted)
            return false;

        // Check if reminder is active
        if (!step.IsReminderActive)
            return false;

        // Must have a scheduled start date
        if (!step.ScheduledStartDate.HasValue)
            return false;

        // Calculate when to start reminders (lead days before scheduled start)
        var reminderLeadDays = step.ResolvedParameters.ReminderLeadDays;
        var reminderStartDate = step.ScheduledStartDate.Value.AddDays(-reminderLeadDays).Date;

        // Don't send if we haven't reached the reminder start date yet
        if (today < reminderStartDate)
            return false;

        // Don't send if we're past the scheduled end date
        if (step.ScheduledEndDate.HasValue && today > step.ScheduledEndDate.Value.Date)
            return false;

        // Check if we already sent a reminder today
        if (step.LastReminderSentDate.HasValue && 
            step.LastReminderSentDate.Value.Date == today)
            return false;

        return true;
    }

    private async Task SendReminderForStepAsync(ActiveWorkflowStep step)
    {
        if (!step.ScheduledStartDate.HasValue || !step.ScheduledEndDate.HasValue)
            return;

        await _notificationService.SendReminderAsync(
            step.UserCropId,
            step.Id,
            step.WorkflowStepDefinition.Name,
            step.ScheduledStartDate.Value,
            step.ScheduledEndDate.Value
        );
    }
}
