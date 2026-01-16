using GardenTracker.Domain.Entities;

namespace GardenTracker.Domain.Interfaces;

public interface IUserCropRepository
{
    Task<UserCrop?> GetByIdAsync(int id);
    Task<UserCrop?> GetWithWorkflowStepsAsync(int id);
    Task<IEnumerable<UserCrop>> GetActiveUserCropsAsync();
    Task<IEnumerable<UserCrop>> GetAllUserCropsAsync();
    Task<UserCrop> AddAsync(UserCrop userCrop);
    Task UpdateAsync(UserCrop userCrop);
    Task DeleteAsync(int id);
}
