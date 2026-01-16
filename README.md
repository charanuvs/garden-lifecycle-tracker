# Garden Lifecycle Tracker

ASP.NET Core MVC application for managing garden crop lifecycles using finite state machines.

## Features

- **Reusable Workflow Steps**: Define steps like "Watering", "Pruning", "Harvesting" once and reuse across different crops
- **Parameterized Workflows**: Customize duration, frequency, and other parameters per crop type
- **Concurrent Execution**: Run multiple workflow steps in parallel (e.g., water while weeding)
- **State Machine Powered**: Robust state management using the Stateless library
- **Dependency Management**: Define step dependencies to ensure proper sequencing
- **Date Calculation**: Automatic scheduling of next tasks based on parameters

## Architecture

The solution follows clean architecture principles:

- **GardenTracker.Web**: ASP.NET Core MVC presentation layer
- **GardenTracker.Domain**: Core domain entities, interfaces, and FSM logic
- **GardenTracker.Infrastructure**: EF Core, repositories, and services
- **GardenTracker.Tests**: Unit and integration tests

## Technologies

- .NET 9.0
- ASP.NET Core MVC
- Entity Framework Core 9.0
- SQLite (cross-platform database)
- Stateless (Finite State Machine library)
- Bootstrap 5

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- No additional database installation required (uses SQLite)

### Setup

1. Clone the repository:
```bash
git clone <repository-url>
cd workparty-ai
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Apply database migrations:
```bash
dotnet ef database update --project GardenTracker.Infrastructure --startup-project GardenTracker.Web
```

4. Run the application:
```bash
dotnet run --project GardenTracker.Web
```

5. Navigate to `https://localhost:5001` in your browser

## Database Schema

The application uses SQL Server LocalDB with the following main entities:

- **CropDefinition**: Defines crop types (Spinach, Carrot, etc.)
- **WorkflowStepDefinition**: Reusable workflow step templates
- **CropWorkflowConfiguration**: Maps steps to crops with parameters
- **UserCrop**: User's crop instances
- **ActiveWorkflowStep**: Running workflow step instances
- **WorkflowStepHistory**: Audit trail of state transitions

## Workflow System

### Finite State Machines

Each workflow step is managed by a state machine with the following states:
- **NotStarted**: Initial state
- **InProgress**: Step is currently being worked on
- **Completed**: Step finished successfully
- **Skipped**: Step was skipped

### Transitions

Available triggers:
- **Start**: Begin a step
- **Complete**: Finish a step
- **Skip**: Skip a step
- **Reset**: Reset to initial state

### Concurrent Workflows

Workflows can execute concurrently when configured. For example:
- Watering and Weeding can run simultaneously
- Pruning can occur while Watering continues
- Dependencies ensure sequential steps run in order

## Supported Crops

The application comes pre-seeded with:
- **Spinach** (40-50 day growing season)
- **Carrot** (70-80 day growing season)
- **Tomato** (60-90 day growing season)

## Development

### Running Tests

```bash
dotnet test
```

### Adding New Crops

1. Navigate to Browse Crops
2. Add new crop definition via seed data or admin UI
3. Configure workflow steps with parameters
4. Set dependencies and sequencing

### Adding New Workflow Steps

1. Create WorkflowStepDefinition
2. Define parameter schema
3. Associate with crops via CropWorkflowConfiguration
4. Set default parameters

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.
