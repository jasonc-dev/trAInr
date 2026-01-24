/**
 * Authentication Types
 * Types related to user authentication and authorization
 */

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
  dateOfBirth: string;  
  fitnessLevel: number | undefined;
  primaryGoal: number | undefined;
  workoutDaysPerWeek: number;
}

export interface AuthResponse {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  token: string;
  expiresAt: Date;
}

export interface StoredUser {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  expiresAt: string;
}

export interface CheckUsernameResponse {
  available: boolean;
}
