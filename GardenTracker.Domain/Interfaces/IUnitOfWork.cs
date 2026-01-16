namespace GardenTracker.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ICropDefinitionRepository CropDefinitions { get; }
    IWorkflowStepDefinitionRepository WorkflowStepDefinitions { get; }
    IUserCropRepository UserCrops { get; }
    IActiveWorkflowStepRepository ActiveWorkflowSteps { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
