/**
 * Dashboard Types
 * Types related to dashboard analytics and metrics
 */

import { ProgrammeSummary } from "./programme";

export interface Dashboard {
  athleteId: string;
  activeProgramme?: ProgrammeSummary;
  currentWeekMetrics?: WeeklyMetrics;
  weeklyProgress: WeeklyMetrics[];
  topExercises: ExerciseMetrics[];
  overallStats: OverallStats;
}

export interface WeeklyMetrics {
  weekNumber: number;
  weekId: string;
  workoutsPlanned: number;
  workoutsCompleted: number;
  totalSetsPlanned: number;
  totalSetsCompleted: number;
  totalVolume: number;
  totalReps: number;
  averageIntensity: number;
}

export interface ExerciseMetrics {
  exerciseId: number;
  exerciseName: string;
  totalSets: number;
  totalReps: number;
  totalVolume: number;
  maxWeight: number;
  lastPerformed: string;
  progressPoints: ExerciseProgressPoint[];
}

export interface ExerciseProgressPoint {
  date: string;
  weight: number;
  reps: number;
  volume: number;
}

export interface OverallStats {
  totalWorkoutsCompleted: number;
  totalSetsCompleted: number;
  totalVolumeLifted: number;
  totalTimeSpent: number;
  currentStreak: number;
  longestStreak: number;
}

export interface IntensityTrend {
  date: string;
  averageIntensity: number;
  workoutCount: number;
}

export interface VolumeComparison {
  muscleGroup: string;
  thisWeek: number;
  lastWeek: number;
  percentChange: number;
}
