# Exercise Management - Architecture Diagram

## Component Hierarchy

```
ProgrammeDetail Component
│
├── Navigation
│
├── Programme Header
│   ├── Title
│   ├── Description
│   └── Active Badge
│
├── Week Tabs
│   └── WeekTab[] (mapped from programme.weeks)
│
├── Workout Days Grid
│   └── DayCard[] (mapped from currentWeek.workoutDays)
│       ├── DayHeader
│       │   ├── DayName
│       │   ├── DayOfWeek
│       │   └── Badges (Rest Day, Completed)
│       │
│       ├── ExerciseList (if not rest day)
│       │   └── ExerciseItem[] (mapped from day.exercises)
│       │       ├── ExerciseOrderHandle (⋮⋮) [draggable]
│       │       ├── Exercise Info
│       │       │   ├── Name + RPE Badge
│       │       │   ├── Sets × Reps @ Weight
│       │       │   ├── Rest time
│       │       │   └── Notes (💡)
│       │       └── Action Buttons
│       │           ├── Edit (✎)
│       │           └── Remove (×)
│       │
│       └── Add Exercise Button
│
└── Modals
    ├── Add Day Modal
    ├── Add Exercise Modal
    ├── Edit Exercise Modal
    └── Exercise Details Modal
```

## Data Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                        User Actions                          │
└───────────────┬─────────────────────────────────────────────┘
                │
                ▼
┌───────────────────────────────────────────────────────────────┐
│                   ProgrammeDetail Component                    │
│                                                                │
│  State:                                                        │
│  - programme: Programme | null                                 │
│  - selectedWeek: number                                        │
│  - showAddExerciseModal: boolean                               │
│  - showEditExerciseModal: boolean                              │
│  - showExerciseDetailsModal: boolean                           │
│  - draggedExercise: string | null                              │
│  - newExercise: { sets, reps, weight, rest, rpe, notes }     │
│  - editExercise: { id, sets, reps, weight, rest, rpe, notes }│
│                                                                │
└────────────────┬──────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│                      Handler Functions                          │
│                                                                 │
│  handleAddExercise(exerciseId)                                 │
│  handleUpdateExercise()                                        │
│  handleOpenExerciseDetails(exercise)                           │
│  handleRemoveExercise(exerciseId)                              │
│  handleDragStart/Over/End/Drop(...)                            │
│                                                                 │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│                       API Layer                                 │
│                                                                 │
│  workoutsApi                          exercisesApi             │
│  ├── addExercise()                    ├── getById()            │
│  ├── updateExercise()                 └── search()             │
│  ├── removeExercise()                                          │
│  └── reorderExercises()                                        │
│                                                                 │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│                    Backend API Endpoints                        │
│                                                                 │
│  POST   /api/workoutsession/days/{id}/exercises                │
│  PUT    /api/workoutsession/exercises/{id}                     │
│  DELETE /api/workoutsession/exercises/{id}                     │
│  PUT    /api/workoutsession/days/{id}/exercises/reorder        │
│  GET    /api/exercisedefinition/{id}                           │
│                                                                 │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│                   Backend Services                              │
│                                                                 │
│  WorkoutSessionService                                         │
│  ├── AddExerciseToWorkoutAsync()                               │
│  ├── UpdateWorkoutExerciseAsync()                              │
│  ├── RemoveExerciseFromWorkoutAsync()                          │
│  └── ReorderExercisesAsync()                                   │
│                                                                 │
│  ExerciseDefinitionService                                     │
│  └── GetByIdAsync()                                            │
│                                                                 │
└────────────────┬───────────────────────────────────────────────┘
                 │
                 ▼
┌────────────────────────────────────────────────────────────────┐
│                       Database                                  │
│                                                                 │
│  Tables:                                                        │
│  - AssignedProgrammes                                          │
│  - ProgrammeWeeks                                              │
│  - WorkoutDays                                                 │
│  - WorkoutExercises                                            │
│  - ExerciseDefinitions                                         │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## User Interaction Flows

### Flow 1: Add Exercise to Workout Day

