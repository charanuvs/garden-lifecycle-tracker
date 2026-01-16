# Garden Lifecycle Tracker - Features

## Core Features

### 1. Finite State Machine (FSM) Workflow Management
- Each workflow step uses the Stateless library for state management
- States: NotStarted → InProgress → Completed (or Skipped)
- Triggers: Start, Complete, Skip, Reset
- Automatic state transition validation

### 2. Time Range Scheduling ⏰
**NEW**: Each workflow step now has a scheduled time range

- **ScheduledStartDate**: When the step should begin
- **ScheduledEndDate**: When the step should be completed
- Automatically calculated based on crop start date and step dependencies
- Displayed in the UI with clear visual indicators

**Example**: 
- Planting Seeds: Jan 10-11, 2026
- Watering: Jan 12-13, 2026 (first occurrence)

### 3. Daily Reminder System 🔔
**NEW**: Automated reminder notifications for upcoming tasks

**How it works**:
1. Background service runs every hour
2. Checks all workflow steps once per day
3. Sends reminders starting **1 day before** the scheduled start date
4. Continues daily until the step is started
5. Automatically stops when step state changes from "NotStarted"

**Configurable per step**:
- `ReminderLeadDays`: How many days before to start reminders (default: 1)
- `IsReminderActive`: Enable/disable reminders for specific steps

**Current Implementation**:
- Console-based notifications (logs and console output)
- Ready to replace with email, SMS, or push notifications
- See `ConsoleNotificationService.cs` for implementation

### 4. Recurring Workflow Steps 🔄
**NEW**: Steps that automatically repeat at regular intervals

**Perfect for**:
- Watering crops every 2 days
- Weekly weeding
- Regular pruning schedules

**Configuration Parameters**:
- `IsRecurring`: Mark step as recurring
- `RecurrenceIntervalDays`: Days between recurrences (e.g., 2 for every 2 days)
- `MaxRecurrences`: Optional limit on total occurrences (null = infinite)

**How it works**:
1. When you complete a recurring step, a new instance is automatically created
2. The next occurrence is scheduled based on `RecurrenceIntervalDays`
3. Each instance is numbered (1st, 2nd, 3rd, etc.)
4. Notifications are sent when new recurring tasks are created
5. Each recurrence has its own time range and reminder schedule

**Example - Watering**:
```
1st Watering: Jan 12-13 (complete) → Creates 2nd Watering
2nd Watering: Jan 14-15 (auto-created, scheduled)
3rd Watering: Jan 16-17 (will be created when 2nd is completed)
```

### 5. Concurrent Workflow Execution
- Multiple crops can be grown simultaneously
- Each crop has its own independent workflow
- All concurrent workflows are visible in "My Garden" view

### 6. Parameterized Workflow Steps
Each step can be configured with:
- **DurationDays**: How long the step takes
- **FrequencyDays**: For recurring activities
- **Quantity**: Amount (seeds, water, etc.)
- **Notes**: Additional instructions
- **CustomParameters**: Extensible key-value pairs
- **IsRecurring**: Whether step repeats
- **RecurrenceIntervalDays**: Days between recurrences
- **MaxRecurrences**: Limit on repetitions
- **ReminderLeadDays**: When to start reminders

### 7. Reusable Workflow Steps
- Step definitions are templates (e.g., "Watering", "Pruning")
- Each crop can override default parameters
- Example: Tomatoes need more water than spinach

### 8. Dependency Management
- Steps can depend on other steps being completed
- Prevents starting steps out of order
- Ensures proper crop lifecycle progression

## Technical Architecture

### Domain Layer
- **Entities**: CropDefinition, UserCrop, ActiveWorkflowStep, WorkflowStepDefinition, etc.
- **ValueObjects**: WorkflowStepParameters (with recurring and reminder fields)
- **Enums**: WorkflowStepState, WorkflowStepTrigger
- **StateMachines**: WorkflowStepStateMachine using Stateless library
- **Interfaces**: INotificationService, repository interfaces

