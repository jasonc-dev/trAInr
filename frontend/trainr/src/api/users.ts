import apiClient from "./client";
import { User, CreateUserRequest, UpdateUserRequest } from "../types";

export const usersApi = {
  getAll: () => apiClient.get<User[]>("/users"),

  getById: (id: string) => apiClient.get<User>(`/users/${id}`),

  getByEmail: (email: string) => apiClient.get<User>(`/users/email/${email}`),

  create: (request: CreateUserRequest) =>
    apiClient.post<User>("/users", request),

  update: (id: string, request: UpdateUserRequest) =>
    apiClient.put<User>(`/users/${id}`, request),

  delete: (id: string) => apiClient.delete(`/users/${id}`),
};
