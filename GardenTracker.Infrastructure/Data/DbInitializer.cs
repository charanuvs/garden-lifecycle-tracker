using GardenTracker.Domain.Entities;
using GardenTracker.Domain.ValueObjects;
using GardenTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GardenTracker.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GardenTrackerDbContext>();

        // Ensure database is created
        await context.Database.MigrateAsync();

        // Check if already seeded
        if (await context.WorkflowStepDefinitions.AnyAsync())
        {
            return; // DB has been seeded
        }

        // Seed workflow step definitions
        var workflowSteps = new List<WorkflowStepDefinition>
        {
            new()
            {
                Name = "Getting Seeds",
                Description = "Acquire or purchase seeds for planting",
                StepType = "GettingSeeds",
                DefaultParameters = new WorkflowStepParameters(durationDays: 3),
                ParameterSchema = "{\"durationDays\": \"number\", \"quantity\": \"number\"}"
            },
            new()
            {
                Name = "Preparing Soil",
                Description = "Prepare and amend soil for planting",
                StepType = "PreparingSoil",
                DefaultParameters = new WorkflowStepParameters(durationDays: 2),
                ParameterSchema = "{\"durationDays\": \"number\"}"
            },
            new()
            {
                Name = "Planting Seeds",
                Description = "Plant seeds in prepared soil",
                StepType = "PlantingSeeds",
                DefaultParameters = new WorkflowStepParameters(durationDays: 1),
                ParameterSchema = "{\"durationDays\": \"number\", \"quantity\": \"number\"}"
            },
            new()
            {
                Name = "Watering",
                Description = "Regular watering schedule",
                StepType = "Watering",
                DefaultParameters = new WorkflowStepParameters(
                    durationDays: 1, 
                    frequencyDays: 2,
                    isRecurring: true,
                    recurrenceIntervalDays: 2,
                    reminderLeadDays: 0 // Same day reminder
                ),
                ParameterSchema = "{\"durationDays\": \"number\", \"frequencyDays\": \"number\", \"isRecurring\": \"boolean\", \"recurrenceIntervalDays\": \"number\"}"
            },
            new()
            {
                Name = "Pruning",
                Description = "Prune and maintain plant health",
                StepType = "Pruning",
                DefaultParameters = new WorkflowStepParameters(durationDays: 7, frequencyDays: 7),
                ParameterSchema = "{\"durationDays\": \"number\", \"frequencyDays\": \"number\"}"
            },
            new()
            {
                Name = "Weeding",
                Description = "Remove weeds and maintain garden bed",
                StepType = "Weeding",
                DefaultParameters = new WorkflowStepParameters(durationDays: 30, frequencyDays: 7),
                ParameterSchema = "{\"durationDays\": \"number\", \"frequencyDays\": \"number\"}"
            },
            new()
            {
                Name = "Harvesting",
                Description = "Harvest mature crops",
                StepType = "Harvesting",
                DefaultParameters = new WorkflowStepParameters(durationDays: 7),
                ParameterSchema = "{\"durationDays\": \"number\", \"quantity\": \"number\"}"
            },
            new()
            {
                Name = "Clearing",
                Description = "Clear the bed after harvest",
                StepType = "Clearing",
                DefaultParameters = new WorkflowStepParameters(durationDays: 1),
                ParameterSchema = "{\"durationDays\": \"number\"}"
            }
        };

        await context.WorkflowStepDefinitions.AddRangeAsync(workflowSteps);
        await context.SaveChangesAsync();

        // Seed crop definitions
        var spinach = new CropDefinition
        {
            Name = "Spinach",
            Description = "Fast-growing leafy green, ready in 40-50 days",
            CropType = "Spinach",
            EstimatedGrowingSeasonDays = 50
        };

        var carrot = new CropDefinition
        {
            Name = "Carrot",
            Description = "Root vegetable, ready in 70-80 days",
            CropType = "Carrot",
            EstimatedGrowingSeasonDays = 75
        };

        var tomato = new CropDefinition
        {
            Name = "Tomato",
            Description = "Warm-season fruit, ready in 60-80 days after transplanting",
            CropType = "Tomato",
            EstimatedGrowingSeasonDays = 90
        };

        await context.CropDefinitions.AddRangeAsync(new[] { spinach, carrot, tomato });
        await context.SaveChangesAsync();

        // Configure spinach workflow
        var spinachWorkflow = new List<CropWorkflowConfiguration>
        {
            new()
            {
                CropDefinitionId = spinach.Id,
                WorkflowStepDefinitionId = workflowSteps[0].Id, // Getting Seeds
                Sequence = 1,
                AllowsConcurrentExecution = false,
                ParameterOverrides = new WorkflowStepParameters(durationDays: 2)
            },
            new()
            {
                CropDefinitionId = spinach.Id,
                WorkflowStepDefinitionId = workflowSteps[1].Id, // Preparing Soil
                Sequence = 2,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "GettingSeeds"
            },
            new()
            {
                CropDefinitionId = spinach.Id,
                WorkflowStepDefinitionId = workflowSteps[2].Id, // Planting Seeds
                Sequence = 3,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "PreparingSoil"
            },
            new()
            {
                CropDefinitionId = spinach.Id,
                WorkflowStepDefinitionId = workflowSteps[3].Id, // Watering
                Sequence = 4,
                AllowsConcurrentExecution = true,
                DependsOnStepTypes = "PlantingSeeds",
                ParameterOverrides = new WorkflowStepParameters(durationDays: 40, frequencyDays: 2)
            },
            new()
            {
                CropDefinitionId = spinach.Id,
                WorkflowStepDefinitionId = workflowSteps[5].Id, // Weeding
                Sequence = 5,
                AllowsConcurrentExecution = true,
                DependsOnStepTypes = "PlantingSeeds",
                ParameterOverrides = new WorkflowStepParameters(durationDays: 40, frequencyDays: 10)
            },
            new()
            {
                CropDefinitionId = spinach.Id,
                WorkflowStepDefinitionId = workflowSteps[6].Id, // Harvesting
                Sequence = 6,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "Watering",
                ParameterOverrides = new WorkflowStepParameters(durationDays: 5)
            },
            new()
            {
                CropDefinitionId = spinach.Id,
                WorkflowStepDefinitionId = workflowSteps[7].Id, // Clearing
                Sequence = 7,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "Harvesting"
            }
        };

        // Configure carrot workflow
        var carrotWorkflow = new List<CropWorkflowConfiguration>
        {
            new()
            {
                CropDefinitionId = carrot.Id,
                WorkflowStepDefinitionId = workflowSteps[0].Id, // Getting Seeds
                Sequence = 1,
                AllowsConcurrentExecution = false
            },
            new()
            {
                CropDefinitionId = carrot.Id,
                WorkflowStepDefinitionId = workflowSteps[1].Id, // Preparing Soil
                Sequence = 2,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "GettingSeeds",
                ParameterOverrides = new WorkflowStepParameters(durationDays: 3)
            },
            new()
            {
                CropDefinitionId = carrot.Id,
                WorkflowStepDefinitionId = workflowSteps[2].Id, // Planting Seeds
                Sequence = 3,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "PreparingSoil"
            },
            new()
            {
                CropDefinitionId = carrot.Id,
                WorkflowStepDefinitionId = workflowSteps[3].Id, // Watering
                Sequence = 4,
                AllowsConcurrentExecution = true,
                DependsOnStepTypes = "PlantingSeeds",
                ParameterOverrides = new WorkflowStepParameters(durationDays: 70, frequencyDays: 3)
            },
            new()
            {
                CropDefinitionId = carrot.Id,
                WorkflowStepDefinitionId = workflowSteps[5].Id, // Weeding
                Sequence = 5,
                AllowsConcurrentExecution = true,
                DependsOnStepTypes = "PlantingSeeds",
                ParameterOverrides = new WorkflowStepParameters(durationDays: 70, frequencyDays: 7)
            },
            new()
            {
                CropDefinitionId = carrot.Id,
                WorkflowStepDefinitionId = workflowSteps[6].Id, // Harvesting
                Sequence = 6,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "Watering",
                ParameterOverrides = new WorkflowStepParameters(durationDays: 7)
            },
            new()
            {
                CropDefinitionId = carrot.Id,
                WorkflowStepDefinitionId = workflowSteps[7].Id, // Clearing
                Sequence = 7,
                AllowsConcurrentExecution = false,
                DependsOnStepTypes = "Harvesting"
            }
        };

        await context.CropWorkflowConfigurations.AddRangeAsync(spinachWorkflow);
        await context.CropWorkflowConfigurations.AddRangeAsync(carrotWorkflow);
        await context.SaveChangesAsync();
    }
}
