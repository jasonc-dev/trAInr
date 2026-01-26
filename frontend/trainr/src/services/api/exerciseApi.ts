/**
 * Exercise API Service
 * Handles exercise definition operations
 */

import apiClient from "./client";
import {
  Exercise,
  ExerciseSummary,
  ExerciseType,
  MuscleGroup,
} from "../../types";

export const exerciseApi = {
  getAll: () => apiClient.get<Exercise[]>("/exercisedefinition"),

  getById: (id: number) => apiClient.get<Exercise>(`/exercisedefinition/${id}`),

  search: (query?: string, type?: ExerciseType, muscleGroup?: MuscleGroup) => {
    const params = new URLSearchParams();
    if (query) params.append("query", query);
    if (type !== undefined) params.append("type", type.toString());
    if (muscleGroup !== undefined)
      params.append("muscleGroup", muscleGroup.toString());
    return apiClient.get<ExerciseSummary[]>(
      `/exercisedefinition/search?${params.toString()}`,
    );
  },

  getByType: (type: ExerciseType) =>
    apiClient.get<ExerciseSummary[]>(`/exercisedefinition/type/${type}`),

  getByMuscleGroup: (muscleGroup: MuscleGroup) =>
    apiClient.get<ExerciseSummary[]>(
      `/exercisedefinition/muscle-group/${muscleGroup}`,
    ),
};
