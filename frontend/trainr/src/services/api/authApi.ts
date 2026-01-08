/**
 * Authentication API Service
 * Handles login, registration, and auth-related operations
 */

import apiClient from "./client";
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  CheckUsernameResponse,
} from "../../types";

export const authApi = {
  login: (request: LoginRequest) =>
    apiClient.post<AuthResponse>("/auth/login", request),

  register: (request: RegisterRequest) =>
    apiClient.post<AuthResponse>("/auth/register", request),

  checkUsername: (username: string) =>
    apiClient.get<CheckUsernameResponse>(`/auth/check-username/${username}`),
};
