using GardenTracker.Domain.Entities;
using GardenTracker.Domain.Enums;
using GardenTracker.Domain.Interfaces;
using GardenTracker.Domain.StateMachines;

namespace GardenTracker.Infrastructure.Services;

public class CropWorkflowService
{
    private readonly IUnitOfWork _unitOfWork;

    public CropWorkflowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserCrop> StartCropAsync(int cropDefinitionId, string nickname, DateTime startDate)
    {
        var cropDefinition = await _unitOfWork.CropDefinitions.GetWithWorkflowsAsync(cropDefinitionId);
        if (cropDefinition == null)
        {
            throw new InvalidOperationException($"Crop definition {cropDefinitionId} not found");
        }

        // Create the user crop
        var userCrop = new UserCrop
        {
            CropDefinitionId = cropDefinitionId,
            Nickname = nickname,
            StartDate = startDate,
            IsActive = true
        };

        await _unitOfWork.UserCrops.AddAsync(userCrop);
        await _unitOfWork.SaveChangesAsync();

        // Initialize all workflow steps based on crop configuration
        await InitializeWorkflowStepsAsync(userCrop, cropDefinition, startDate);

        return userCrop;
    }

    private async Task InitializeWorkflowStepsAsync(UserCrop userCrop, CropDefinition cropDefinition, DateTime startDate)
    {
        var orderedConfigurations = cropDefinition.WorkflowConfigurations
            .OrderBy(c => c.Sequence)
            .ToList();

        DateTime currentPlannedDate = startDate;

        foreach (var config in orderedConfigurations)
        {
            // Resolve parameters (merge defaults with overrides)
            var resolvedParams = config.WorkflowStepDefinition.DefaultParameters
                .Merge(config.ParameterOverrides);

            var step = new ActiveWorkflowStep
            {
                UserCropId = userCrop.Id,
                WorkflowStepDefinitionId = config.WorkflowStepDefinitionId,
                CurrentState = WorkflowStepState.NotStarted,
                PlannedStartDate = currentPlannedDate,
                ResolvedParameters = resolvedParams
            };

            // Calculate planned completion date based on duration
            if (resolvedParams.DurationDays.HasValue)
            {
                step.PlannedCompletionDate = currentPlannedDate.AddDays(resolvedParams.DurationDays.Value);
                currentPlannedDate = step.PlannedCompletionDate.Value;
            }

            await _unitOfWork.ActiveWorkflowSteps.AddAsync(step);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ActiveWorkflowStep> TransitionStepAsync(int stepId, WorkflowStepTrigger trigger, string? notes = null)
    {
        var step = await _unitOfWork.ActiveWorkflowSteps.GetWithHistoryAsync(stepId);
        if (step == null)
        {
            throw new InvalidOperationException($"Active workflow step {stepId} not found");
        }

        // Create state machine and validate transition
        var stateMachine = new WorkflowStepStateMachine(step.CurrentState);
        
        if (!stateMachine.CanFire(trigger))
        {
            throw new InvalidOperationException(
                $"Cannot fire trigger {trigger} from state {step.CurrentState}");
        }

        var previousState = step.CurrentState;
        stateMachine.Fire(trigger);
        var newState = stateMachine.CurrentState;

        // Update step state and dates
        step.CurrentState = newState;
        
        if (trigger == WorkflowStepTrigger.Start)
        {
            step.ActualStartDate = DateTime.UtcNow;
        }
        else if (trigger == WorkflowStepTrigger.Complete)
        {
            step.ActualCompletionDate = DateTime.UtcNow;
        }

        // Record history
        var history = new WorkflowStepHistory
        {
            ActiveWorkflowStepId = step.Id,
            FromState = previousState,
            ToState = newState,
            Trigger = trigger,
            TransitionDate = DateTime.UtcNow,
            Notes = notes
        };

        step.History.Add(history);

        await _unitOfWork.ActiveWorkflowSteps.UpdateAsync(step);
        await _unitOfWork.SaveChangesAsync();

        return step;
    }

    public async Task<IEnumerable<ActiveWorkflowStep>> GetNextStepsForCropAsync(int userCropId)
    {
        var userCrop = await _unitOfWork.UserCrops.GetWithWorkflowStepsAsync(userCropId);
        if (userCrop == null)
        {
            throw new InvalidOperationException($"User crop {userCropId} not found");
        }

        var cropDefinition = await _unitOfWork.CropDefinitions.GetWithWorkflowsAsync(userCrop.CropDefinitionId);
        if (cropDefinition == null)
        {
            return Enumerable.Empty<ActiveWorkflowStep>();
        }

        var allSteps = await _unitOfWork.ActiveWorkflowSteps.GetByUserCropIdAsync(userCropId);
        var nextSteps = new List<ActiveWorkflowStep>();

        foreach (var step in allSteps)
        {
            // Skip completed or skipped steps
            if (step.CurrentState == WorkflowStepState.Completed || 
                step.CurrentState == WorkflowStepState.Skipped)
            {
                continue;
            }

            // Find the configuration for this step
            var config = cropDefinition.WorkflowConfigurations
                .FirstOrDefault(c => c.WorkflowStepDefinitionId == step.WorkflowStepDefinitionId);

            if (config == null) continue;

            // Check dependencies
            if (!string.IsNullOrEmpty(config.DependsOnStepTypes))
            {
                var dependencyStepTypes = config.DependsOnStepTypes.Split(',');
                var allDependenciesMet = true;

                foreach (var depStepType in dependencyStepTypes)
                {
                    var dependencyStep = allSteps.FirstOrDefault(s => 
                        s.WorkflowStepDefinition.StepType == depStepType.Trim());

                    if (dependencyStep == null || dependencyStep.CurrentState != WorkflowStepState.Completed)
                    {
                        allDependenciesMet = false;
                        break;
                    }
                }

                if (!allDependenciesMet) continue;
            }

            nextSteps.Add(step);
        }

        return nextSteps.OrderBy(s => s.PlannedStartDate);
    }

    public async Task<IEnumerable<UserCrop>> GetAllActiveUserCropsAsync()
    {
        return await _unitOfWork.UserCrops.GetActiveUserCropsAsync();
    }
}
