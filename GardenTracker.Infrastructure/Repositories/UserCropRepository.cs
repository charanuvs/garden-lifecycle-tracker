using GardenTracker.Domain.Entities;
using GardenTracker.Domain.Interfaces;
using GardenTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenTracker.Infrastructure.Repositories;

public class UserCropRepository : IUserCropRepository
{
    private readonly GardenTrackerDbContext _context;

    public UserCropRepository(GardenTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<UserCrop?> GetByIdAsync(int id)
    {
        return await _context.UserCrops.FindAsync(id);
    }

    public async Task<UserCrop?> GetWithWorkflowStepsAsync(int id)
    {
        return await _context.UserCrops
            .Include(u => u.CropDefinition)
            .Include(u => u.ActiveWorkflowSteps)
                .ThenInclude(a => a.WorkflowStepDefinition)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<UserCrop>> GetActiveUserCropsAsync()
    {
        return await _context.UserCrops
            .Include(u => u.CropDefinition)
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserCrop>> GetAllUserCropsAsync()
    {
        return await _context.UserCrops
            .Include(u => u.CropDefinition)
            .ToListAsync();
    }

    public async Task<UserCrop> AddAsync(UserCrop userCrop)
    {
        await _context.UserCrops.AddAsync(userCrop);
        return userCrop;
    }

    public Task UpdateAsync(UserCrop userCrop)
    {
        _context.UserCrops.Update(userCrop);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var userCrop = await GetByIdAsync(id);
        if (userCrop != null)
        {
            _context.UserCrops.Remove(userCrop);
        }
    }
}
