import apiClient from "./client";
import {
  Dashboard,
  WeeklyMetrics,
  ExerciseMetrics,
  OverallStats,
  IntensityTrend,
  VolumeComparison,
} from "../types";

export const dashboardApi = {
  getDashboard: (userId: string) =>
    apiClient.get<Dashboard>(`/dashboard/user/${userId}`),

  getWeeklyProgress: (programmeId: string) =>
    apiClient.get<WeeklyMetrics[]>(
      `/dashboard/programme/${programmeId}/weekly-progress`
    ),

  getExerciseMetrics: (userId: string, exerciseId?: string) => {
    const params = exerciseId ? `?exerciseId=${exerciseId}` : "";
    return apiClient.get<ExerciseMetrics[]>(
      `/dashboard/user/${userId}/exercises${params}`
    );
  },

  getOverallStats: (userId: string) =>
    apiClient.get<OverallStats>(`/dashboard/user/${userId}/stats`),

  getIntensityTrends: (programmeId: string) =>
    apiClient.get<IntensityTrend[]>(
      `/dashboard/programme/${programmeId}/intensity-trends`
    ),

  getVolumeComparison: (programmeId: string) =>
    apiClient.get<VolumeComparison[]>(
      `/dashboard/programme/${programmeId}/volume-comparison`
    ),
};
