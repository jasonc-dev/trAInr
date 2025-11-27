import { useState, useEffect, useCallback } from 'react';
import { Dashboard, WeeklyMetrics, ExerciseMetrics, VolumeComparison, IntensityTrend } from '../types';
import { dashboardApi } from '../api';

export const useDashboard = (userId: string | undefined) => {
  const [dashboard, setDashboard] = useState<Dashboard | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadDashboard = useCallback(async () => {
    if (!userId) return;

    try {
      setLoading(true);
      const response = await dashboardApi.getDashboard(userId);
      setDashboard(response.data);
      setError(null);
    } catch (err) {
      console.error('Failed to load dashboard:', err);
      setError('Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    loadDashboard();
  }, [loadDashboard]);

  const getWeeklyProgress = async (programmeId: string): Promise<WeeklyMetrics[]> => {
    const response = await dashboardApi.getWeeklyProgress(programmeId);
    return response.data;
  };

  const getExerciseMetrics = async (exerciseId?: string): Promise<ExerciseMetrics[]> => {
    if (!userId) throw new Error('User not authenticated');
    const response = await dashboardApi.getExerciseMetrics(userId, exerciseId);
    return response.data;
  };

  const getVolumeComparison = async (programmeId: string): Promise<VolumeComparison[]> => {
    const response = await dashboardApi.getVolumeComparison(programmeId);
    return response.data;
  };

  const getIntensityTrends = async (programmeId: string): Promise<IntensityTrend[]> => {
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

