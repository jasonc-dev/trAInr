/**
 * Workout Types
 * Types related to workout days, exercises, and sets
 */

import { DayOfWeek, Difficulty, Intensity, SetType } from "./enums";

export interface WorkoutDayResponse {
  id: string;
  programmeWeekId: string;
  name: string;
  description?: string;
  scheduledDate: Date;
  completedDate?: Date;
  isCompleted: boolean;
  isRestDay: boolean;
  exercises: WorkoutExerciseResponse[];
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

export interface WorkoutExerciseResponse {
  id: string;
  exerciseDefinitionId: number;
  exerciseName: string;
  orderIndex: number;
  notes?: string;
  targetSets: number;
  targetReps: number;
  targetWeight?: number;
  targetDurationSeconds?: number;
  targetDistance?: number;
  restSeconds?: number;
  targetRpe?: number;
  supersetGroupId?: string;
  supersetRestSeconds?: number;
  sets: ExerciseSetResponse[];
}

export interface WorkoutExerciseSummary {
  id: string;
  exerciseId: number;
  exerciseName: string;
  orderIndex: number;
  targetSets: number;
  targetReps: number;
  completedSets: number;
}

export interface AddWorkoutExerciseRequest {
  exerciseId: number;
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

export interface ExerciseSetResponse {
  id: string;
  setNumber: number;
  reps?: number;
  weight?: number;
  durationSeconds?: number;
  distance?: number;
  difficulty?: Difficulty;
  intensity?: Intensity;
  isCompleted: boolean;
  setType: SetType;
  dropPercentage?: number;
  notes?: string;
  completedAt?: Date;
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
