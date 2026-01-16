using GardenTracker.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GardenTracker.Infrastructure.BackgroundServices;

public class DailyReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyReminderBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Check every hour

    public DailyReminderBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<DailyReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Reminder Background Service started");

        // Track the last date we processed reminders
        DateTime? lastProcessedDate = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var today = DateTime.UtcNow.Date;

                // Only process once per day
                if (lastProcessedDate == null || lastProcessedDate.Value < today)
                {
                    _logger.LogInformation("Processing daily reminders for {Date}", today);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reminderService = scope.ServiceProvider.GetRequiredService<ReminderService>();
                        await reminderService.ProcessDailyRemindersAsync();
                    }

                    lastProcessedDate = today;
                    _logger.LogInformation("Daily reminders processed successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing daily reminders");
            }

            // Wait before next check
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Daily Reminder Background Service stopped");
    }
}
