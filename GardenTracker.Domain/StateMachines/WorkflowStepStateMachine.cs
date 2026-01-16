using GardenTracker.Domain.Enums;
using Stateless;

namespace GardenTracker.Domain.StateMachines;

/// <summary>
/// Base class for workflow step state machines
/// </summary>
public class WorkflowStepStateMachine
{
    private readonly StateMachine<WorkflowStepState, WorkflowStepTrigger> _stateMachine;
    
    public WorkflowStepState CurrentState => _stateMachine.State;
    
    public event Action<WorkflowStepState, WorkflowStepState, WorkflowStepTrigger>? StateChanged;
    
    public WorkflowStepStateMachine(WorkflowStepState initialState)
    {
        _stateMachine = new StateMachine<WorkflowStepState, WorkflowStepTrigger>(initialState);
        
        ConfigureStateMachine();
    }
    
    protected virtual void ConfigureStateMachine()
    {
        _stateMachine.Configure(WorkflowStepState.NotStarted)
            .Permit(WorkflowStepTrigger.Start, WorkflowStepState.InProgress)
            .Permit(WorkflowStepTrigger.Skip, WorkflowStepState.Skipped);
        
        _stateMachine.Configure(WorkflowStepState.InProgress)
            .Permit(WorkflowStepTrigger.Complete, WorkflowStepState.Completed)
            .Permit(WorkflowStepTrigger.Reset, WorkflowStepState.NotStarted);
        
        _stateMachine.Configure(WorkflowStepState.Completed)
            .Permit(WorkflowStepTrigger.Reset, WorkflowStepState.NotStarted);
        
        _stateMachine.Configure(WorkflowStepState.Skipped)
            .Permit(WorkflowStepTrigger.Reset, WorkflowStepState.NotStarted);
        
        // Subscribe to state transitions
        _stateMachine.OnTransitioned(t =>
        {
            StateChanged?.Invoke(t.Source, t.Destination, t.Trigger);
        });
    }
    
    public bool CanFire(WorkflowStepTrigger trigger)
    {
        return _stateMachine.CanFire(trigger);
    }
    
    public void Fire(WorkflowStepTrigger trigger)
    {
        if (CanFire(trigger))
        {
            _stateMachine.Fire(trigger);
        }
        else
        {
            throw new InvalidOperationException(
                $"Cannot fire trigger {trigger} from state {CurrentState}");
        }
    }
    
    public IEnumerable<WorkflowStepTrigger> PermittedTriggers => _stateMachine.PermittedTriggers;
}
