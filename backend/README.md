# trAInr Backend - Domain-Driven Design Architecture

This backend follows **Domain-Driven Design (DDD)** principles with **Clean Architecture** layering to ensure separation of concerns and maintainability.

## Architecture Overview

The backend is organized into four main layers following Clean Architecture principles:

```
trAInr.API (Presentation Layer)
    ↓
trAInr.Application (Application Layer)
    ↓
trAInr.Domain (Domain Layer)
    ↑
trAInr.Infrastructure (Infrastructure Layer)
```

### Layer Responsibilities

#### **Domain Layer** (`trAInr.Domain`)

- **Pure business logic** - no dependencies on external frameworks
- Contains:
  - **Aggregates**: `Athlete`, `AssignedProgram`, `ExerciseDefinition`, `WorkoutSession`
  - **Value Objects**: `Load`, `Reps`, `RPE`, `Tempo`, `FatigueIndex`, `EquipmentRequirement`
  - **Domain Events**: `ProgramAssigned`, `ProgramPhaseCompleted`, `SessionCompleted`, `SetLogged`, etc.
  - **Domain Services**: `AutoProgressionRule`, `DeloadRule`, `FatigueIndexCalculator`, `VolumeLoadCalculator`
- **No EF Core, no controllers, no DTOs**

#### **Application Layer** (`trAInr.Application`)

- **Use cases and orchestration** - coordinates domain objects to fulfill business operations
- Contains:
  - **Services**: Application services that orchestrate domain logic
  - **DTOs**: Data transfer objects for API communication
  - **Repository Interfaces**: Contracts for data access (defined here, implemented in Infrastructure)
- **No business logic** - delegates to domain layer
- **No direct database access** - uses repositories and Unit of Work pattern

#### **Infrastructure Layer** (`trAInr.Infrastructure`)

- **Technical concerns** - persistence, external integrations
- Contains:
  - **Repositories**: EF Core implementations of repository interfaces
  - **DbContext**: Entity Framework Core database context
  - **Migrations**: Database schema migrations
  - **External Services**: JWT token service, password hasher, etc.
  - **Unit of Work**: Transaction management

#### **Presentation Layer** (`trAInr.API`)

- **API endpoints** - receives HTTP requests, forwards to application layer
- Contains:
  - **Controllers**: REST API endpoints
  - **Middleware**: Authentication, CORS, etc.
- **No business logic** - only input validation and response formatting

## Domain Model

### Aggregates (Aggregate Roots)

#### 1. **Athlete** (Aggregate Root)

Represents a user/athlete with their training profile.

**Properties:**

- Personal information (username, email, name, DOB)
- Training profile (level, goal, workout days per week)
- Readiness and constraints
- Equipment preferences

**Key Methods:**

- `AdjustTrainingLevel()` - Updates training level and raises domain event
- `UpdateReadiness()` - Updates readiness score
- `UpdateProfile()` - Updates profile information
- `AddConstraint()` / `RemoveConstraint()` - Manages constraints
- `AddEquipmentPreference()` - Adds equipment preference

**Domain Events:**

- `TrainingLevelAdjusted`
- `ReadinessUpdated`

#### 2. **AssignedProgram** (Aggregate Root)

Represents a program template assigned to an athlete.

**Properties:**

- Athlete ID
- Program template ID
- Current phase
- Start/end dates
- Active status

**Key Methods:**

- `AdvancePhase()` - Advances to next phase with validation
- `ValidateProgression()` - Validates progression rules
- `Reset()` - Resets program to beginning
- `Activate()` / `Deactivate()` - Manages active status

**Domain Events:**

- `ProgramAssigned`
- `ProgramPhaseCompleted`

#### 3. **ExerciseDefinition** (Aggregate Root)

Represents a canonical exercise definition in the exercise catalog.

**Properties:**

- Name, description, instructions
- Exercise type and movement pattern
- Primary/secondary muscle groups
- Equipment requirements
- System vs. custom exercise flag

**Key Methods:**

- `ValidateUsage()` - Validates exercise can be used with available equipment
- `AddEquipmentRequirement()` - Adds equipment requirement
- `Update()` - Updates exercise definition (non-system exercises only)

#### 4. **WorkoutSession** (Aggregate Root)

Represents a single workout session with exercise instances and completed sets.

**Properties:**

- Athlete ID
- Scheduled date
- Exercise instances (owned entities)
- Completion status

**Key Methods:**

- `AddExerciseInstance()` - Adds an exercise to the session
- `AddCompletedSet()` - Records a completed set and raises domain event
- `FinalizeSession()` - Marks session as complete and raises domain event

**Domain Events:**

- `SessionCompleted`
- `SetLogged`

### Value Objects

Value objects enforce invariants and prevent invalid states:

- **Load**: Weight/load value with validation
- **Reps**: Repetition count with validation
- **RPE**: Rate of Perceived Exertion (1-10 scale)
- **Tempo**: Exercise tempo (e.g., "2-0-1-0")
- **FatigueIndex**: Calculated fatigue metric
- **EquipmentRequirement**: Equipment needed for an exercise

