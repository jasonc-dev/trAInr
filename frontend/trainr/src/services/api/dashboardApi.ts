/**
 * Dashboard API Service
 * Handles dashboard analytics and metrics operations
 */

import apiClient from "./client";
import {
  Dashboard,
  WeeklyMetrics,
  ExerciseMetrics,
  VolumeComparison,
  IntensityTrend,
} from "../../types";

export const dashboardApi = {
  getDashboard: (athleteId: string) =>
    apiClient.get<Dashboard>(`/dashboard/athlete/${athleteId}`),

  getWeeklyProgress: (programmeId: string) =>
    apiClient.get<WeeklyMetrics[]>(
      `/dashboard/programme/${programmeId}/weekly-progress`,
    ),

  getExerciseMetrics: (athleteId: string, exerciseId?: number) => {
    const params = exerciseId ? `?exerciseId=${exerciseId}` : "";
    return apiClient.get<ExerciseMetrics[]>(
      `/dashboard/athlete/${athleteId}/exercises${params}`,
    );
  },

  getVolumeComparison: (programmeId: string) =>
    apiClient.get<VolumeComparison[]>(
      `/dashboard/programme/${programmeId}/volume-comparison`,
    ),

  getIntensityTrends: (programmeId: string) =>
    apiClient.get<IntensityTrend[]>(
      `/dashboard/programme/${programmeId}/intensity-trends`,
    ),
};
