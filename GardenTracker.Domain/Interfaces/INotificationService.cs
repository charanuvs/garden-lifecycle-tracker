namespace GardenTracker.Domain.Interfaces;

public interface INotificationService
{
    Task SendReminderAsync(int userCropId, int activeWorkflowStepId, string stepName, DateTime scheduledStart, DateTime scheduledEnd);
    Task SendRecurringStepCreatedAsync(int userCropId, string stepName, int recurrenceNumber);
}
