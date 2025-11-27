import apiClient from "./client";
import {
  WorkoutDay,
  CreateWorkoutDayRequest,
  WorkoutExercise,
  AddWorkoutExerciseRequest,
  ExerciseSet,
  CompleteSetRequest,
} from "../types";

export const workoutsApi = {
  // Workout Days
  getWorkoutDay: (id: string) =>
    apiClient.get<WorkoutDay>(`/workouts/days/${id}`),

  createWorkoutDay: (weekId: string, request: CreateWorkoutDayRequest) =>
    apiClient.post<WorkoutDay>(`/workouts/weeks/${weekId}/days`, request),

  updateWorkoutDay: (id: string, request: Partial<WorkoutDay>) =>
    apiClient.put<WorkoutDay>(`/workouts/days/${id}`, request),

  deleteWorkoutDay: (id: string) => apiClient.delete(`/workouts/days/${id}`),

  completeWorkout: (id: string, completedDate: string) =>
    apiClient.post<WorkoutDay>(`/workouts/days/${id}/complete`, {
      completedDate,
    }),

  // Workout Exercises
  addExercise: (workoutDayId: string, request: AddWorkoutExerciseRequest) =>
    apiClient.post<WorkoutExercise>(
      `/workouts/days/${workoutDayId}/exercises`,
      request
    ),

  updateExercise: (id: string, request: Partial<AddWorkoutExerciseRequest>) =>
    apiClient.put<WorkoutExercise>(`/workouts/exercises/${id}`, request),

  removeExercise: (id: string) => apiClient.delete(`/workouts/exercises/${id}`),

  reorderExercises: (workoutDayId: string, exerciseIds: string[]) =>
    apiClient.put(
      `/workouts/days/${workoutDayId}/exercises/reorder`,
      exerciseIds
    ),

  // Exercise Sets
  addSet: (workoutExerciseId: string, request: Partial<ExerciseSet>) =>
    apiClient.post<ExerciseSet>(
      `/workouts/exercises/${workoutExerciseId}/sets`,
      request
    ),

  updateSet: (id: string, request: Partial<ExerciseSet>) =>
    apiClient.put<ExerciseSet>(`/workouts/sets/${id}`, request),

  completeSet: (id: string, request: CompleteSetRequest) =>
    apiClient.post<ExerciseSet>(`/workouts/sets/${id}/complete`, request),

  deleteSet: (id: string) => apiClient.delete(`/workouts/sets/${id}`),
};
