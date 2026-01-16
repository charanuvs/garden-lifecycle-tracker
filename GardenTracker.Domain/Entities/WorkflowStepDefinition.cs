using GardenTracker.Domain.Common;
using GardenTracker.Domain.ValueObjects;

namespace GardenTracker.Domain.Entities;

/// <summary>
/// Defines a reusable workflow step template (e.g., "Preparing Soil", "Watering", "Pruning")
/// </summary>
public class WorkflowStepDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string StepType { get; set; } = string.Empty; // e.g., "PreparingSoil", "Watering", "Pruning"
    
    /// <summary>
    /// Default parameters for this step (can be overridden at crop level)
    /// </summary>
    public WorkflowStepParameters DefaultParameters { get; set; } = new();
    
    /// <summary>
    /// JSON schema defining what parameters this step accepts
    /// </summary>
    public string ParameterSchema { get; set; } = "{}";
    
    public ICollection<CropWorkflowConfiguration> CropWorkflows { get; set; } = new List<CropWorkflowConfiguration>();
}
