using GardenTracker.Domain.Common;
using GardenTracker.Domain.ValueObjects;

namespace GardenTracker.Domain.Entities;

/// <summary>
/// Maps a workflow step to a specific crop with crop-specific parameters and sequencing
/// </summary>
public class CropWorkflowConfiguration : BaseEntity
{
    public int CropDefinitionId { get; set; }
    public CropDefinition CropDefinition { get; set; } = null!;
    
    public int WorkflowStepDefinitionId { get; set; }
    public WorkflowStepDefinition WorkflowStepDefinition { get; set; } = null!;
    
    /// <summary>
    /// Order in which this step should appear (for sequential workflows)
    /// </summary>
    public int Sequence { get; set; }
    
    /// <summary>
    /// Can this step run concurrently with other steps?
    /// </summary>
    public bool AllowsConcurrentExecution { get; set; }
    
    /// <summary>
    /// Comma-separated list of step types that must complete before this step can start
    /// </summary>
    public string? DependsOnStepTypes { get; set; }
    
    /// <summary>
    /// Crop-specific parameter overrides
    /// </summary>
    public WorkflowStepParameters? ParameterOverrides { get; set; }
}
