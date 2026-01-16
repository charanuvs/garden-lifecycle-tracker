using GardenTracker.Domain.Entities;

namespace GardenTracker.Domain.Interfaces;

public interface IWorkflowStepDefinitionRepository
{
    Task<WorkflowStepDefinition?> GetByIdAsync(int id);
    Task<WorkflowStepDefinition?> GetByStepTypeAsync(string stepType);
    Task<IEnumerable<WorkflowStepDefinition>> GetAllAsync();
    Task<WorkflowStepDefinition> AddAsync(WorkflowStepDefinition definition);
    Task UpdateAsync(WorkflowStepDefinition definition);
    Task DeleteAsync(int id);
}
