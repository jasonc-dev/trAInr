/**
 * Workout API Service
 * Handles workout days, exercises, and sets operations
 */

import apiClient from "./client";
import {
  WorkoutDayResponse,
  WorkoutExerciseResponse,
  CreateWorkoutDayRequest,
  UpdateWorkoutDayRequest,
  CompleteWorkoutRequest,
  AddWorkoutExerciseRequest,
  UpdateWorkoutExerciseRequest,
  CreateExerciseSetRequest,
  UpdateExerciseSetRequest,
  CompleteSetRequest,
  GroupSupersetRequest,
  CreateDropSetRequest,
  ProgrammeWeek,
} from "../../types";

export const workoutApi = {
  // Workout Day operations
  getWorkoutDay: (id: string) =>
    apiClient.get<WorkoutDayResponse>(`/workoutsession/days/${id}`),

  getWorkoutWeeks: (programmeId: string) =>
    apiClient.get<ProgrammeWeek[]>(
      `/workoutsession/programme/${programmeId}/weeks`,
    ),

  getWorkoutDays: (weekId: string) =>
    apiClient.get<WorkoutDayResponse[]>(`/workoutsession/weeks/${weekId}/days`),

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

  // Set operations - now return WorkoutExerciseResponse
  addSet: (workoutExerciseId: string, request: CreateExerciseSetRequest) =>
    apiClient.post<WorkoutExerciseResponse>(
      `/workoutsession/exercises/${workoutExerciseId}/sets`,
      request,
    ),

  updateSet: (setId: string, request: UpdateExerciseSetRequest) =>
    apiClient.put<WorkoutExerciseResponse>(
      `/workoutsession/sets/${setId}`,
      request,
    ),

  completeSet: (setId: string, request: CompleteSetRequest) =>
    apiClient.post<WorkoutExerciseResponse>(
      `/workoutsession/sets/${setId}/complete`,
      request,
    ),

  deleteSet: (setId: string) =>
    apiClient.delete<WorkoutExerciseResponse>(`/workoutsession/sets/${setId}`),

  // Superset operations - now return WorkoutDayResponse
  groupSuperset: (workoutDayId: string, request: GroupSupersetRequest) =>
    apiClient.put<WorkoutDayResponse>(
      `/workoutsession/days/${workoutDayId}/exercises/superset`,
      request,
    ),

  ungroupSuperset: (supersetGroupId: string) =>
    apiClient.delete<WorkoutDayResponse>(
      `/workoutsession/exercises/superset/${supersetGroupId}`,
    ),

  // Drop set operations - now return WorkoutExerciseResponse
  createDropSetSequence: (
    workoutExerciseId: number,
    request: CreateDropSetRequest,
  ) =>
    apiClient.post<WorkoutExerciseResponse>(
      `/workoutsession/exercises/${workoutExerciseId}/dropsets`,
      request,
    ),
};
