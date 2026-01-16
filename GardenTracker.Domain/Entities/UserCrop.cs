using GardenTracker.Domain.Common;

namespace GardenTracker.Domain.Entities;

/// <summary>
/// A user's instance of a crop they're growing
/// </summary>
public class UserCrop : BaseEntity
{
    public int CropDefinitionId { get; set; }
    public CropDefinition CropDefinition { get; set; } = null!;
    
    public string Nickname { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    public ICollection<ActiveWorkflowStep> ActiveWorkflowSteps { get; set; } = new List<ActiveWorkflowStep>();
}
