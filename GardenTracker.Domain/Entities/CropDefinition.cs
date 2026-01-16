using GardenTracker.Domain.Common;

namespace GardenTracker.Domain.Entities;

/// <summary>
/// Defines a crop type with its associated workflow steps (e.g., "Spinach", "Carrot")
/// </summary>
public class CropDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CropType { get; set; } = string.Empty; // e.g., "Spinach", "Carrot"
    
    /// <summary>
    /// Estimated total growing season in days
    /// </summary>
    public int EstimatedGrowingSeasonDays { get; set; }
    
    public ICollection<CropWorkflowConfiguration> WorkflowConfigurations { get; set; } = new List<CropWorkflowConfiguration>();
    public ICollection<UserCrop> UserCrops { get; set; } = new List<UserCrop>();
}
