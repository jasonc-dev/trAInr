/**
 * useExercises Hook
 * Manages exercise definitions
 */

import { useState, useEffect, useCallback } from 'react';
import { Exercise, ExerciseSummary, ExerciseType, MuscleGroup } from '../types';
import { exerciseApi } from '../services';

export const useExercises = () => {
  const [exercises, setExercises] = useState<Exercise[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadExercises = useCallback(async () => {
    try {
      setLoading(true);
      const response = await exerciseApi.getAll();
      setExercises(response.data);
      setError(null);
    } catch (err) {
      console.error('Failed to load exercises:', err);
      setError('Failed to load exercises');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadExercises();
  }, [loadExercises]);

  const searchExercises = useCallback(
    async (
      query?: string,
      type?: ExerciseType,
      muscleGroup?: MuscleGroup
    ): Promise<ExerciseSummary[]> => {
      try {
        const response = await exerciseApi.search(query, type, muscleGroup);
        return response.data;
      } catch (err) {
        console.error('Failed to search exercises:', err);
        throw err;
      }
    },
    []
  );

  const getExercise = async (id: string): Promise<Exercise> => {
    const response = await exerciseApi.getById(id);
    return response.data;
  };

  const getByType = async (type: ExerciseType): Promise<ExerciseSummary[]> => {
    const response = await exerciseApi.getByType(type);
    return response.data;
  };

  const getByMuscleGroup = async (group: MuscleGroup): Promise<ExerciseSummary[]> => {
    const response = await exerciseApi.getByMuscleGroup(group);
    return response.data;
  };

  return {
    exercises,
    loading,
    error,
    searchExercises,
    getExercise,
    getByType,
    getByMuscleGroup,
    refresh: loadExercises,
  };
};
