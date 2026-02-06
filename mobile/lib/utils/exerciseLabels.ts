/**
 * Exercise enum label helpers
 * Maps backend ExerciseType and MuscleGroup (trAInr.Domain.Entities.Exercise) to display labels.
 */

import { ExerciseType, MuscleGroup } from "../types/programme";

const EXERCISE_TYPE_LABELS: Record<ExerciseType, string> = {
  [ExerciseType.WeightTraining]: "Weight Training",
  [ExerciseType.Cardio]: "Cardio",
  [ExerciseType.Bodyweight]: "Bodyweight",
  [ExerciseType.Flexibility]: "Flexibility",
};

const MUSCLE_GROUP_LABELS: Record<MuscleGroup, string> = {
  [MuscleGroup.Chest]: "Chest",
  [MuscleGroup.Back]: "Back",
  [MuscleGroup.Shoulders]: "Shoulders",
  [MuscleGroup.Biceps]: "Biceps",
  [MuscleGroup.Triceps]: "Triceps",
  [MuscleGroup.Forearms]: "Forearms",
  [MuscleGroup.Core]: "Core",
  [MuscleGroup.Quadriceps]: "Quadriceps",
  [MuscleGroup.Hamstrings]: "Hamstrings",
  [MuscleGroup.Glutes]: "Glutes",
  [MuscleGroup.Calves]: "Calves",
  [MuscleGroup.FullBody]: "Full Body",
  [MuscleGroup.Cardio]: "Cardio",
};

export function getExerciseTypeLabel(type: ExerciseType): string {
  return EXERCISE_TYPE_LABELS[type] ?? "Unknown";
}

export function getMuscleGroupLabel(group: MuscleGroup): string {
  return MUSCLE_GROUP_LABELS[group] ?? "Unknown";
}
