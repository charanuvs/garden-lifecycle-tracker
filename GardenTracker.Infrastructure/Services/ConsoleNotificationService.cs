using GardenTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GardenTracker.Infrastructure.Services;

/// <summary>
/// Simple console-based notification service for demonstration
/// In production, replace with email, SMS, or push notification service
/// </summary>
public class ConsoleNotificationService : INotificationService
{
    private readonly ILogger<ConsoleNotificationService> _logger;

    public ConsoleNotificationService(ILogger<ConsoleNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendReminderAsync(int userCropId, int activeWorkflowStepId, string stepName, DateTime scheduledStart, DateTime scheduledEnd)
    {
        var message = $"🌱 REMINDER: '{stepName}' for Crop #{userCropId} is scheduled from {scheduledStart:MMM dd} to {scheduledEnd:MMM dd}";
        
        _logger.LogInformation("NOTIFICATION: {Message}", message);
        Console.WriteLine($"\n{DateTime.Now:HH:mm:ss} - {message}\n");
        
        return Task.CompletedTask;
    }

    public Task SendRecurringStepCreatedAsync(int userCropId, string stepName, int recurrenceNumber)
    {
        var message = $"🔄 New recurring task: '{stepName}' (#{recurrenceNumber}) created for Crop #{userCropId}";
        
        _logger.LogInformation("NOTIFICATION: {Message}", message);
        Console.WriteLine($"\n{DateTime.Now:HH:mm:ss} - {message}\n");
        
        return Task.CompletedTask;
    }
}
