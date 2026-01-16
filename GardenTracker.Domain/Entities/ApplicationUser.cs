using Microsoft.AspNetCore.Identity;

namespace GardenTracker.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public ICollection<UserCrop> UserCrops { get; set; } = new List<UserCrop>();
}
