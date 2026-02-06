/**
 * Workouts API Service
 * Handles workout days, exercises, and sets operations
 */

import { apiClient } from "./client";
import type {
  WorkoutDayResponse,
  WorkoutExerciseResponse,
  ProgrammeWeek,
  CreateWorkoutDayRequest,
  UpdateWorkoutDayRequest,
  CompleteWorkoutRequest,
  AddWorkoutExerciseRequest,
  UpdateWorkoutExerciseRequest,
  CreateExerciseSetRequest,
  UpdateExerciseSetRequest,
  CompleteSetRequest,
  GroupSupersetRequest,
} from "../types/programme";

export const workoutsApi = {
  /**
   * Get workout day details
   */
  async getWorkoutDay(id: string): Promise<WorkoutDayResponse> {
    const response = await apiClient.get<WorkoutDayResponse>(
      `/WorkoutSession/days/${id}`
    );
    return response.data;
  },

  /**
   * Create workout day (returns updated week)
   */
  async createWorkoutDay(
    weekId: string,
    data: CreateWorkoutDayRequest
  ): Promise<ProgrammeWeek> {
    const response = await apiClient.post<ProgrammeWeek>(
      `/WorkoutSession/weeks/${weekId}/days`,
      data
    );
    return response.data;
  },

  /**
   * Update workout day (returns updated week)
   */
  async updateWorkoutDay(
    id: string,
    data: UpdateWorkoutDayRequest
  ): Promise<ProgrammeWeek> {
    const response = await apiClient.put<ProgrammeWeek>(
      `/WorkoutSession/days/${id}`,
      data
    );
    return response.data;
  },

  /**
   * Delete workout day (returns updated week)
   */
  async deleteWorkoutDay(id: string): Promise<ProgrammeWeek> {
    const response = await apiClient.delete<ProgrammeWeek>(
      `/WorkoutSession/days/${id}`
    );
    return response.data;
  },

  /**
   * Complete workout
   */
  async completeWorkout(
    id: string,
    data: CompleteWorkoutRequest
  ): Promise<ProgrammeWeek> {
    const response = await apiClient.post<ProgrammeWeek>(
      `/WorkoutSession/days/${id}/complete`,
      data
    );
    return response.data;
  },

  /**
   * Add exercise to workout day (returns updated day)
   */
  async addExercise(
    workoutDayId: string,
    data: AddWorkoutExerciseRequest
  ): Promise<WorkoutDayResponse> {
    const response = await apiClient.post<WorkoutDayResponse>(
      `/WorkoutSession/days/${workoutDayId}/exercises`,
      data
    );
    return response.data;
  },

  /**
   * Update exercise (returns updated day)
   */
  async updateExercise(
    exerciseId: string,
    data: UpdateWorkoutExerciseRequest
  ): Promise<WorkoutDayResponse> {
    const response = await apiClient.put<WorkoutDayResponse>(
      `/WorkoutSession/exercises/${exerciseId}`,
      data
    );
    return response.data;
  },

  /**
   * Remove exercise (returns updated day)
   */
  async removeExercise(exerciseId: string): Promise<WorkoutDayResponse> {
    const response = await apiClient.delete<WorkoutDayResponse>(
      `/WorkoutSession/exercises/${exerciseId}`
    );
    return response.data;
  },

  /**
   * Reorder exercises in a workout day (returns updated day)
   */
  async reorderExercises(
    workoutDayId: string,
    exerciseIds: string[]
  ): Promise<WorkoutDayResponse> {
    const response = await apiClient.put<WorkoutDayResponse>(
      `/WorkoutSession/days/${workoutDayId}/exercises/reorder`,
      exerciseIds
    );
    return response.data;
  },

  /**
   * Group exercises as superset (returns updated day)
   */
  async groupSuperset(
    workoutDayId: string,
    data: GroupSupersetRequest
  ): Promise<WorkoutDayResponse> {
    const response = await apiClient.put<WorkoutDayResponse>(
      `/WorkoutSession/days/${workoutDayId}/exercises/superset`,
      data
    );
    return response.data;
  },

  /**
   * Ungroup superset (returns updated day)
   */
  async ungroupSuperset(supersetGroupId: string): Promise<WorkoutDayResponse> {
    const response = await apiClient.delete<WorkoutDayResponse>(
      `/WorkoutSession/exercises/superset/${supersetGroupId}`
    );
    return response.data;
  },

  /**
   * Add a set to an exercise (returns updated exercise)
   */
  async addSet(
    workoutExerciseId: string,
    data: CreateExerciseSetRequest
  ): Promise<WorkoutExerciseResponse> {
    const response = await apiClient.post<WorkoutExerciseResponse>(
      `/WorkoutSession/exercises/${workoutExerciseId}/sets`,
      data
    );
    return response.data;
  },

  /**
   * Complete a set (returns updated exercise)
   */
  async completeSet(
    setId: string,
    data: CompleteSetRequest
  ): Promise<WorkoutExerciseResponse> {
    const response = await apiClient.post<WorkoutExerciseResponse>(
      `/WorkoutSession/sets/${setId}/complete`,
      data
    );
    return response.data;
  },

  /**
   * Update set details (returns updated exercise)
   */
  async updateSet(
    setId: string,
    data: UpdateExerciseSetRequest
  ): Promise<WorkoutExerciseResponse> {
    const response = await apiClient.put<WorkoutExerciseResponse>(
      `/WorkoutSession/sets/${setId}`,
      data
    );
    return response.data;
  },

  /**
   * Delete a set (returns updated exercise)
   */
  async deleteSet(setId: string): Promise<WorkoutExerciseResponse> {
    const response = await apiClient.delete<WorkoutExerciseResponse>(
      `/WorkoutSession/sets/${setId}`
    );
    return response.data;
  },
};
