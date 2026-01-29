/**
 * Workout API Service
 * Handles workout days, exercises, and sets operations
 */

import apiClient from "./client";
import {
  WorkoutDayResponse,
  WorkoutExerciseResponse,
  ExerciseSetResponse,
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

  createWorkoutDay: (weekId: string, request: CreateWorkoutDayRequest) =>
    apiClient.post<WorkoutDayResponse>(
      `/workoutsession/weeks/${weekId}/days`,
      request,
    ),

  updateWorkoutDay: (id: string, request: UpdateWorkoutDayRequest) =>
    apiClient.put<WorkoutDayResponse>(`/workoutsession/days/${id}`, request),

  deleteWorkoutDay: (id: string) =>
    apiClient.delete(`/workoutsession/days/${id}`),

  completeWorkout: (id: string, request: CompleteWorkoutRequest) =>
    apiClient.post<WorkoutDayResponse>(
      `/workoutsession/days/${id}/complete`,
      request,
    ),

  // Exercise operations
  addExercise: (workoutDayId: string, request: AddWorkoutExerciseRequest) =>
    apiClient.post<WorkoutExerciseResponse>(
      `/workoutsession/days/${workoutDayId}/exercises`,
      request,
    ),

  updateExercise: (exerciseId: number, request: UpdateWorkoutExerciseRequest) =>
    apiClient.put<WorkoutExerciseResponse>(
      `/workoutsession/exercises/${exerciseId}`,
      request,
    ),

  removeExercise: (exerciseId: string) =>
    apiClient.delete(`/workoutsession/exercises/${exerciseId}`),

  reorderExercises: (workoutDayId: string, exerciseIds: string[]) =>
    apiClient.put(
      `/workoutsession/days/${workoutDayId}/exercises/reorder`,
      exerciseIds,
    ),

  // Set operations
  addSet: (workoutExerciseId: string, request: CreateExerciseSetRequest) =>
    apiClient.post<ExerciseSetResponse>(
      `/workoutsession/exercises/${workoutExerciseId}/sets`,
      request,
    ),

  updateSet: (setId: string, request: UpdateExerciseSetRequest) =>
    apiClient.put<ExerciseSetResponse>(
      `/workoutsession/sets/${setId}`,
      request,
    ),

  completeSet: (setId: string, request: CompleteSetRequest) =>
    apiClient.post<ExerciseSetResponse>(
      `/workoutsession/sets/${setId}/complete`,
      request,
    ),

  deleteSet: (setId: string) =>
    apiClient.delete(`/workoutsession/sets/${setId}`),

  // Superset operations
  groupSuperset: (workoutDayId: string, request: GroupSupersetRequest) =>
    apiClient.put<WorkoutExerciseResponse[]>(
      `/workoutsession/days/${workoutDayId}/exercises/superset`,
      request,
    ),

  ungroupSuperset: (supersetGroupId: string) =>
    apiClient.delete(`/workoutsession/exercises/superset/${supersetGroupId}`),

  // Drop set operations
  createDropSetSequence: (
    workoutExerciseId: number,
    request: CreateDropSetRequest,
  ) =>
    apiClient.post<ExerciseSetResponse[]>(
      `/workoutsession/exercises/${workoutExerciseId}/dropsets`,
      request,
    ),
};
