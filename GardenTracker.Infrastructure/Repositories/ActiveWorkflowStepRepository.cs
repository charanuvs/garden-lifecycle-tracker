using GardenTracker.Domain.Entities;
using GardenTracker.Domain.Enums;
using GardenTracker.Domain.Interfaces;
using GardenTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GardenTracker.Infrastructure.Repositories;

public class ActiveWorkflowStepRepository : IActiveWorkflowStepRepository
{
    private readonly GardenTrackerDbContext _context;

    public ActiveWorkflowStepRepository(GardenTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<ActiveWorkflowStep?> GetByIdAsync(int id)
    {
        return await _context.ActiveWorkflowSteps.FindAsync(id);
    }

    public async Task<ActiveWorkflowStep?> GetWithHistoryAsync(int id)
    {
        return await _context.ActiveWorkflowSteps
            .Include(a => a.History)
            .Include(a => a.WorkflowStepDefinition)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<ActiveWorkflowStep>> GetByUserCropIdAsync(int userCropId)
    {
        return await _context.ActiveWorkflowSteps
            .Include(a => a.WorkflowStepDefinition)
            .Where(a => a.UserCropId == userCropId)
            .OrderBy(a => a.PlannedStartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActiveWorkflowStep>> GetActiveStepsAsync()
    {
        return await _context.ActiveWorkflowSteps
            .Include(a => a.WorkflowStepDefinition)
            .Include(a => a.UserCrop)
            .Where(a => a.CurrentState == WorkflowStepState.InProgress)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActiveWorkflowStep>> GetAllAsync()
    {
        return await _context.ActiveWorkflowSteps
            .Include(a => a.WorkflowStepDefinition)
            .Include(a => a.UserCrop)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActiveWorkflowStep>> GetNextStepsAsync(int userCropId)
    {
        return await _context.ActiveWorkflowSteps
            .Include(a => a.WorkflowStepDefinition)
            .Where(a => a.UserCropId == userCropId && 
                       (a.CurrentState == WorkflowStepState.NotStarted || 
                        a.CurrentState == WorkflowStepState.InProgress))
            .OrderBy(a => a.PlannedStartDate)
            .ToListAsync();
    }

    public async Task<ActiveWorkflowStep> AddAsync(ActiveWorkflowStep step)
    {
        await _context.ActiveWorkflowSteps.AddAsync(step);
        return step;
    }

    public Task UpdateAsync(ActiveWorkflowStep step)
    {
        _context.ActiveWorkflowSteps.Update(step);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var step = await GetByIdAsync(id);
        if (step != null)
        {
            _context.ActiveWorkflowSteps.Remove(step);
        }
    }
}
