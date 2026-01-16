using GardenTracker.Domain.Entities;
using GardenTracker.Domain.Interfaces;
using GardenTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenTracker.Infrastructure.Repositories;

public class WorkflowStepDefinitionRepository : IWorkflowStepDefinitionRepository
{
    private readonly GardenTrackerDbContext _context;

    public WorkflowStepDefinitionRepository(GardenTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<WorkflowStepDefinition?> GetByIdAsync(int id)
    {
        return await _context.WorkflowStepDefinitions.FindAsync(id);
    }

    public async Task<WorkflowStepDefinition?> GetByStepTypeAsync(string stepType)
    {
        return await _context.WorkflowStepDefinitions
            .FirstOrDefaultAsync(w => w.StepType == stepType);
    }

    public async Task<IEnumerable<WorkflowStepDefinition>> GetAllAsync()
    {
        return await _context.WorkflowStepDefinitions.ToListAsync();
    }

    public async Task<WorkflowStepDefinition> AddAsync(WorkflowStepDefinition definition)
    {
        await _context.WorkflowStepDefinitions.AddAsync(definition);
        return definition;
    }

    public Task UpdateAsync(WorkflowStepDefinition definition)
    {
        _context.WorkflowStepDefinitions.Update(definition);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var definition = await GetByIdAsync(id);
        if (definition != null)
        {
            _context.WorkflowStepDefinitions.Remove(definition);
        }
    }
}
