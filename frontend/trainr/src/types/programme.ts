/**
 * Programme Types
 * Types related to training programmes and weeks
 */

import { WorkoutDay, WorkoutDaySummary } from "./workout";

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
  workoutDays: WorkoutDay[];
}

export interface ProgrammeWeekSummary {
  id: string;
  programmeId: string;
  weekNumber: number;
  notes?: string;
  isCompleted: boolean;
  workoutDays: WorkoutDaySummary[];
}

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