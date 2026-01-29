---
name: Optimize Workout API Responses
overview: Refactor workout session APIs to return contextual data (week/day/exercise) instead of just the updated entity, eliminating the need for a second API call to fetch the full programme after every modification.
todos:
  - id: backend-interface
    content: Update IWorkoutSessionService interface with new return types (ProgrammeWeekResponse, WorkoutDayResponse, WorkoutExerciseResponse)
    status: completed
  - id: backend-service-impl
    content: Update WorkoutSessionService to return contextual data for all operations
    status: completed
  - id: backend-cached-service
    content: Update CachedWorkoutSessionService wrapper methods to match new signatures
    status: completed
  - id: backend-controller
    content: Update WorkoutSessionController endpoints to return new response types
    status: completed
  - id: frontend-api-types
    content: Update workoutApi.ts return types to match backend changes
    status: completed
  - id: frontend-helpers
    content: Create programmeUpdaters.ts utility functions for merging contextual data
    status: completed
  - id: frontend-programme-detail
    content: Update ProgrammeDetail.tsx handlers to use returned contextual data
    status: completed
  - id: frontend-workout-detail
    content: Update WorkoutDetail.tsx set operations to use returned data
    status: completed
  - id: frontend-hooks
    content: Update useWorkouts.ts hook to handle contextual data from set operations
    status: completed
isProject: false
---

# Optimize Workout API Responses with Contextual Data

## Overview

Currently, the frontend makes two API calls for every workout modification:

1. Update/create operation (e.g., `workoutsApi.updateWorkoutDay()`)
2. Fetch full programme (`programmesApi.getById()`) to refresh UI

This refactoring will make operations return contextual data, eliminating the second call.

## Strategy: Contextual Data Returns

Instead of returning just the modified entity, each operation will return its parent context:

- **Workout Day operations** → Return `ProgrammeWeekResponse` (containing all days in the week)
- **Exercise operations** → Return `WorkoutDayResponse` (containing all exercises in the day)
- **Set operations** → Return `WorkoutExerciseResponse` (containing all sets in the exercise)

This provides enough context for the frontend to update its local state without fetching the entire programme.

## Backend Changes

### 1. Update Service Interface

**File:** `[backend/trAInr.Application/Interfaces/Services/IWorkoutSessionService.cs](backend/trAInr.Application/Interfaces/Services/IWorkoutSessionService.cs)`

Update method signatures to return contextual data:

```csharp
// Workout Day operations - return ProgrammeWeekResponse
Task<ProgrammeWeekResponse?> CreateWorkoutDayAsync(Guid weekId, CreateWorkoutDayRequest request);
Task<ProgrammeWeekResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request);
Task<ProgrammeWeekResponse?> DeleteWorkoutDayAsync(Guid workoutDayId);
Task<ProgrammeWeekResponse?> CompleteWorkoutAsync(Guid workoutDayId, CompleteWorkoutRequest request);

// Exercise operations - return WorkoutDayResponse
Task<WorkoutDayResponse?> AddExerciseToWorkoutAsync(Guid workoutDayId, AddWorkoutExerciseRequest request);
Task<WorkoutDayResponse?> UpdateWorkoutExerciseAsync(Guid workoutExerciseId, UpdateWorkoutExerciseRequest request);
Task<WorkoutDayResponse?> RemoveExerciseFromWorkoutAsync(Guid workoutExerciseId);
Task<WorkoutDayResponse?> ReorderExercisesAsync(Guid workoutDayId, List<Guid> workoutExerciseIds);
Task<WorkoutDayResponse?> GroupExercisesInSupersetAsync(Guid workoutDayId, GroupSupersetRequest request);
Task<WorkoutDayResponse?> UngroupExercisesFromSupersetAsync(Guid supersetGroupId);

// Set operations - return WorkoutExerciseResponse
Task<WorkoutExerciseResponse?> AddSetAsync(Guid workoutExerciseId, CreateExerciseSetRequest request);
Task<WorkoutExerciseResponse?> UpdateSetAsync(Guid setId, UpdateExerciseSetRequest request);
Task<WorkoutExerciseResponse?> CompleteSetAsync(Guid setId, CompleteSetRequest request);
Task<WorkoutExerciseResponse?> DeleteSetAsync(Guid setId);
Task<WorkoutExerciseResponse?> CreateDropSetSequenceAsync(Guid workoutExerciseId, CreateDropSetRequest request);
```