### Domain Services

Domain services encapsulate behavior that doesn't belong to a single aggregate:

- **AutoProgressionRule**: Applies automatic progression logic
- **DeloadRule**: Determines when and how to deload
- **FatigueIndexCalculator**: Calculates fatigue metrics
- **VolumeLoadCalculator**: Computes volume and load metrics

## Repository Pattern

Repositories provide abstraction over data access:

### Repository Interfaces (Application Layer)

- `IAthleteRepository` - Athlete aggregate operations
- `IExerciseDefinitionRepository` - Exercise definition operations
- `IAssignedProgramRepository` - Assigned program operations
- `IWorkoutSessionRepository` - Workout session operations

### Repository Implementations (Infrastructure Layer)

- `AthleteRepository` - EF Core implementation
- `ExerciseDefinitionRepository` - EF Core implementation
- `AssignedProgramRepository` - EF Core implementation
- `WorkoutSessionRepository` - EF Core implementation

## Unit of Work Pattern

The `IUnitOfWork` interface and `UnitOfWork` implementation manage database transactions:

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

Application services use `IUnitOfWork` to persist changes, maintaining the separation between Application and Infrastructure layers.

## Current Implementation Status

### ✅ Completed (DDD-Compliant)

- **Athlete Aggregate**: Fully implemented with domain methods and events
- **ExerciseDefinition Aggregate**: Fully implemented with validation
- **WorkoutSession Aggregate**: Fully implemented with exercise instances and sets
- **AssignedProgram Aggregate**: Basic implementation (needs ProgramTemplate support)
- **Repository Pattern**: All aggregate repositories implemented
- **User/Auth Services**: Refactored to use Athlete aggregate
- **Exercise Service**: Refactored to use ExerciseDefinition aggregate

### 🚧 In Progress / TODO

- **ProgrammeService**: Needs refactoring to use AssignedProgram aggregate
- **WorkoutService**: Needs refactoring to use WorkoutSession aggregate
- **DashboardService**: Needs refactoring to use aggregates
- **ProgramTemplate Aggregate**: Needs to be created for programme templates
- **Domain Events Dispatching**: Needs implementation in Application layer
- **Value Objects**: Some value objects need full implementation

## Key Invariants (Domain Rules)

1. **WorkoutSession** cannot contain exercises not defined in ExerciseCatalog
2. **AssignedProgram** cannot advance phases without satisfying progression rules
3. **CompletedSet** must validate against target sets and exercise constraints
4. **Athlete** constraints must be respected by program templates and session creation
5. **ExerciseDefinition** system exercises cannot be updated or deleted

## Database Context

The `TrainrDbContext` is configured in the Infrastructure layer and maps aggregates to database tables:

- Uses owned entity types for value objects (e.g., `ExerciseInstance`, `CompletedSet`)
- JSON serialization for complex value object collections
- Proper aggregate boundary enforcement through EF Core configuration

## API Endpoints

### Users (Athletes)

- `GET /api/users` - Get all athletes
- `GET /api/users/{id}` - Get athlete by ID
- `POST /api/users` - Create athlete
- `PUT /api/users/{id}` - Update athlete
- `DELETE /api/users/{id}` - Delete athlete

### Authentication

- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Register new athlete

### Exercises

- `GET /api/exercises` - Get all exercises
- `GET /api/exercises/{id}` - Get exercise by ID
- `GET /api/exercises/search` - Search exercises
- `POST /api/exercises` - Create custom exercise
- `PUT /api/exercises/{id}` - Update exercise
- `DELETE /api/exercises/{id}` - Delete exercise

### Programmes (Needs Refactoring)

- Currently uses old entity model - needs migration to AssignedProgram aggregate

### Workouts (Needs Refactoring)

- Currently uses old entity model - needs migration to WorkoutSession aggregate

## Development Guidelines

### Adding New Features

1. **Start with Domain Layer**: Define aggregates, value objects, and domain events
2. **Add Domain Methods**: Encapsulate business logic in aggregate methods
3. **Create Repository Interface**: Define in Application layer
4. **Implement Repository**: Create EF Core implementation in Infrastructure
5. **Create Application Service**: Orchestrate domain objects
6. **Add API Endpoint**: Create controller in Presentation layer

### Domain Rules

- **Never** access database directly from Domain or Application layers
- **Never** put business logic in controllers or infrastructure
- **Always** use aggregates for data modification
- **Always** raise domain events for important state changes
- **Always** validate invariants in aggregate methods

### Testing Strategy

- **Unit Tests**: Test domain logic in isolation
- **Integration Tests**: Test repository implementations
- **API Tests**: Test end-to-end workflows

## Migration Path

For services still using old entities:

1. Identify the aggregate that should replace the entity
2. Refactor service to use aggregate repository
3. Update DTOs to map from aggregate properties
4. Update controllers if needed
5. Remove old entity and repository interfaces

## References

- Domain-Driven Design rules: `.cursor/rules/domain-driven-design.mdc`
- Clean Architecture principles
- Entity Framework Core documentation
