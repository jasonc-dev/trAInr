/**
 * Application Constants
 * Shared constants used across the application
 */

import { FitnessLevel, FitnessGoal, ExerciseType, MuscleGroup, Intensity } from "../types";

// Day names for display
export const DAY_NAMES = [
  "Sunday",
  "Monday",
  "Tuesday",
  "Wednesday",
  "Thursday",
  "Friday",
  "Saturday",
];

// Programme duration options
export const DURATION_OPTIONS = [
  { value: "4", label: "4 weeks" },
  { value: "6", label: "6 weeks" },
  { value: "8", label: "8 weeks" },
  { value: "10", label: "10 weeks" },
  { value: "12", label: "12 weeks" },
  { value: "16", label: "16 weeks" },
] as const;

// Workout days per week options
export const WORKOUT_DAYS_OPTIONS = [
  { value: "2", label: "2 days per week" },
  { value: "3", label: "3 days per week" },
  { value: "4", label: "4 days per week" },
  { value: "5", label: "5 days per week" },
  { value: "6", label: "6 days per week" },
] as const;

// Alias for backwards compatibility
export const DAYS_PER_WEEK_OPTIONS = WORKOUT_DAYS_OPTIONS;

// Difficulty options for sets
export const DIFFICULTY_OPTIONS = [
  { value: "", label: "Select..." },
  { value: "0", label: "Very Easy" },
  { value: "1", label: "Easy" },
  { value: "2", label: "Moderate" },
  { value: "3", label: "Hard" },
  { value: "4", label: "Very Hard" },
] as const;

// Intensity (RPE) options
export const INTENSITY_OPTIONS = [
  { value: "", label: "RPE" },
  { value: Intensity.RPE10.toString(), label: "RPE 10" },
  { value: Intensity.RPE9.toString(), label: "RPE 9" },
  { value: Intensity.RPE8.toString(), label: "RPE 8" },
  { value: Intensity.RPE7.toString(), label: "RPE 7" },
  { value: Intensity.RPE6.toString(), label: "RPE 6" },
  { value: Intensity.RPE5.toString(), label: "RPE 5" },
  { value: Intensity.RPE4.toString(), label: "RPE 4" },
  { value: Intensity.RPE3.toString(), label: "RPE 3" },
  { value: Intensity.RPE2.toString(), label: "RPE 2" },
  { value: Intensity.RPE1.toString(), label: "RPE 1" },
];

// Fitness level options for registration
export const FITNESS_LEVEL_OPTIONS = [
  {
    value: FitnessLevel.Beginner,
    label: "Beginner",
    icon: "🌱",
    description: "New to fitness or returning after a break",
  },
  {
    value: FitnessLevel.Intermediate,
    label: "Intermediate",
    icon: "💪",
    description: "1-3 years of consistent training",
  },
  {
    value: FitnessLevel.Advanced,
    label: "Advanced",
    icon: "🔥",
    description: "3+ years with solid foundation",
  },
  {
    value: FitnessLevel.Elite,
    label: "Elite",
    icon: "⚡",
    description: "Competitive or professional athlete",
  },
] as const;

// Fitness goal options for registration
export const FITNESS_GOAL_OPTIONS = [
  {
    value: FitnessGoal.BuildMuscle,
    label: "Build Muscle",
    icon: "🏋️",
    description: "Increase size and strength",
  },
  {
    value: FitnessGoal.LoseWeight,
    label: "Lose Weight",
    icon: "🎯",
    description: "Reduce body fat percentage",
  },
  {
    value: FitnessGoal.ImproveEndurance,
    label: "Improve Endurance",
    icon: "🏃",
    description: "Better stamina and cardio",
  },
  {
    value: FitnessGoal.IncreaseStrength,
    label: "Increase Strength",
    icon: "💎",
    description: "Lift heavier weights",
  },
  {
    value: FitnessGoal.GeneralFitness,
    label: "General Fitness",
    icon: "✨",
    description: "Overall health and wellness",
  },
] as const;

// Exercise type options and helpers
export const EXERCISE_TYPES = [
  { value: null, label: "All Types" },
  { value: ExerciseType.WeightTraining, label: "Weight Training" },
  { value: ExerciseType.Bodyweight, label: "Bodyweight" },
  { value: ExerciseType.Cardio, label: "Cardio" },
  { value: ExerciseType.Flexibility, label: "Flexibility" },
];

// Alias for backwards compatibility
export const EXERCISE_TYPE_OPTIONS = EXERCISE_TYPES;

// Muscle group options
export const MUSCLE_GROUPS = [
  { value: MuscleGroup.Chest, label: "Chest" },
  { value: MuscleGroup.Back, label: "Back" },
  { value: MuscleGroup.Shoulders, label: "Shoulders" },
  { value: MuscleGroup.Biceps, label: "Biceps" },
  { value: MuscleGroup.Triceps, label: "Triceps" },
  { value: MuscleGroup.Forearms, label: "Forearms" },
  { value: MuscleGroup.Quadriceps, label: "Quadriceps" },
  { value: MuscleGroup.Hamstrings, label: "Hamstrings" },
  { value: MuscleGroup.Glutes, label: "Glutes" },
  { value: MuscleGroup.Calves, label: "Calves" },
  { value: MuscleGroup.Core, label: "Core" },
  { value: MuscleGroup.FullBody, label: "Full Body" },
] as const;

// Alias for backwards compatibility
export const MUSCLE_GROUP_OPTIONS = MUSCLE_GROUPS;

// Helper functions
export const getExerciseTypeLabel = (type: ExerciseType): string => {
  return EXERCISE_TYPES.find((t) => t.value === type)?.label || "";
};

export const getMuscleGroupLabel = (group: MuscleGroup): string => {
  return MUSCLE_GROUPS.find((g) => g.value === group)?.label || "";
};

export const getExerciseIcon = (type: ExerciseType) => {
  switch (type) {
    case ExerciseType.WeightTraining:
      return "🏋️";
    case ExerciseType.Bodyweight:
      return "💪";
    case ExerciseType.Cardio:
      return "🏃";
    case ExerciseType.Flexibility:
      return "🧘";
    default:
      return "🏋️";
  }
};