```
User Action                    Component Response               API Call
───────────                    ──────────────────              ────────

Click "+ Add Exercise"  →  Open Add Exercise Modal
                           Show search input
                           Show parameter inputs

Type exercise name      →  Debounced search (300ms)     →  exercisesApi.search()
                           Display search results       ←  ExerciseSummary[]

Configure parameters    →  Update local state
(sets, reps, weight,       (newExercise object)
rest, RPE, notes)

Click exercise result   →  handleAddExercise()          →  workoutsApi.addExercise()
                                                         ←  WorkoutExercise

                           Reload programme              →  programmesApi.getById()
                                                         ←  Programme

                           Update local state
                           Close modal
                           Reset form
```

### Flow 2: Edit Exercise Parameters

```
User Action                    Component Response               API Call
───────────                    ──────────────────              ────────

Click edit button (✎)   →  handleOpenEditExercise()
                           Populate editExercise state
                           Open Edit Exercise Modal

Modify parameters       →  Update local state
                           (editExercise object)

Click "Save Changes"    →  handleUpdateExercise()       →  workoutsApi.updateExercise()
                                                         ←  WorkoutExercise

                           Reload programme              →  programmesApi.getById()
                                                         ←  Programme

                           Update local state
                           Close modal
```

### Flow 3: View Exercise Details

```
User Action                    Component Response               API Call
───────────                    ──────────────────              ────────

Click exercise name     →  handleOpenExerciseDetails()  →  exercisesApi.getById()
                                                         ←  Exercise (full details)

                           Set selectedExercise state
                           Open Exercise Details Modal

View information        →  Display:
                           - Description
                           - Instructions
                           - Muscle groups
                           - Video link
                           - Current targets

Click "Edit Exercise"   →  Close details modal
                           Open edit modal
```

### Flow 4: Drag-and-Drop Reordering

```
User Action                    Component Response               API Call
───────────                    ──────────────────              ────────

Drag exercise handle    →  handleDragStart()
                           Set draggedExercise state
                           Apply opacity: 0.5

Drag over target        →  handleDragOver()
                           Set dragOverExercise state
                           Apply blue border

Drop exercise           →  handleDrop()
                           Calculate new order
                           Build exercise ID array      →  workoutsApi.reorderExercises()

                           Reload programme              →  programmesApi.getById()
                                                         ←  Programme

                           Update local state
                           Reset drag states
```

## State Management Diagram

```
┌───────────────────────────────────────────────────────────────┐
│                    Component State                             │
├───────────────────────────────────────────────────────────────┤
│                                                                │
│  Programme Data State                                         │
│  ├── programme: Programme | null                              │
│  ├── loading: boolean                                         │
│  └── selectedWeek: number                                     │
│                                                                │
│  Modal Visibility State                                       │
│  ├── showAddDayModal: boolean                                 │
│  ├── showAddExerciseModal: boolean                            │
│  ├── showEditExerciseModal: boolean                           │
│  └── showExerciseDetailsModal: boolean                        │
│                                                                │
│  Form Data State                                              │
│  ├── newDay: { dayOfWeek, name, description, isRestDay }     │
│  ├── newExercise: { sets, reps, weight, rest, rpe, notes }   │
│  └── editExercise: { id, sets, reps, weight, rest, rpe, ... }│
│                                                                │
│  Search State                                                 │
│  ├── searchQuery: string                                      │
│  └── searchResults: ExerciseSummary[]                         │
│                                                                │
│  Selection State                                              │
│  ├── selectedWorkoutDayId: string | null                      │
│  └── selectedExercise: any | null                             │
│                                                                │
│  Drag-and-Drop State                                          │
│  ├── draggedExercise: string | null                           │
│  └── dragOverExercise: string | null                          │
│                                                                │
└───────────────────────────────────────────────────────────────┘
```

