/**
 * User Types
 * Types related to user/athlete data
 */

import { FitnessLevel, FitnessGoal, UserRole } from './enums';

export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
  workoutDaysPerWeek: number;
  role: UserRole;
  createdAt: string;
  updatedAt: string;
}

export interface UserSummary {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
}

export interface CreateUserRequest {
  username: string;
  password: string;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
  workoutDaysPerWeek: number;
}

export interface UpdateUserRequest {
  email?: string;
  firstName?: string;
  lastName?: string;
  dateOfBirth?: string;
  fitnessLevel?: FitnessLevel;
  primaryGoal?: FitnessGoal;
  workoutDaysPerWeek?: number;
}
