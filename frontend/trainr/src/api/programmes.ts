import apiClient from "./client";
import {
  Programme,
  ProgrammeSummary,
  CreateProgrammeRequest,
  ProgrammeWeek,
} from "../types";

export const programmesApi = {
  getPreMade: () => apiClient.get<ProgrammeSummary[]>("/programmes/premade"),

  getByUser: (userId: string) =>
    apiClient.get<ProgrammeSummary[]>(`/programmes/user/${userId}`),

  getActiveByUser: (userId: string) =>
    apiClient.get<ProgrammeSummary>(`/programmes/user/${userId}/active`),

  getById: (id: string) => apiClient.get<Programme>(`/programmes/${id}`),

  create: (userId: string, request: CreateProgrammeRequest) =>
    apiClient.post<Programme>(`/programmes/user/${userId}`, request),

  update: (
    id: string,
    request: { name: string; description: string; isActive: boolean }
  ) => apiClient.put<Programme>(`/programmes/${id}`, request),

  delete: (id: string) => apiClient.delete(`/programmes/${id}`),

  clone: (programmeId: string, userId: string) =>
    apiClient.post<Programme>(`/programmes/${programmeId}/clone/${userId}`),

  addWeek: (
    programmeId: string,
    request: { weekNumber: number; notes?: string }
  ) =>
    apiClient.post<ProgrammeWeek>(`/programmes/${programmeId}/weeks`, request),

  updateWeek: (
    weekId: string,
    request: { notes?: string; isCompleted: boolean }
  ) => apiClient.put<ProgrammeWeek>(`/programmes/weeks/${weekId}`, request),
};
