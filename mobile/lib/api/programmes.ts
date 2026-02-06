/**
 * Programmes API Service
 * Handles assigned programmes endpoints
 */

import { apiClient } from "./client";
import type {
  Programme,
  ProgrammeSummary,
  ProgrammeWeek,
  CreateProgrammeRequest,
  UpdateProgrammeRequest,
  CreateProgrammeWeekRequest,
  CloneProgrammeRequest,
} from "../types/programme";

export const programmesApi = {
  /**
   * Get all programmes assigned to an athlete
   */
  async getAssignedProgrammes(athleteId: string): Promise<ProgrammeSummary[]> {
    const response = await apiClient.get<ProgrammeSummary[]>(
      `/AssignedProgramme/athlete/${athleteId}`,
    );
    return response.data;
  },

  /**
   * Get the active programme for an athlete
   */
  async getActiveProgramme(
    athleteId: string,
  ): Promise<ProgrammeSummary | null> {
    try {
      const response = await apiClient.get<ProgrammeSummary>(
        `/AssignedProgramme/athlete/${athleteId}/active`,
      );
      return response.data;
    } catch (error: unknown) {
      const axiosError = error as { response?: { status?: number } };
      if (axiosError.response?.status === 404) {
        return null;
      }
      throw error;
    }
  },

  /**
   * Get programme details by ID
   */
  async getById(id: string): Promise<Programme> {
    const response = await apiClient.get<Programme>(`/AssignedProgramme/${id}`);
    return response.data;
  },

  /**
   * Update programme
   */
  async updateProgramme(
    id: string,
    data: UpdateProgrammeRequest,
  ): Promise<Programme> {
    const response = await apiClient.put<Programme>(
      `/AssignedProgramme/${id}`,
      data,
    );
    return response.data;
  },

  /**
   * Delete programme
   */
  async deleteProgramme(id: string): Promise<void> {
    await apiClient.delete(`/AssignedProgramme/${id}`);
  },

  /**
   * Create new programme
   */
  async createProgramme(
    athleteId: string,
    data: CreateProgrammeRequest,
  ): Promise<Programme> {
    const response = await apiClient.post<Programme>(
      `/AssignedProgramme/athlete/${athleteId}`,
      data,
    );
    return response.data;
  },

  /**
   * Clone programme from template
   */
  async cloneProgramme(
    programmeId: string,
    data: CloneProgrammeRequest,
  ): Promise<Programme> {
    const response = await apiClient.post<Programme>(
      `/AssignedProgramme/${programmeId}/clone`,
      data,
    );
    return response.data;
  },

  /**
   * Add week to programme
   */
  async addWeek(
    programmeId: string,
    data: CreateProgrammeWeekRequest,
  ): Promise<ProgrammeWeek> {
    const response = await apiClient.post<ProgrammeWeek>(
      `/AssignedProgramme/${programmeId}/weeks`,
      data,
    );
    return response.data;
  },

  /**
   * Copy week content from one week to another
   */
  async copyWeekContent(
    sourceWeekId: string,
    targetWeekId: string,
  ): Promise<ProgrammeWeek> {
    const response = await apiClient.post<ProgrammeWeek>(
      `/AssignedProgramme/weeks/${sourceWeekId}/copy-to/${targetWeekId}`,
    );
    return response.data;
  },

  /**
   * Get pre-made programme templates
   */
  async getPreMadeProgrammes(): Promise<ProgrammeSummary[]> {
    const response = await apiClient.get<ProgrammeSummary[]>(
      `/AssignedProgramme/premade`,
    );
    return response.data;
  },

  /**
   * Get programmes created by athlete (their templates)
   */
  async getCreatedProgrammes(athleteId: string): Promise<ProgrammeSummary[]> {
    const response = await apiClient.get<ProgrammeSummary[]>(
      `/AssignedProgramme/athlete/${athleteId}/created`,
    );
    return response.data;
  },
};
