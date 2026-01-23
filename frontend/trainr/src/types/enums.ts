/**
 * Enums
 * All enumeration types used across the application
 */

export enum FitnessLevel {
  Beginner = 0,
  Intermediate = 1,
  Advanced = 2,
  Elite = 3,
}

export enum FitnessGoal {
  BuildMuscle = 0,
  LoseWeight = 1,
  ImproveEndurance = 2,
  IncreaseStrength = 3,
  GeneralFitness = 4,
}

export enum UserRole {
  Athlete = 0,
  Coach = 1,
  Admin = 2,
}

export enum ExerciseType {
  WeightTraining = 1,
  Cardio = 2,
  Bodyweight = 3,
  Flexibility = 4,
}

export enum MuscleGroup {
  Chest = 1,
  Back = 2,
  Shoulders = 3,
  Biceps = 4,
  Triceps = 5,
  Forearms = 6,
  Core = 7,
  Quadriceps = 8,
  Hamstrings = 9,
  Glutes = 10,
  Calves = 11,
  FullBody = 12,
  Cardio = 13,
}

export enum DayOfWeek {
  Sunday = 0,
  Monday = 1,
  Tuesday = 2,
  Wednesday = 3,
  Thursday = 4,
  Friday = 5,
  Saturday = 6,
}

export enum Difficulty {
  VeryEasy = 0,
  Easy = 1,
  Moderate = 2,
  Hard = 3,
  VeryHard = 4,
}

export enum Intensity {
  RPE1 = 1,
  RPE2 = 2,
  RPE3 = 3,
  RPE4 = 4,
  RPE5 = 5,
  RPE6 = 6,
  RPE7 = 7,
  RPE8 = 8,
  RPE9 = 9,
  RPE10 = 10,
}

export enum SetType {
  Normal = 0,
  Warmup = 1,
  DropSet = 2,
  Failure = 3,
  Amrap = 4,
}
