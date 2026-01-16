using GardenTracker.Domain.Common;
using GardenTracker.Domain.Enums;

namespace GardenTracker.Domain.Entities;

/// <summary>
/// Audit trail of state transitions for a workflow step
/// </summary>
public class WorkflowStepHistory : BaseEntity
{
    public int ActiveWorkflowStepId { get; set; }
    public ActiveWorkflowStep ActiveWorkflowStep { get; set; } = null!;
    
    public WorkflowStepState FromState { get; set; }
    public WorkflowStepState ToState { get; set; }
    public WorkflowStepTrigger Trigger { get; set; }
    
    public DateTime TransitionDate { get; set; }
    public string? Notes { get; set; }
}
