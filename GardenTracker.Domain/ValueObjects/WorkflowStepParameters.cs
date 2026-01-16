namespace GardenTracker.Domain.ValueObjects;

public class WorkflowStepParameters
{
    public int? DurationDays { get; set; }
    public int? FrequencyDays { get; set; }
    public int? Quantity { get; set; }
    public string? Notes { get; set; }
    public Dictionary<string, string> CustomParameters { get; set; } = new();

    public WorkflowStepParameters() { }

    public WorkflowStepParameters(
        int? durationDays = null,
        int? frequencyDays = null,
        int? quantity = null,
        string? notes = null,
        Dictionary<string, string>? customParameters = null)
    {
        DurationDays = durationDays;
        FrequencyDays = frequencyDays;
        Quantity = quantity;
        Notes = notes;
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
            overrides.CustomParameters.Count > 0 ? overrides.CustomParameters : CustomParameters
        );
    }
}
