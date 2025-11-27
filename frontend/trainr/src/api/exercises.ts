import apiClient from "./client";
import { Exercise, ExerciseSummary, ExerciseType, MuscleGroup } from "../types";

export const exercisesApi = {
  getAll: () => apiClient.get<Exercise[]>("/exercises"),

  search: (query?: string, type?: ExerciseType, muscleGroup?: MuscleGroup) => {
    const params = new URLSearchParams();
    if (query) params.append("query", query);
    if (type !== undefined) params.append("type", type.toString());
    if (muscleGroup !== undefined)
      params.append("muscleGroup", muscleGroup.toString());
    return apiClient.get<ExerciseSummary[]>(
      `/exercises/search?${params.toString()}`
    );
  },

  getById: (id: string) => apiClient.get<Exercise>(`/exercises/${id}`),

  getByType: (type: ExerciseType) =>
    apiClient.get<ExerciseSummary[]>(`/exercises/type/${type}`),

  getByMuscleGroup: (muscleGroup: MuscleGroup) =>
    apiClient.get<ExerciseSummary[]>(`/exercises/muscle-group/${muscleGroup}`),

  create: (
    request: Omit<Exercise, "id" | "isSystemExercise">,
    userId?: string
  ) => {
    const params = userId ? `?userId=${userId}` : "";
    return apiClient.post<Exercise>(`/exercises${params}`, request);
  },

  update: (
    id: string,
    request: {
      name: string;
      description: string;
      instructions?: string;
      videoUrl?: string;
    }
  ) => apiClient.put<Exercise>(`/exercises/${id}`, request),

  delete: (id: string) => apiClient.delete(`/exercises/${id}`),
};
