/**
 * Programme Types
 * Types for training programmes, weeks, workout days, and exercises
 */

// Enums
export enum DayOfWeek {
  Sunday = 0,
  Monday = 1,
  Tuesday = 2,
  Wednesday = 3,
  Thursday = 4,
  Friday = 5,
  Saturday = 6,
}

export enum ExerciseType {
  Compound = 0,
  Isolation = 1,
  Cardio = 2,
  Flexibility = 3,
  Plyometric = 4,
  Strongman = 5,
  Olympic = 6,
}

export enum MuscleGroup {
  Chest = 0,
  Back = 1,
  Shoulders = 2,
  Biceps = 3,
  Triceps = 4,
  Forearms = 5,
  Abs = 6,
  Obliques = 7,
  Glutes = 8,
  Quadriceps = 9,
  Hamstrings = 10,
  Calves = 11,
  Traps = 12,
  Lats = 13,
  LowerBack = 14,
  MiddleBack = 15,
  Neck = 16,
  FullBody = 17,
}

export enum SetType {
  Normal = 0,
  Warmup = 1,
  DropSet = 2,
  Superset = 3,
  FailureSet = 4,
}

export enum Difficulty {
  VeryEasy = 0,
  Easy = 1,
  Medium = 2,
  Hard = 3,
  VeryHard = 4,
}

export enum Intensity {
  Low = 0,
  Moderate = 1,
  High = 2,
  VeryHigh = 3,
  Maximum = 4,
}

// Programme Types
export interface Programme {
  id: string;
  name: string;
  description?: string;
  athleteId: string;
  durationWeeks: number;
  startDate?: string;
  endDate?: string;
  isActive: boolean;
  isTemplate: boolean;
  weeks: ProgrammeWeek[];
  createdAt: string;
  updatedAt: string;
}

export interface ProgrammeSummary {
  id: string;
  name: string;
  description?: string;
  athleteId: string;
  durationWeeks: number;
  startDate?: string;
  endDate?: string;
  isActive: boolean;
  isTemplate: boolean;
  isPreMade: boolean;
  completedWeeks: number;
  progressPercentage: number;
}

export interface ProgrammeWeek {
  id: string;
  programmeId: string;
  weekNumber: number;
  weekStartDate: string;
  notes?: string;
  isCompleted: boolean;
  workoutDays: WorkoutDayResponse[];
}

export interface ProgrammeWeekSummary {
  id: string;
  programmeId: string;
  weekNumber: number;
  notes?: string;
  isCompleted: boolean;
  workoutDays: WorkoutDaySummary[];
}

// Workout Day Types
export interface WorkoutDayResponse {
  id: string;
  programmeWeekId: string;
  name: string;
  description?: string;
  scheduledDate: string;
  completedDate?: string;
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

// Exercise Types
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
  completedAt?: string;
}

// Exercise Definition Types
export interface ExerciseSummary {
  id: number;
  name: string;
  type: ExerciseType;
  primaryMuscleGroup: MuscleGroup;
  secondaryMuscleGroup?: MuscleGroup;
}

export interface ExerciseDefinition {
  id: number;
  name: string;
  description: string;
  instructions?: string;
  type: ExerciseType;
  primaryMuscleGroup: MuscleGroup;
  secondaryMuscleGroup?: MuscleGroup;
  equipment?: string;
  difficulty?: string;
  videoUrl?: string;
  isCustom: boolean;
  createdById?: string;
}

// Request Types
export interface CreateProgrammeRequest {
  name: string;
  description?: string;
  durationWeeks: number;
  startDate?: string;
}

export interface UpdateProgrammeRequest {
  name?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  isActive?: boolean;
}

export interface CreateProgrammeWeekRequest {
  weekNumber: number;
  notes?: string;
}

export interface UpdateProgrammeWeekRequest {
  notes?: string;
  isCompleted?: boolean;
}

export interface CopyWeekRequest {
  targetWeekNumber: number;
}

export interface CloneProgrammeRequest {
  athleteId: string;
  startDate: string;
}

export interface CreateWorkoutDayRequest {
  scheduledDate: string;
  name: string;
  description?: string;
  isRestDay?: boolean;
}

export interface UpdateWorkoutDayRequest {
  scheduledDate: string;
  name: string;
  description?: string;
  isRestDay?: boolean;
}

export interface CompleteWorkoutRequest {
  completedAt: string;
  notes?: string;
}

export interface AddWorkoutExerciseRequest {
  exerciseDefinitionId: number;
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
