/**
 * Workout Types
 * Types related to workout days, exercises, and sets
 */

import { DayOfWeek, Difficulty, Intensity, SetType } from "./enums";

export interface WorkoutDay {
  id: string;
  weekId: string;
  name: string;
  description?: string;
  isRestDay: boolean;
  isCompleted: boolean;
  scheduledDate: Date;
  completedDate?: Date;
  exercises: WorkoutExercise[];
}

export interface WorkoutDaySummary {
  id: string;
  weekId: string;
  dayOfWeek: DayOfWeek;
  name: string;
  isRestDay: boolean;
  isCompleted: boolean;
  exerciseCount: number;
}

export interface CreateWorkoutDayRequest {
  scheduledDate: Date;
  name: string;
  description?: string;
  isRestDay?: boolean;
}

export interface UpdateWorkoutDayRequest {
  scheduledDate: Date;
  name: string;
  description?: string;
  isRestDay?: boolean;
}

export interface CompleteWorkoutRequest {
  completedAt: string;
  notes?: string;
}

export interface WorkoutExercise {
  id: string;
  workoutDayId: string;
  exerciseId: string;
  exerciseName: string;
  orderIndex: number;
  targetSets: number;
  targetReps: number;
  targetWeight?: number;
  restSeconds?: number;
  targetRpe?: number;
  notes?: string;
  supersetGroupId?: string;
  supersetRestSeconds?: number;
  sets: ExerciseSet[];
}

export interface WorkoutExerciseSummary {
  id: string;
  exerciseId: string;
  exerciseName: string;
  orderIndex: number;
  targetSets: number;
  targetReps: number;
  completedSets: number;
}

export interface AddWorkoutExerciseRequest {
  exerciseId: string;
  orderIndex: number;
  targetSets: number;
  targetReps: number;
  targetWeight?: number;
  restSeconds?: number;
  targetRpe?: number;
  notes?: string;
  supersetGroupId?: string;
  supersetRestSeconds?: number;
}

export interface UpdateWorkoutExerciseRequest {
  orderIndex?: number;
  targetSets?: number;
  targetReps?: number;
  targetWeight?: number;
  restSeconds?: number;
  targetRpe?: number;
  notes?: string;
  supersetGroupId?: string;
  supersetRestSeconds?: number;
}

export interface ExerciseSet {
  id: string;
  workoutExerciseId: string;
  setNumber: number;
  reps?: number;
  weight?: number;
  setType: SetType;
  dropPercentage?: number;
  isCompleted: boolean;
  completedAt?: string;
  difficulty?: Difficulty;
  intensity?: Intensity;
  notes?: string;
}

export interface CreateExerciseSetRequest {
  setNumber: number;
  reps?: number;
  weight?: number;
  setType?: SetType;
  dropPercentage?: number;
}

export interface UpdateExerciseSetRequest {
  reps?: number;
  weight?: number;
  setType?: SetType;
  dropPercentage?: number;
  difficulty?: Difficulty;
  intensity?: Intensity;
  notes?: string;
}

export interface CompleteSetRequest {
  reps?: number;
  weight?: number;
  difficulty?: Difficulty;
  intensity?: Intensity;
  notes?: string;
}

export interface GroupSupersetRequest {
  exerciseIds: string[];
  supersetRestSeconds?: number;
}

export interface CreateDropSetRequest {
  startingWeight: number;
  startingReps: number;
  numberOfDrops: number;
  dropPercentage: number;
  repsAdjustment: number;
}
