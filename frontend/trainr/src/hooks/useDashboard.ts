/**
 * useDashboard Hook
 * Manages dashboard analytics and metrics
 */

import { useState, useEffect, useCallback } from "react";
import {
  Dashboard,
  WeeklyMetrics,
  ExerciseMetrics,
  VolumeComparison,
  IntensityTrend,
} from "../types";
import { dashboardApi } from "../services";

export const useDashboard = (athleteId: string | undefined) => {
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadDashboard = useCallback(async () => {
    if (!athleteId) return;

    try {
      setLoading(true);
      const response = await dashboardApi.getDashboard(athleteId);
      setDashboard(response.data);
      setError(null);
    } catch (err) {
      console.error("Failed to load dashboard:", err);
      setError("Failed to load dashboard");
    } finally {
      setLoading(false);
    }
  }, [athleteId]);

  useEffect(() => {
    loadDashboard();
  }, [loadDashboard]);

  const getWeeklyProgress = async (
    programmeId: string
  ): Promise<WeeklyMetrics[]> => {
    const response = await dashboardApi.getWeeklyProgress(programmeId);
    return response.data;
  };

  const getExerciseMetrics = async (
    exerciseId?: string
  ): Promise<ExerciseMetrics[]> => {
    if (!athleteId) throw new Error("Athlete not authenticated");
    const response = await dashboardApi.getExerciseMetrics(
      athleteId,
      exerciseId
    );
    return response.data;
  };

  const getVolumeComparison = async (
    programmeId: string
  ): Promise<VolumeComparison[]> => {
    const response = await dashboardApi.getVolumeComparison(programmeId);
    return response.data;
  };

  const getIntensityTrends = async (
    programmeId: string
  ): Promise<IntensityTrend[]> => {
    const response = await dashboardApi.getIntensityTrends(programmeId);
    return response.data;
  };

  return {
    dashboard,
    loading,
    error,
    getWeeklyProgress,
    getExerciseMetrics,
    getVolumeComparison,
    getIntensityTrends,
    refresh: loadDashboard,
  };
};
