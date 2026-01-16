using GardenTracker.Domain.Entities;

namespace GardenTracker.Domain.Interfaces;

public interface ICropDefinitionRepository
{
    Task<CropDefinition?> GetByIdAsync(int id);
    Task<CropDefinition?> GetByCropTypeAsync(string cropType);
    Task<IEnumerable<CropDefinition>> GetAllAsync();
    Task<CropDefinition?> GetWithWorkflowsAsync(int id);
    Task<CropDefinition> AddAsync(CropDefinition definition);
    Task UpdateAsync(CropDefinition definition);
    Task DeleteAsync(int id);
}