### 2. Update Service Implementation

**File:** `[backend/trAInr.Application/Services/WorkoutSession/WorkoutSessionService.cs](backend/trAInr.Application/Services/WorkoutSession/WorkoutSessionService.cs)`

For each method:

1. Perform the operation as before
2. Fetch and return the parent context (week/day/exercise)

Example for `UpdateWorkoutDayAsync`:

```csharp
public async Task<ProgrammeWeekResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request)
{
    var assignedProgram = await assignedProgramRepository.GetByWorkoutDayIdAsync(workoutDayId);
    if (assignedProgram is null) return null;

    var workoutDay = assignedProgram.Weeks
        .SelectMany(w => w.WorkoutDays)
        .FirstOrDefault(d => d.Id == workoutDayId);
    if (workoutDay is null) return null;

    // Update the workout day
    workoutDay.Name = request.Name;
    workoutDay.Description = request.Description;
    // ... other updates

    await assignedProgramRepository.UpdateAsync(assignedProgram);
    await unitOfWork.SaveChangesAsync();

    // Return the containing week
    var week = assignedProgram.Weeks.First(w => w.WorkoutDays.Any(d => d.Id == workoutDayId));
    return MapWeekToResponse(week);
}
```

### 3. Update Cached Service

**File:** `[backend/trAInr.Application/Services/WorkoutSession/CachedWorkoutSessionService.cs](backend/trAInr.Application/Services/WorkoutSession/CachedWorkoutSessionService.cs)`

Update wrapper methods to:

1. Call inner service (which now returns contextual data)
2. Invalidate caches as before
3. Return the contextual data

Example:

```csharp
public async Task<ProgrammeWeekResponse?> UpdateWorkoutDayAsync(Guid workoutDayId, UpdateWorkoutDayRequest request)
{
    var week = await _inner.UpdateWorkoutDayAsync(workoutDayId, request);
    if (week is not null)
    {
        await InvalidateWorkoutDayCaches(workoutDayId);
        await InvalidateProgrammeCacheByWorkoutDayId(workoutDayId);
    }
    return week;
}
```

### 4. Update Controllers

**File:** `[backend/trAInr.API/Controllers/WorkoutSessionController.cs](backend/trAInr.API/Controllers/WorkoutSessionController.cs)`

Update controller methods to return new response types:

```csharp
[HttpPost("weeks/{weekId:guid}/days")]
public async Task<ActionResult<ProgrammeWeekResponse>> CreateWorkoutDay(
    Guid weekId,
    [FromBody] CreateWorkoutDayRequest request)
{
    var week = await workoutSessionService.CreateWorkoutDayAsync(weekId, request);
    if (week is null) return NotFound("Week not found");
    return Ok(week);  // Returns the week, not just the day
}

[HttpPut("days/{id:guid}")]
public async Task<ActionResult<ProgrammeWeekResponse>> UpdateWorkoutDay(
    Guid id,
    [FromBody] UpdateWorkoutDayRequest request)
{
    var week = await workoutSessionService.UpdateWorkoutDayAsync(id, request);
    if (week is null) return NotFound();
    return Ok(week);
}

// ... similar updates for all other operations
```

## Frontend Changes

### 5. Update API Service Types

**File:** `[frontend/trainr/src/services/api/workoutApi.ts](frontend/trainr/src/services/api/workoutApi.ts)`

Update return types to match new backend responses:

```typescript
export const workoutApi = {
  // Workout Day operations - now return ProgrammeWeek
  createWorkoutDay: (weekId: string, request: CreateWorkoutDayRequest) =>
    apiClient.post<ProgrammeWeek>(
      `/workoutsession/weeks/${weekId}/days`,
      request,
    ),

  updateWorkoutDay: (id: string, request: UpdateWorkoutDayRequest) =>
    apiClient.put<ProgrammeWeek>(`/workoutsession/days/${id}`, request),

  deleteWorkoutDay: (id: string) =>
    apiClient.delete<ProgrammeWeek>(`/workoutsession/days/${id}`),

  completeWorkout: (id: string, request: CompleteWorkoutRequest) =>
    apiClient.post<ProgrammeWeek>(
      `/workoutsession/days/${id}/complete`,
      request,
    ),

  // Exercise operations - now return WorkoutDayResponse
  addExercise: (workoutDayId: string, request: AddWorkoutExerciseRequest) =>
    apiClient.post<WorkoutDayResponse>(
      `/workoutsession/days/${workoutDayId}/exercises`,
      request,
    ),

  updateExercise: (exerciseId: number, request: UpdateWorkoutExerciseRequest) =>
    apiClient.put<WorkoutDayResponse>(
      `/workoutsession/exercises/${exerciseId}`,
      request,
    ),

  removeExercise: (exerciseId: string) =>
    apiClient.delete<WorkoutDayResponse>(
      `/workoutsession/exercises/${exerciseId}`,
    ),

  reorderExercises: (workoutDayId: string, exerciseIds: string[]) =>
    apiClient.put<WorkoutDayResponse>(
      `/workoutsession/days/${workoutDayId}/exercises/reorder`,
      exerciseIds,
    ),

  // ... similar updates for superset, set operations
};
```

### 6. Create Helper Functions

**File:** `[frontend/trainr/src/utils/programmeUpdaters.ts](frontend/trainr/src/utils/programmeUpdaters.ts)` (new file)

Create utility functions to merge contextual data into programme state:

```typescript
export const updateProgrammeWithWeek = (
  programme: Programme,
  updatedWeek: ProgrammeWeek,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) =>
      week.id === updatedWeek.id ? updatedWeek : week,
    ),
  };
};

export const updateProgrammeWithDay = (
  programme: Programme,
  updatedDay: WorkoutDayResponse,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) => ({
      ...week,
      workoutDays: week.workoutDays.map((day) =>
        day.id === updatedDay.id ? updatedDay : day,
      ),
    })),
  };
};

export const updateProgrammeWithExercise = (
  programme: Programme,
  updatedExercise: WorkoutExerciseResponse,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) => ({
      ...week,
      workoutDays: week.workoutDays.map((day) => ({
        ...day,
        exercises: day.exercises.map((ex) =>
          ex.id === updatedExercise.id ? updatedExercise : ex,
        ),
      })),
    })),
  };
};
```

### 7. Update ProgrammeDetail Component

**File:** `[frontend/trainr/src/pages/ProgrammeDetail.tsx](frontend/trainr/src/pages/ProgrammeDetail.tsx)`

Update all handler functions to use returned contextual data. Examples:

**handleAddDay** (lines 476-504):

```typescript
const handleAddDay = async () => {
  if (!programme || !programme.weeks[selectedWeek]) return;
  try {
    const weekId = programme.weeks[selectedWeek].id;
    const response = await workoutsApi.createWorkoutDay(weekId, {
      scheduledDate: newDay.scheduledDate,
      name: newDay.name || `Day ${existingDays}`,
      description: newDay.description,
      isRestDay: newDay.isRestDay,
    });

    // Use returned week data directly
    setProgramme(updateProgrammeWithWeek(programme, response.data));
    setShowAddDayModal(false);
    // ... reset form
  } catch (err) {
    console.error("Failed to add day:", err);
  }
};
```

**handleUpdateDay** (lines 523-542):

```typescript
const handleUpdateDay = async () => {
  if (!editDay.id) return;
  try {
    const response = await workoutsApi.updateWorkoutDay(editDay.id, {
      scheduledDate: editDay.scheduledDate,
      name: editDay.name,
      description: editDay.description,
      isRestDay: editDay.isRestDay,
    });

    // Use returned week data directly
    setProgramme(updateProgrammeWithWeek(programme!, response.data));
    setShowEditDayModal(false);
    setSelectedWorkoutDayId(null);
  } catch (err) {
    console.error("Failed to update day:", err);
  }
};
```

**handleAddExercise** (lines 565-605):

```typescript
const handleAddExercise = async (exerciseId: number) => {
  if (!selectedWorkoutDayId) return;
  try {
    const workoutDay = programme?.weeks[selectedWeek]?.workoutDays.find(
      (d) => d.id === selectedWorkoutDayId,
    );
    const orderIndex = workoutDay?.exercises.length || 0;

    const response = await workoutsApi.addExercise(selectedWorkoutDayId, {
      exerciseId,
      orderIndex,
      targetSets: newExercise.targetSets,
      targetReps: newExercise.targetReps,
      targetWeight: newExercise.targetWeight || undefined,
      restSeconds: newExercise.restSeconds || undefined,
      targetRpe: newExercise.targetRpe || undefined,
      notes: newExercise.notes || undefined,
    });

    // Use returned day data directly
    setProgramme(updateProgrammeWithDay(programme!, response.data));
    setShowAddExerciseModal(false);
    // ... reset form
  } catch (err) {
    console.error("Failed to add exercise:", err);
  }
};
```