## Modal Component Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                      Modal (Backdrop)                         │
│  ┌────────────────────────────────────────────────────────┐  │
│  │              ModalContent (Container)                   │  │
│  │                                                         │  │
│  │  ┌─────────────────────────────────────────────────┐  │  │
│  │  │          Add Exercise Modal                      │  │  │
│  │  │                                                  │  │  │
│  │  │  Title: "Add Exercise"                          │  │  │
│  │  │                                                  │  │  │
│  │  │  Search Input                                   │  │  │
│  │  │  Parameter Inputs (Sets, Reps, Weight)         │  │  │
│  │  │  Parameter Inputs (Rest, RPE)                  │  │  │
│  │  │  Notes Input                                    │  │  │
│  │  │                                                  │  │  │
│  │  │  Exercise Search Results List                   │  │  │
│  │  │  └── ExerciseSearchItem[]                       │  │  │
│  │  │                                                  │  │  │
│  │  │  [Close Button]                                 │  │  │
│  │  └─────────────────────────────────────────────────┘  │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                      Edit Exercise Modal                      │
│  ┌────────────────────────────────────────────────────────┐  │
│  │                                                         │  │
│  │  Title: "Edit Exercise: {name}"                        │  │
│  │                                                         │  │
│  │  Stack Layout:                                         │  │
│  │  ├── Flex (Sets, Reps, Weight)                        │  │
│  │  ├── Flex (Rest, RPE)                                 │  │
│  │  └── Notes Input                                      │  │
│  │                                                         │  │
│  │  Action Buttons:                                       │  │
│  │  [Cancel]  [Save Changes]                             │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│                  Exercise Details Modal                       │
│  ┌────────────────────────────────────────────────────────┐  │
│  │                                                         │  │
│  │  Title: {exerciseName}                                 │  │
│  │                                                         │  │
│  │  Stack Layout:                                         │  │
│  │  ├── Description                                       │  │
│  │  ├── Badges (Type, Muscle Groups)                     │  │
│  │  ├── Instructions                                      │  │
│  │  ├── Video Link                                        │  │
│  │  └── Current Targets Section                          │  │
│  │      ├── Sets & Reps                                   │  │
│  │      ├── Weight & Rest                                 │  │
│  │      ├── RPE                                           │  │
│  │      └── Notes                                         │  │
│  │                                                         │  │
│  │  Action Buttons:                                       │  │
│  │  [Edit Exercise]  [Close]                             │  │
│  └────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

## API Request/Response Examples

### Add Exercise Request

```typescript
POST /api/workoutsession/days/{workoutDayId}/exercises

Request Body:
{
  "exerciseId": "uuid",
  "orderIndex": 0,
  "targetSets": 3,
  "targetReps": 10,
  "targetWeight": 100,
  "restSeconds": 90,
  "targetRpe": 8,
  "notes": "Focus on form"
}

Response:
{
  "id": "uuid",
  "exerciseId": "uuid",
  "exerciseName": "Bench Press",
  "orderIndex": 0,
  "targetSets": 3,
  "targetReps": 10,
  "targetWeight": 100,
  "restSeconds": 90,
  "targetRpe": 8,
  "notes": "Focus on form",
  "sets": []
}
```

### Update Exercise Request

```typescript
PUT /api/workoutsession/exercises/{id}

Request Body:
{
  "orderIndex": 0,
  "targetSets": 4,
  "targetReps": 8,
  "targetWeight": 110,
  "restSeconds": 120,
  "targetRpe": 9,
  "notes": "Increased weight"
}

Response:
{
  "id": "uuid",
  "exerciseId": "uuid",
  "exerciseName": "Bench Press",
  "orderIndex": 0,
  "targetSets": 4,
  "targetReps": 8,
  "targetWeight": 110,
  "restSeconds": 120,
  "targetRpe": 9,
  "notes": "Increased weight",
  "sets": [...]
}
```

### Reorder Exercises Request

```typescript
PUT /api/workoutsession/days/{workoutDayId}/exercises/reorder

Request Body: [
  "exercise-uuid-3",
  "exercise-uuid-1",
  "exercise-uuid-2"
]

Response: 204 No Content
```

## Technology Stack

```
Frontend                Backend               Database
────────               ───────               ────────

React 18                .NET 8.0              PostgreSQL
TypeScript             C#                    (via EF Core)
styled-components      ASP.NET Core
react-router-dom       Entity Framework Core
                       
Build Tools            Architecture          ORM
───────────            ────────────          ───

react-scripts          Clean Architecture    Entity Framework Core
TypeScript Compiler    Domain-Driven Design  Code-First Migrations
                       CQRS patterns
```

---

**Document Version**: 1.0  
**Last Updated**: December 27, 2025  
**Status**: Production Ready

