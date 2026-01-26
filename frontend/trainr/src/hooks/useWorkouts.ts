/**
 * useWorkouts Hook
 * Manages workout sessions following the DDD WorkoutSession aggregate pattern
 */

import { useState, useCallback } from "react";
import { workoutApi, programmeApi } from "../services";
import {
  WorkoutDay,
  WorkoutExercise,
  ProgrammeWeek,
  ExerciseSet,
  CreateWorkoutDayRequest,
  UpdateWorkoutDayRequest,
  AddWorkoutExerciseRequest,
  UpdateWorkoutExerciseRequest,
  CreateExerciseSetRequest,
  UpdateExerciseSetRequest,
  CompleteSetRequest,
  GroupSupersetRequest,
  CreateDropSetRequest,
} from "../types";

export const useWorkouts = () => {
  const [currentWorkout, setCurrentWorkout] = useState<WorkoutDay | null>(null);
  const [workoutWeeks, setWorkoutWeeks] = useState<ProgrammeWeek[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadWorkout = useCallback(
    async (workoutDayId: string): Promise<WorkoutDay> => {
      try {
        setLoading(true);
        setError(null);
        const response = await workoutApi.getWorkoutDay(workoutDayId);
        setCurrentWorkout(response.data);
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to load workout";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [],
  );

  const loadWorkoutWeeks = useCallback(
    async (programmeId: string): Promise<ProgrammeWeek[]> => {
      try {
        const response = await programmeApi.getById(programmeId);
        if (workoutWeeks) {
          setWorkoutWeeks(response.data.weeks);
        }
        return response.data.weeks;
      } catch (err: any) {
        const message = err.response?.data || "Failed to load workout weeks";
        setError(message);
        throw new Error(message);
      }
    },
    [workoutWeeks],
  );

  const createWorkoutDay = useCallback(
    async (
      weekId: string,
      request: CreateWorkoutDayRequest,
    ): Promise<WorkoutDay> => {
      try {
        setLoading(true);
        setError(null);
        const response = await workoutApi.createWorkoutDay(weekId, request);
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to create workout day";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [],
  );

  const updateWorkoutDay = useCallback(
    async (
      id: string,
      request: UpdateWorkoutDayRequest,
    ): Promise<WorkoutDay> => {
      try {
        setLoading(true);
        setError(null);
        const response = await workoutApi.updateWorkoutDay(id, request);
        if (currentWorkout?.id === id) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update workout day";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [currentWorkout?.id],
  );

  const deleteWorkoutDay = useCallback(
    async (id: string): Promise<void> => {
      try {
        setLoading(true);
        setError(null);
        await workoutApi.deleteWorkoutDay(id);
        if (currentWorkout?.id === id) {
          setCurrentWorkout(null);
        }
      } catch (err: any) {
        const message = err.response?.data || "Failed to delete workout day";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [currentWorkout?.id],
  );

  const completeWorkout = useCallback(
    async (
      workoutDayId: string,
      completedAt: Date = new Date(),
    ): Promise<WorkoutDay> => {
      try {
        setLoading(true);
        setError(null);
        const response = await workoutApi.completeWorkout(workoutDayId, {
          completedAt: completedAt.toISOString(),
        });
        if (currentWorkout?.id === workoutDayId) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to complete workout";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [currentWorkout?.id],
  );

  const addExercise = useCallback(
    async (
      workoutDayId: string,
      request: AddWorkoutExerciseRequest,
    ): Promise<WorkoutExercise> => {
      try {
        const response = await workoutApi.addExercise(workoutDayId, request);
        if (currentWorkout?.id === workoutDayId) {
          await loadWorkout(workoutDayId);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to add exercise";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout?.id, loadWorkout],
  );

  const updateExercise = useCallback(
    async (
      exerciseId: number,
      request: UpdateWorkoutExerciseRequest,
    ): Promise<WorkoutExercise> => {
      try {
        const response = await workoutApi.updateExercise(exerciseId, request);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update exercise";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const removeExercise = useCallback(
    async (exerciseId: number): Promise<void> => {
      try {
        await workoutApi.removeExercise(exerciseId);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
      } catch (err: any) {
        const message = err.response?.data || "Failed to remove exercise";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const reorderExercises = useCallback(
    async (workoutDayId: string, exerciseIds: number[]): Promise<void> => {
      try {
        await workoutApi.reorderExercises(workoutDayId, exerciseIds);
        if (currentWorkout?.id === workoutDayId) {
          await loadWorkout(workoutDayId);
        }
      } catch (err: any) {
        const message = err.response?.data || "Failed to reorder exercises";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout?.id, loadWorkout],
  );

  const addSet = useCallback(
    async (
      workoutExerciseId: number,
      request: CreateExerciseSetRequest,
    ): Promise<ExerciseSet> => {
      try {
        const response = await workoutApi.addSet(workoutExerciseId, request);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to add set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const updateSet = useCallback(
    async (
      setId: string,
      request: UpdateExerciseSetRequest,
    ): Promise<ExerciseSet> => {
      try {
        const response = await workoutApi.updateSet(setId, request);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const completeSet = useCallback(
    async (
      setId: string,
      request: CompleteSetRequest,
    ): Promise<ExerciseSet> => {
      try {
        const response = await workoutApi.completeSet(setId, request);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to complete set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const deleteSet = useCallback(
    async (setId: string): Promise<void> => {
      try {
        await workoutApi.deleteSet(setId);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
      } catch (err: any) {
        const message = err.response?.data || "Failed to delete set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const groupSuperset = useCallback(
    async (
      workoutDayId: string,
      request: GroupSupersetRequest,
    ): Promise<WorkoutExercise[]> => {
      try {
        const response = await workoutApi.groupSuperset(workoutDayId, request);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
        return response.data;
      } catch (err: any) {
        const message =
          err.response?.data || "Failed to group exercises as superset";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const ungroupSuperset = useCallback(
    async (supersetGroupId: string): Promise<void> => {
      try {
        await workoutApi.ungroupSuperset(supersetGroupId);
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
      } catch (err: any) {
        const message = err.response?.data || "Failed to ungroup superset";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  const createDropSetSequence = useCallback(
    async (
      workoutExerciseId: number,
      request: CreateDropSetRequest,
    ): Promise<ExerciseSet[]> => {
      try {
        const response = await workoutApi.createDropSetSequence(
          workoutExerciseId,
          request,
        );
        if (currentWorkout) {
          await loadWorkout(currentWorkout.id);
        }
        return response.data;
      } catch (err: any) {
        const message =
          err.response?.data || "Failed to create drop set sequence";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout, loadWorkout],
  );

  return {
    currentWorkout,
    loading,
    error,
    loadWorkout,
    loadWorkoutWeeks,
    createWorkoutDay,
    updateWorkoutDay,
    deleteWorkoutDay,
    completeWorkout,
    addExercise,
    updateExercise,
    removeExercise,
    reorderExercises,
    addSet,
    updateSet,
    completeSet,
    deleteSet,
    groupSuperset,
    ungroupSuperset,
    createDropSetSequence,
    clearError,
  };
};