Apply similar pattern to:

- `handleDeleteDay` (lines 549-563)
- `handleUpdateExercise` (lines 624-646)
- `handleDrop` (lines 672-719) - reorder exercises
- `handleRemoveExercise` (lines 721-729)
- `handleGroupAsSuperset` (lines 743-759)
- `handleUngroupSuperset` (lines 761-769)
- `handleCreateDropSet` (lines 784-800)
- `handleCopyWeek` (lines 802-821)

### 8. Update WorkoutDetail Component

**File:** `[frontend/trainr/src/pages/WorkoutDetail.tsx](frontend/trainr/src/pages/WorkoutDetail.tsx)`

Update set operations to use returned exercise data:

**handleAddSet** (lines 241-256):

```typescript
const handleAddSet = async (
  workoutExerciseId: string,
  exercise: WorkoutExerciseResponse,
) => {
  const setNumber = exercise.sets.length + 1;
  try {
    const response = await addSet(workoutExerciseId, {
      setNumber,
      reps: exercise.targetReps,
      weight: exercise.targetWeight,
    });
    // addSet in hook should handle updating currentWorkout with returned exercise
  } catch (err) {
    console.error("Failed to add set:", err);
  }
};
```

**handleCompleteSet** (lines 258-276):

```typescript
const handleCompleteSet = async (setId: string) => {
  const localSet = localSets[setId];
  if (!localSet) return;
  setCompletingSet(setId);
  try {
    const request: CompleteSetRequest = {
      reps: localSet.reps,
      weight: localSet.weight,
      intensity: localSet.intensity,
    };
    await completeSet(setId, request);
    // completeSet in hook should handle updating currentWorkout with returned exercise
  } catch (err) {
    console.error("Failed to complete set:", err);
  } finally {
    setCompletingSet(null);
  }
};
```

### 9. Update useWorkouts Hook

**File:** `[frontend/trainr/src/hooks/useWorkouts.ts](frontend/trainr/src/hooks/useWorkouts.ts)`

Update the hook to use returned contextual data for set operations:

```typescript
const addSet = async (
  workoutExerciseId: string,
  request: CreateExerciseSetRequest,
) => {
  const response = await workoutsApi.addSet(workoutExerciseId, request);
  if (currentWorkout) {
    // Update the exercise in current workout with returned data
    setCurrentWorkout({
      ...currentWorkout,
      exercises: currentWorkout.exercises.map((ex) =>
        ex.id === workoutExerciseId ? response.data : ex,
      ),
    });
  }
  return response.data;
};

const completeSet = async (setId: string, request: CompleteSetRequest) => {
  const response = await workoutsApi.completeSet(setId, request);
  if (currentWorkout) {
    // Update the exercise in current workout with returned data
    setCurrentWorkout({
      ...currentWorkout,
      exercises: currentWorkout.exercises.map((ex) =>
        ex.sets.some((s) => s.id === setId) ? response.data : ex,
      ),
    });
  }
  return response.data;
};
```

## Implementation Order

1. Backend Service Interface changes
2. Backend Service Implementation (WorkoutSessionService)
3. Backend Cached Service wrapper updates
4. Backend Controller updates
5. Frontend API service type updates
6. Frontend helper functions
7. Frontend ProgrammeDetail component updates
8. Frontend WorkoutDetail component updates
9. Frontend useWorkouts hook updates

## Benefits

- **Reduced API calls**: Eliminates ~13 redundant `getById` calls in ProgrammeDetail alone
- **Lower backend load**: Fewer database queries and cache lookups
- **Faster UI updates**: Single round-trip instead of two
- **Maintained cache consistency**: Cache invalidation logic remains unchanged
- **Better UX**: Faster perceived performance for users

## Testing Strategy

For each updated operation, verify:

1. Operation completes successfully
2. Returned data includes correct parent context
3. Frontend updates local state correctly
4. No second API call is made
5. Cache invalidation still works properly
