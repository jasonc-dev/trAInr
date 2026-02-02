/**
 * Auth API Service
 * Handles authentication endpoints
 */

import { apiClient } from "./client";
import { secureStorage } from "../storage/secureStorage";

export interface LoginRequest {
  username: string;
  password: string;
  deviceInfo?: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  fitnessLevel: number;
  primaryGoal: number;
  workoutDaysPerWeek: number;
  deviceInfo?: string;
}

export interface AuthResponse {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  refreshTokenExpiresAt: string;
}

export interface StoredUser {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  fitnessLevel?: number;
  primaryGoal?: number;
  workoutDaysPerWeek?: number;
}

export const authApi = {
  async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>("/auth/login", request);

    // Store tokens and user
    await secureStorage.setAccessToken(response.data.accessToken);
    await secureStorage.setRefreshToken(response.data.refreshToken);
    await secureStorage.setUser<StoredUser>({
      id: response.data.id,
      username: response.data.username,
      email: response.data.email,
      firstName: response.data.firstName,
      lastName: response.data.lastName,
    });

    return response.data;
  },

  async register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>(
      "/auth/register",
      request
    );

    // Store tokens and user
    await secureStorage.setAccessToken(response.data.accessToken);
    await secureStorage.setRefreshToken(response.data.refreshToken);
    await secureStorage.setUser<StoredUser>({
      id: response.data.id,
      username: response.data.username,
      email: response.data.email,
      firstName: response.data.firstName,
      lastName: response.data.lastName,
    });

    return response.data;
  },

  async logout(): Promise<void> {
    try {
      const refreshToken = await secureStorage.getRefreshToken();
      if (refreshToken) {
        await apiClient.post("/auth/revoke", { refreshToken });
      }
    } catch {
      // Ignore errors during logout
    }

    await secureStorage.clearAll();
  },

  async checkUsername(username: string): Promise<boolean> {
    const response = await apiClient.get<{ available: boolean }>(
      `/auth/check-username/${username}`
    );
    return response.data.available;
  },
};
