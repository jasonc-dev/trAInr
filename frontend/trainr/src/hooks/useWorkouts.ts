/**
 * useWorkouts Hook
 * Manages workout sessions following the DDD WorkoutSession aggregate pattern
 */

import { useState, useCallback } from "react";
import { workoutApi, programmeApi } from "../services";
import {
  WorkoutDayResponse,
  WorkoutExerciseResponse,
  ProgrammeWeek,
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

/**
 * Helper to update a specific exercise in the current workout
 */
const updateExerciseInWorkout = (
  workout: WorkoutDayResponse,
  updatedExercise: WorkoutExerciseResponse,
): WorkoutDayResponse => {
  return {
    ...workout,
    exercises: workout.exercises.map((ex) =>
      ex.id === updatedExercise.id ? updatedExercise : ex,
    ),
  };
};

export const useWorkouts = () => {
  const [currentWorkout, setCurrentWorkout] =
    useState<WorkoutDayResponse | null>(null);
  const [workoutWeeks, setWorkoutWeeks] = useState<ProgrammeWeek[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadWorkout = useCallback(
    async (workoutDayId: string): Promise<WorkoutDayResponse> => {
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
    ): Promise<ProgrammeWeek> => {
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
    ): Promise<ProgrammeWeek> => {
      try {
        setLoading(true);
        setError(null);
        const response = await workoutApi.updateWorkoutDay(id, request);
        // Extract the updated workout day from the returned week
        const updatedDay = response.data.workoutDays.find((d) => d.id === id);
        if (currentWorkout?.id === id && updatedDay) {
          setCurrentWorkout(updatedDay);
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
    async (id: string): Promise<ProgrammeWeek> => {
      try {
        setLoading(true);
        setError(null);
        const response = await workoutApi.deleteWorkoutDay(id);
        if (currentWorkout?.id === id) {
          setCurrentWorkout(null);
        }
        return response.data;
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
    ): Promise<ProgrammeWeek> => {
      try {
        setLoading(true);
        setError(null);
        const response = await workoutApi.completeWorkout(workoutDayId, {
          completedAt: completedAt.toISOString(),
        });
        // Extract the updated workout day from the returned week
        const updatedDay = response.data.workoutDays.find(
          (d) => d.id === workoutDayId,
        );
        if (currentWorkout?.id === workoutDayId && updatedDay) {
          setCurrentWorkout(updatedDay);
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
    ): Promise<WorkoutDayResponse> => {
      try {
        const response = await workoutApi.addExercise(workoutDayId, request);
        if (currentWorkout?.id === workoutDayId) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to add exercise";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout?.id],
  );

  const updateExercise = useCallback(
    async (
      exerciseId: number,
      request: UpdateWorkoutExerciseRequest,
    ): Promise<WorkoutDayResponse> => {
      try {
        const response = await workoutApi.updateExercise(exerciseId, request);
        if (currentWorkout) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update exercise";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const removeExercise = useCallback(
    async (exerciseId: string): Promise<WorkoutDayResponse> => {
      try {
        const response = await workoutApi.removeExercise(exerciseId);
        if (currentWorkout) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to remove exercise";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const reorderExercises = useCallback(
    async (
      workoutDayId: string,
      exerciseIds: string[],
    ): Promise<WorkoutDayResponse> => {
      try {
        const response = await workoutApi.reorderExercises(
          workoutDayId,
          exerciseIds,
        );
        if (currentWorkout?.id === workoutDayId) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to reorder exercises";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout?.id],
  );

  const addSet = useCallback(
    async (
      workoutExerciseId: string,
      request: CreateExerciseSetRequest,
    ): Promise<WorkoutExerciseResponse> => {
      try {
        const response = await workoutApi.addSet(workoutExerciseId, request);
        // Update current workout with returned exercise data
        if (currentWorkout) {
          setCurrentWorkout(
            updateExerciseInWorkout(currentWorkout, response.data),
          );
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to add set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const updateSet = useCallback(
    async (
      setId: string,
      request: UpdateExerciseSetRequest,
    ): Promise<WorkoutExerciseResponse> => {
      try {
        const response = await workoutApi.updateSet(setId, request);
        // Update current workout with returned exercise data
        if (currentWorkout) {
          setCurrentWorkout(
            updateExerciseInWorkout(currentWorkout, response.data),
          );
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const completeSet = useCallback(
    async (
      setId: string,
      request: CompleteSetRequest,
    ): Promise<WorkoutExerciseResponse> => {
      try {
        const response = await workoutApi.completeSet(setId, request);
        // Update current workout with returned exercise data
        if (currentWorkout) {
          setCurrentWorkout(
            updateExerciseInWorkout(currentWorkout, response.data),
          );
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to complete set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const deleteSet = useCallback(
    async (setId: string): Promise<WorkoutExerciseResponse> => {
      try {
        const response = await workoutApi.deleteSet(setId);
        // Update current workout with returned exercise data
        if (currentWorkout) {
          setCurrentWorkout(
            updateExerciseInWorkout(currentWorkout, response.data),
          );
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to delete set";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const groupSuperset = useCallback(
    async (
      workoutDayId: string,
      request: GroupSupersetRequest,
    ): Promise<WorkoutDayResponse> => {
      try {
        const response = await workoutApi.groupSuperset(workoutDayId, request);
        if (currentWorkout) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message =
          err.response?.data || "Failed to group exercises as superset";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const ungroupSuperset = useCallback(
    async (supersetGroupId: string): Promise<WorkoutDayResponse> => {
      try {
        const response = await workoutApi.ungroupSuperset(supersetGroupId);
        if (currentWorkout) {
          setCurrentWorkout(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to ungroup superset";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
  );

  const createDropSetSequence = useCallback(
    async (
      workoutExerciseId: number,
      request: CreateDropSetRequest,
    ): Promise<WorkoutExerciseResponse> => {
      try {
        const response = await workoutApi.createDropSetSequence(
          workoutExerciseId,
          request,
        );
        if (currentWorkout) {
          setCurrentWorkout(
            updateExerciseInWorkout(currentWorkout, response.data),
          );
        }
        return response.data;
      } catch (err: any) {
        const message =
          err.response?.data || "Failed to create drop set sequence";
        setError(message);
        throw new Error(message);
      }
    },
    [currentWorkout],
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