### Infrastructure Layer
- **DbContext**: EF Core 9.0 with SQLite
- **Repositories**: UnitOfWork pattern with specialized repositories
- **Services**:
  - `CropWorkflowService`: Manages workflow lifecycle, creates recurring steps
  - `ReminderService`: Checks and sends daily reminders
  - `ConsoleNotificationService`: Sends notifications (replaceable)
- **BackgroundServices**: `DailyReminderBackgroundService` for automated checks

### Web Layer
- ASP.NET Core 9.0 MVC
- Bootstrap 5 UI
- Controllers: CropsController, UserCropsController
- Views: Responsive design with workflow progress tracking

## Database Schema Updates

### ActiveWorkflowSteps Table (New Fields)
```sql
ScheduledStartDate       DATETIME  -- When step should start
ScheduledEndDate         DATETIME  -- When step should end
LastReminderSentDate     DATETIME  -- Track last reminder sent
IsReminderActive         BOOLEAN   -- Enable/disable reminders
IsRecurringInstance      BOOLEAN   -- Is this a recurring instance
RecurrenceNumber         INTEGER   -- 1st, 2nd, 3rd occurrence
```

### WorkflowStepParameters (Updated)
```json
{
  "DurationDays": 1,
  "FrequencyDays": 2,
  "IsRecurring": true,
  "RecurrenceIntervalDays": 2,
  "MaxRecurrences": null,
  "ReminderLeadDays": 1
}
```

## Sample Crops

### Spinach (50 days total)
1. Getting Seeds (3 days)
2. Preparing Soil (2 days)
3. Planting Seeds (1 day)
4. **Watering** (recurring every 2 days) 🔄
5. Weeding (as needed)
6. Harvesting (7 days)
7. Clearing (1 day)

### Carrot (75 days total)
Similar workflow with longer growing period

### Tomato (90 days total)
Includes additional pruning steps

## How to Use Recurring Steps

### When Starting a Crop:
1. Browse available crops
2. Start a crop with a nickname (e.g., "My Tomatoes")
3. System automatically creates all workflow steps with calculated time ranges
4. Recurring steps (like Watering) are created for their first occurrence

### When Working Through Steps:
1. View "My Garden" to see all active crops
2. Click on a crop to see pending tasks
3. Each task shows:
   - ⏰ Scheduled time range
   - 🔄 Recurrence indicator (if applicable)
   - Recurrence number (#1, #2, #3...)
4. Start and complete tasks as scheduled
5. When you complete a recurring task, the next occurrence is automatically created

### Reminder Notifications:
- Check console output for reminder messages
- Format: "🌱 REMINDER: 'Watering' for Crop #1 is scheduled from Jan 14 to Jan 15"
- Reminders appear daily starting 1 day before scheduled time
- Stop automatically when you start the task

## Customization

### Replace Notification Service
Replace `ConsoleNotificationService` with your own implementation:

```csharp
public class EmailNotificationService : INotificationService
{
    public async Task SendReminderAsync(...)
    {
        // Send email using SMTP, SendGrid, etc.
    }
    
    public async Task SendRecurringStepCreatedAsync(...)
    {
        // Send email notification
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddSingleton<INotificationService, EmailNotificationService>();
```

### Adjust Reminder Timing
Modify reminder lead time per step:
```csharp
new WorkflowStepParameters(
    reminderLeadDays: 2  // Start reminders 2 days before
)
```

### Configure Recurring Steps
```csharp
new WorkflowStepParameters(
    isRecurring: true,
    recurrenceIntervalDays: 7,      // Weekly
    maxRecurrences: 10,              // Stop after 10 occurrences
    reminderLeadDays: 1              // Remind 1 day before
)
```

## Future Enhancements

Potential additions:
- Email/SMS notification integration
- Mobile app with push notifications
- Calendar integration (Google Calendar, iCal)
- User-configurable reminder preferences
- Snooze/postpone reminders
- Weather-aware scheduling
- Harvest tracking and yield analysis
- Photo uploads for progress tracking
- Community sharing of crop workflows
