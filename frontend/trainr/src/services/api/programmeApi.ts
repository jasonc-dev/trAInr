/**
 * Programme API Service
 * Handles programme CRUD and week operations
 */

import apiClient from "./client";
import {
  Programme,
  ProgrammeSummary,
  ProgrammeWeek,
  CreateProgrammeRequest,
  UpdateProgrammeRequest,
  CreateProgrammeWeekRequest,
  UpdateProgrammeWeekRequest,
  CopyWeekRequest,
} from "../../types";

export const programmeApi = {
  // Programme operations
  getById: (id: string) => apiClient.get<Programme>(`/assignedprogramme/${id}`),

  getByAthlete: (athleteId: string) =>
    apiClient.get<ProgrammeSummary[]>(
      `/assignedprogramme/athlete/${athleteId}`
    ),

  getActiveByAthlete: (athleteId: string) =>
    apiClient.get<ProgrammeSummary>(
      `/assignedprogramme/athlete/${athleteId}/active`
    ),

  getPreMade: () =>
    apiClient.get<ProgrammeSummary[]>("/assignedprogramme/premade"),

  create: (athleteId: string, request: CreateProgrammeRequest) =>
    apiClient.post<Programme>(
      `/assignedprogramme/athlete/${athleteId}`,
      request
    ),

  update: (id: string, request: UpdateProgrammeRequest) =>
    apiClient.put<Programme>(`/assignedprogramme/${id}`, request),

  delete: (id: string) => apiClient.delete(`/assignedprogramme/${id}`),

  clone: (programmeId: string, athleteId: string) =>
    apiClient.post<Programme>(
      `/assignedprogramme/${programmeId}/clone/${athleteId}`
    ),

  // Week operations
  addWeek: (programmeId: string, request: CreateProgrammeWeekRequest) =>
    apiClient.post<ProgrammeWeek>(
      `/assignedprogramme/${programmeId}/weeks`,
      request
    ),

  updateWeek: (weekId: string, request: UpdateProgrammeWeekRequest) =>
    apiClient.put<ProgrammeWeek>(`/assignedprogramme/weeks/${weekId}`, request),

  copyWeek: (weekId: string, request: CopyWeekRequest) =>
    apiClient.post<ProgrammeWeek>(
      `/assignedprogramme/weeks/${weekId}/copy`,
      request
    ),

  copyWeekContent: (sourceWeekId: string, targetWeekId: string) =>
    apiClient.post<ProgrammeWeek>(
      `/assignedprogramme/weeks/${sourceWeekId}/copy-to/${targetWeekId}`
    ),
};
