/**
 * Athlete API Service
 * Handles athlete/user CRUD operations
 */

import apiClient from "./client";
import {
  User,
  UserSummary,
  CreateUserRequest,
  UpdateUserRequest,
} from "../../types";

export const athleteApi = {
  getAll: () => apiClient.get<UserSummary[]>("/athlete"),

  getById: (id: string) => apiClient.get<User>(`/athlete/${id}`),

  getByEmail: (email: string) => apiClient.get<User>(`/athlete/email/${email}`),

  create: (request: CreateUserRequest) =>
    apiClient.post<User>("/athlete", request),

  update: (id: string, request: UpdateUserRequest) =>
    apiClient.put<User>(`/athlete/${id}`, request),

  delete: (id: string) => apiClient.delete(`/athlete/${id}`),
};
