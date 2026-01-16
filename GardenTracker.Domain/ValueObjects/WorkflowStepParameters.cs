namespace GardenTracker.Domain.ValueObjects;

public class WorkflowStepParameters
{
    public int? DurationDays { get; set; }
    public int? FrequencyDays { get; set; }
    public int? Quantity { get; set; }
    public string? Notes { get; set; }
    
    /// <summary>
    /// Indicates this step should recur (e.g., watering every N days)
    /// </summary>
    public bool IsRecurring { get; set; }
    
    /// <summary>
    /// Interval in days for recurring steps
    /// </summary>
    public int? RecurrenceIntervalDays { get; set; }
    
    /// <summary>
    /// Maximum number of recurrences (null = infinite)
    /// </summary>
    public int? MaxRecurrences { get; set; }
    
    /// <summary>
    /// Days before scheduled start to begin sending reminders
    /// </summary>
    public int ReminderLeadDays { get; set; } = 1;
    
    public Dictionary<string, string> CustomParameters { get; set; } = new();

    public WorkflowStepParameters() { }

    public WorkflowStepParameters(
        int? durationDays = null,
        int? frequencyDays = null,
        int? quantity = null,
        string? notes = null,
        bool isRecurring = false,
        int? recurrenceIntervalDays = null,
        int? maxRecurrences = null,
        int reminderLeadDays = 1,
        Dictionary<string, string>? customParameters = null)
    {
        DurationDays = durationDays;
        FrequencyDays = frequencyDays;
        Quantity = quantity;
        Notes = notes;
        IsRecurring = isRecurring;
        RecurrenceIntervalDays = recurrenceIntervalDays;
        MaxRecurrences = maxRecurrences;
        ReminderLeadDays = reminderLeadDays;
        CustomParameters = customParameters ?? new();
    }

    public WorkflowStepParameters Merge(WorkflowStepParameters? overrides)
    {
        if (overrides == null) return this;

        return new WorkflowStepParameters(
            overrides.DurationDays ?? DurationDays,
            overrides.FrequencyDays ?? FrequencyDays,
            overrides.Quantity ?? Quantity,
            overrides.Notes ?? Notes,
            overrides.IsRecurring || IsRecurring,
            overrides.RecurrenceIntervalDays ?? RecurrenceIntervalDays,
            overrides.MaxRecurrences ?? MaxRecurrences,
            overrides.ReminderLeadDays != 1 ? overrides.ReminderLeadDays : ReminderLeadDays,
            overrides.CustomParameters.Count > 0 ? overrides.CustomParameters : CustomParameters
        );
    }
}
