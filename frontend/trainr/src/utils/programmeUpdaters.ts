/**
 * Utility functions for updating programme state with contextual data
 * from API responses, eliminating the need for additional API calls
 */

import {
  Programme,
  ProgrammeWeek,
  WorkoutDayResponse,
  WorkoutExerciseResponse,
} from "../types";

/**
 * Update a programme by replacing a specific week with updated data
 */
export const updateProgrammeWithWeek = (
  programme: Programme,
  updatedWeek: ProgrammeWeek,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) =>
      week.id === updatedWeek.id ? updatedWeek : week,
    ),
  };
};

/**
 * Update a programme by replacing a specific workout day with updated data
 */
export const updateProgrammeWithDay = (
  programme: Programme,
  updatedDay: WorkoutDayResponse,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) => ({
      ...week,
      workoutDays: week.workoutDays.map((day) =>
        day.id === updatedDay.id ? updatedDay : day,
      ),
    })),
  };
};

/**
 * Update a programme by replacing a specific exercise with updated data
 * This is used when set operations return the updated exercise
 */
export const updateProgrammeWithExercise = (
  programme: Programme,
  updatedExercise: WorkoutExerciseResponse,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) => ({
      ...week,
      workoutDays: week.workoutDays.map((day) => ({
        ...day,
        exercises: day.exercises.map((ex) =>
          ex.id === updatedExercise.id ? updatedExercise : ex,
        ),
      })),
    })),
  };
};

/**
 * Remove a workout day from a programme after deletion
 * (For backwards compatibility with delete operations that might not return data)
 */
export const removeDayFromProgramme = (
  programme: Programme,
  dayId: string,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) => ({
      ...week,
      workoutDays: week.workoutDays.filter((day) => day.id !== dayId),
    })),
  };
};

/**
 * Remove an exercise from a programme after deletion
 * (For backwards compatibility with delete operations that might not return data)
 */
export const removeExerciseFromProgramme = (
  programme: Programme,
  exerciseId: string,
): Programme => {
  return {
    ...programme,
    weeks: programme.weeks.map((week) => ({
      ...week,
      workoutDays: week.workoutDays.map((day) => ({
        ...day,
        exercises: day.exercises.filter((ex) => ex.id !== exerciseId),
      })),
    })),
  };
};
