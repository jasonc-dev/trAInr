/**
 * Exercise Types
 * Types related to exercise definitions
 */

import { ExerciseType, MuscleGroup } from "./enums";

export interface Exercise {
  id: string;
  name: string;
  description: string;
  instructions?: string;
  type: ExerciseType;
  primaryMuscleGroup: MuscleGroup;
  secondaryMuscleGroup?: MuscleGroup;
  videoUrl?: string;
  createdAt: string;
  updatedAt: string;
}

export interface ExerciseSummary {
  id: string;
  name: string;
  type: ExerciseType;
  primaryMuscleGroup: MuscleGroup;
  secondaryMuscleGroup?: MuscleGroup;
}

export interface CreateExerciseRequest {
  name: string;
  description: string;
  instructions?: string;
  type: ExerciseType;
  primaryMuscleGroup: MuscleGroup;
  secondaryMuscleGroup?: MuscleGroup;
  videoUrl?: string;
}

export interface UpdateExerciseRequest {
  name?: string;
  description?: string;
  instructions?: string;
  type?: ExerciseType;
  primaryMuscleGroup?: MuscleGroup;
  secondaryMuscleGroup?: MuscleGroup;
  videoUrl?: string;
}
