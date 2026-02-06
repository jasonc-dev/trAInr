/**
 * Exercises API Service
 * Handles exercise search and details
 */

import { apiClient } from "./client";
import type {
  ExerciseSummary,
  ExerciseDefinition,
  ExerciseType,
  MuscleGroup,
} from "../types/programme";

export const exercisesApi = {
  /**
   * Search exercises
   */
  async searchExercises(
    query: string,
    type?: ExerciseType,
    muscleGroup?: MuscleGroup,
  ): Promise<ExerciseSummary[]> {
    const params = new URLSearchParams();
    if (query) params.append("query", query);
    if (type !== undefined) params.append("type", type.toString());
    if (muscleGroup !== undefined)
      params.append("muscleGroup", muscleGroup.toString());

    const response = await apiClient.get<ExerciseSummary[]>(
      `/ExerciseDefinition/search?${params.toString()}`,
    );
    return response.data;
  },

  /**
   * Get exercise details by ID
   */
  async getExerciseById(id: number): Promise<ExerciseDefinition> {
    const response = await apiClient.get<ExerciseDefinition>(
      `/ExerciseDefinition/${id}`,
    );
    return response.data;
  },
};
