using GardenTracker.Domain.Interfaces;
using GardenTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace GardenTracker.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GardenTrackerDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private ICropDefinitionRepository? _cropDefinitions;
    private IWorkflowStepDefinitionRepository? _workflowStepDefinitions;
    private IUserCropRepository? _userCrops;
    private IActiveWorkflowStepRepository? _activeWorkflowSteps;

    public UnitOfWork(GardenTrackerDbContext context)
    {
        _context = context;
    }

    public ICropDefinitionRepository CropDefinitions =>
        _cropDefinitions ??= new CropDefinitionRepository(_context);

    public IWorkflowStepDefinitionRepository WorkflowStepDefinitions =>
        _workflowStepDefinitions ??= new WorkflowStepDefinitionRepository(_context);

    public IUserCropRepository UserCrops =>
        _userCrops ??= new UserCropRepository(_context);

    public IActiveWorkflowStepRepository ActiveWorkflowSteps =>
        _activeWorkflowSteps ??= new ActiveWorkflowStepRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
