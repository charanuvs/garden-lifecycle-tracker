using GardenTracker.Domain.Entities;

namespace GardenTracker.Domain.Interfaces;

public interface IActiveWorkflowStepRepository
{
    Task<ActiveWorkflowStep?> GetByIdAsync(int id);
    Task<ActiveWorkflowStep?> GetWithHistoryAsync(int id);
    Task<IEnumerable<ActiveWorkflowStep>> GetByUserCropIdAsync(int userCropId);
    Task<IEnumerable<ActiveWorkflowStep>> GetActiveStepsAsync();
    Task<IEnumerable<ActiveWorkflowStep>> GetAllAsync();
    Task<IEnumerable<ActiveWorkflowStep>> GetNextStepsAsync(int userCropId);
    Task<ActiveWorkflowStep> AddAsync(ActiveWorkflowStep step);
    Task UpdateAsync(ActiveWorkflowStep step);
    Task DeleteAsync(int id);
}
