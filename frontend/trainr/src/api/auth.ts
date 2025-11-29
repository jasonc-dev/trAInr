import { AuthResponse, LoginRequest, RegisterRequest } from "../types";
import apiClient from "./client";

export const authApi = {
  login: (request: LoginRequest) => 
    apiClient.post<AuthResponse>("/auth/login", request),
  
  register: (request: RegisterRequest) => 
    apiClient.post<AuthResponse>("/auth/register", request),
  
  checkUsername: (username: string) =>
    apiClient.get<{ available: boolean }>(`/auth/check-username/${username}`),
};
