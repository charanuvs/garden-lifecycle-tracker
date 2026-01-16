using GardenTracker.Domain.Entities;
using GardenTracker.Domain.Interfaces;
using GardenTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenTracker.Infrastructure.Repositories;

public class CropDefinitionRepository : ICropDefinitionRepository
{
    private readonly GardenTrackerDbContext _context;

    public CropDefinitionRepository(GardenTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<CropDefinition?> GetByIdAsync(int id)
    {
        return await _context.CropDefinitions.FindAsync(id);
    }

    public async Task<CropDefinition?> GetByCropTypeAsync(string cropType)
    {
        return await _context.CropDefinitions
            .FirstOrDefaultAsync(c => c.CropType == cropType);
    }

    public async Task<IEnumerable<CropDefinition>> GetAllAsync()
    {
        return await _context.CropDefinitions.ToListAsync();
    }

    public async Task<CropDefinition?> GetWithWorkflowsAsync(int id)
    {
        return await _context.CropDefinitions
            .Include(c => c.WorkflowConfigurations)
                .ThenInclude(w => w.WorkflowStepDefinition)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<CropDefinition> AddAsync(CropDefinition definition)
    {
        await _context.CropDefinitions.AddAsync(definition);
        return definition;
    }

    public Task UpdateAsync(CropDefinition definition)
    {
        _context.CropDefinitions.Update(definition);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var definition = await GetByIdAsync(id);
        if (definition != null)
        {
            _context.CropDefinitions.Remove(definition);
        }
    }
}
