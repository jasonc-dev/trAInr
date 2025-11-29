// Enums matching backend
export enum FitnessLevel {
  Beginner = 1,
  Intermediate = 2,
  Advanced = 3,
  Elite = 4,
}

export enum FitnessGoal {
  BuildMuscle = 1,
  LoseWeight = 2,
  ImproveEndurance = 3,
  IncreaseStrength = 4,
  GeneralFitness = 5,
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

export enum Difficulty {
  VeryEasy = 1,
  Easy = 2,
  Moderate = 3,
  Hard = 4,
  VeryHard = 5,
  Maximum = 6,
}

export enum Intensity {
  Low = 1,
  Moderate = 2,
  High = 3,
  VeryHigh = 4,
  Maximum = 5,
}

// Auth types
export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  email: string;
  firstName: string;
  lastName: string;
  /** Date of birth in ISO format (YYYY-MM-DD) */
  dateOfBirth: string;
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
  workoutDaysPerWeek: number;
}

export interface AuthResponse {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  token: string;
  /** Token expiration in ISO 8601 format */
  expiresAt: string;
}

// User types
export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  /** Date of birth in ISO format (YYYY-MM-DD) */
  dateOfBirth: string;
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
  workoutDaysPerWeek: number;
  /** Created timestamp in ISO 8601 format */
  createdAt: string;
}

export interface CreateUserRequest {
  username: string;
  password: string;
  email: string;
  firstName: string;
  lastName: string;
  /** Date of birth in ISO format (YYYY-MM-DD) */
  dateOfBirth: string;
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
  workoutDaysPerWeek: number;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
  workoutDaysPerWeek: number;
}

// Programme types
export interface Programme {
  id: string;
  userId: string;
  name: string;
  description: string;
  durationWeeks: number;
  isPreMade: boolean;
  isActive: boolean;
  /** Start date in ISO format (YYYY-MM-DD) */
  startDate: string;
  /** End date in ISO format (YYYY-MM-DD), optional */
  endDate?: string;
  /** Created timestamp in ISO 8601 format */
  createdAt: string;
  weeks: ProgrammeWeek[];
}

export interface ProgrammeSummary {
  id: string;
  name: string;
  description: string;
  durationWeeks: number;
  isActive: boolean;
  /** Start date in ISO format (YYYY-MM-DD) */
  startDate: string;
  completedWeeks: number;
  progressPercentage: number;
}

export interface ProgrammeWeek {
  id: string;
  weekNumber: number;
  notes?: string;
  isCompleted: boolean;
  workoutDays: WorkoutDay[];
}

export interface CreateProgrammeRequest {
  name: string;
  description: string;
  durationWeeks: number;
  /** Start date in ISO format (YYYY-MM-DD) */
  startDate: string;
}

// Workout types
export interface WorkoutDay {
  id: string;
  programmeWeekId: string;
  dayOfWeek: number;
  name: string;
  description?: string;
  /** Scheduled date in ISO format (YYYY-MM-DD), optional */
  scheduledDate?: string;
  /** Completed timestamp in ISO 8601 format, optional */
  completedDate?: string;
  isCompleted: boolean;
  isRestDay: boolean;
  exercises: WorkoutExercise[];
}

export interface CreateWorkoutDayRequest {
  dayOfWeek: number;
  name: string;
  description?: string;
  /** Scheduled date in ISO format (YYYY-MM-DD), optional */
  scheduledDate?: string;
  isRestDay: boolean;
}

export interface WorkoutExercise {
  id: string;
  exerciseId: string;
  exerciseName: string;
  orderIndex: number;
  notes?: string;
  targetSets: number;
  targetReps: number;
  targetWeight?: number;
  targetDurationSeconds?: number;
  targetDistance?: number;
  sets: ExerciseSet[];
}

export interface AddWorkoutExerciseRequest {
  exerciseId: string;
  orderIndex: number;
  notes?: string;
  targetSets: number;
  targetReps: number;
  targetWeight?: number;
  targetDurationSeconds?: number;
  targetDistance?: number;
}

export interface ExerciseSet {
  id: string;
  setNumber: number;
  reps?: number;
  weight?: number;
  durationSeconds?: number;
  distance?: number;
  difficulty?: Difficulty;
  intensity?: Intensity;
  isCompleted: boolean;
  isWarmup: boolean;
  notes?: string;
  completedAt?: string;
}

export interface CompleteSetRequest {
  reps?: number;
  weight?: number;
  durationSeconds?: number;
  distance?: number;
  difficulty?: Difficulty;
  intensity?: Intensity;
  notes?: string;
}

// Exercise types
export interface Exercise {
  id: string;
  name: string;
  description: string;
  type: ExerciseType;
  primaryMuscleGroup: MuscleGroup;
  secondaryMuscleGroup?: MuscleGroup;
  instructions?: string;
  videoUrl?: string;
  isSystemExercise: boolean;
}

export interface ExerciseSummary {
  id: string;
  name: string;
  type: ExerciseType;
  primaryMuscleGroup: MuscleGroup;
}

// Dashboard types
export interface Dashboard {
  activeProgramme?: ProgrammeSummary;
  currentWeekMetrics: WeeklyMetrics;
  weeklyProgress: WeeklyMetrics[];
  topExercises: ExerciseMetrics[];
  overallStats: OverallStats;
}

export interface WeeklyMetrics {
  weekNumber: number;
  totalVolume: number;
  averageIntensity: number;
  workoutsCompleted: number;
  workoutsPlanned: number;
  totalSetsCompleted: number;
  totalReps: number;
  totalDuration: string;
}

export interface ExerciseMetrics {
  exerciseId: string;
  exerciseName: string;
  exerciseType: ExerciseType;
  totalVolume: number;
  maxWeight: number;
  totalSets: number;
  totalReps: number;
  averageReps: number;
  averageWeight: number;
  progressPoints: ExerciseProgressPoint[];
}

export interface ExerciseProgressPoint {
  date: string;
  weekNumber: number;
  volume: number;
  maxWeight: number;
  totalReps: number;
  averageIntensity: number;
}

export interface OverallStats {
  totalWorkoutsCompleted: number;
  totalSetsCompleted: number;
  totalRepsPerformed: number;
  totalVolumeLifted: number;
  totalTrainingTime: string;
  currentStreak: number;
  longestStreak: number;
}

export interface IntensityTrend {
  weekNumber: number;
  averageIntensity: number;
  averageDifficulty: number;
  trend: string;
}

export interface VolumeComparison {
  weekNumber: number;
  volume: number;
  percentageChange: number;
}
