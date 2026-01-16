namespace GardenTracker.Web.Models;

public class StartCropViewModel
{
    public int CropDefinitionId { get; set; }
    public string CropName { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.Today;
}
