using GardenTracker.Domain.Common;
using GardenTracker.Domain.Enums;
using GardenTracker.Domain.ValueObjects;

namespace GardenTracker.Domain.Entities;

/// <summary>
/// A running instance of a workflow step for a user's crop
/// </summary>
public class ActiveWorkflowStep : BaseEntity
{
    public int UserCropId { get; set; }
    public UserCrop UserCrop { get; set; } = null!;
    
    public int WorkflowStepDefinitionId { get; set; }
    public WorkflowStepDefinition WorkflowStepDefinition { get; set; } = null!;
    
    public WorkflowStepState CurrentState { get; set; } = WorkflowStepState.NotStarted;
    
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? PlannedCompletionDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    
    /// <summary>
    /// Resolved parameters for this specific workflow step instance
    /// (DefaultParameters merged with crop-specific overrides)
    /// </summary>
    public WorkflowStepParameters ResolvedParameters { get; set; } = new();
    
    /// <summary>
    /// User notes for this specific step instance
    /// </summary>
    public string? UserNotes { get; set; }
    
    public ICollection<WorkflowStepHistory> History { get; set; } = new List<WorkflowStepHistory>();
}
