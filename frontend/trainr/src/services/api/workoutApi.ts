/**
 * Workout API Service
 * Handles workout days, exercises, and sets operations
 */

import apiClient from "./client";
import {
  WorkoutDay,
  WorkoutExercise,
  ExerciseSet,
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
    apiClient.get<WorkoutDay>(`/workoutsession/days/${id}`),

  getWorkoutWeeks: (programmeId: string) =>
    apiClient.get<ProgrammeWeek[]>(
      `/workoutsession/programme/${programmeId}/weeks`,
    ),

  getWorkoutDays: (weekId: string) =>
    apiClient.get<WorkoutDay[]>(`/workoutsession/weeks/${weekId}/days`),

  createWorkoutDay: (weekId: string, request: CreateWorkoutDayRequest) =>
    apiClient.post<WorkoutDay>(`/workoutsession/weeks/${weekId}/days`, request),

  updateWorkoutDay: (id: string, request: UpdateWorkoutDayRequest) =>
    apiClient.put<WorkoutDay>(`/workoutsession/days/${id}`, request),

  deleteWorkoutDay: (id: string) =>
    apiClient.delete(`/workoutsession/days/${id}`),

  completeWorkout: (id: string, request: CompleteWorkoutRequest) =>
    apiClient.post<WorkoutDay>(`/workoutsession/days/${id}/complete`, request),

  // Exercise operations
  addExercise: (workoutDayId: string, request: AddWorkoutExerciseRequest) =>
    apiClient.post<WorkoutExercise>(
      `/workoutsession/days/${workoutDayId}/exercises`,
      request,
    ),

  updateExercise: (exerciseId: number, request: UpdateWorkoutExerciseRequest) =>
    apiClient.put<WorkoutExercise>(
      `/workoutsession/exercises/${exerciseId}`,
      request,
    ),

  removeExercise: (exerciseId: number) =>
    apiClient.delete(`/workoutsession/exercises/${exerciseId}`),

  reorderExercises: (workoutDayId: string, exerciseIds: number[]) =>
    apiClient.put(
      `/workoutsession/days/${workoutDayId}/exercises/reorder`,
      exerciseIds,
    ),

  // Set operations
  addSet: (workoutExerciseId: number, request: CreateExerciseSetRequest) =>
    apiClient.post<ExerciseSet>(
      `/workoutsession/exercises/${workoutExerciseId}/sets`,
      request,
    ),

  updateSet: (setId: string, request: UpdateExerciseSetRequest) =>
    apiClient.put<ExerciseSet>(`/workoutsession/sets/${setId}`, request),

  completeSet: (setId: string, request: CompleteSetRequest) =>
    apiClient.post<ExerciseSet>(
      `/workoutsession/sets/${setId}/complete`,
      request,
    ),

  deleteSet: (setId: string) =>
    apiClient.delete(`/workoutsession/sets/${setId}`),

  // Superset operations
  groupSuperset: (workoutDayId: string, request: GroupSupersetRequest) =>
    apiClient.put<WorkoutExercise[]>(
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
    apiClient.post<ExerciseSet[]>(
      `/workoutsession/exercises/${workoutExerciseId}/dropsets`,
      request,
    ),
};
