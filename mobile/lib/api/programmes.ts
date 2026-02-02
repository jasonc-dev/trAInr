/**
 * Programmes API Service
 * Handles assigned programmes endpoints
 */

import { apiClient } from "./client";

export interface ProgrammeSummary {
  id: string;
  name: string;
  description: string;
  durationWeeks: number;
  isActive: boolean;
  isPreMade: boolean;
  startDate: string;
  completedWeeks: number;
  progressPercentage: number;
}

export const programmesApi = {
  /**
   * Get all programmes assigned to an athlete
   */
  async getAssignedProgrammes(athleteId: string): Promise<ProgrammeSummary[]> {
    const response = await apiClient.get<ProgrammeSummary[]>(
      `/AssignedProgramme/athlete/${athleteId}`
    );
    return response.data;
  },

  /**
   * Get the active programme for an athlete
   */
  async getActiveProgramme(
    athleteId: string
  ): Promise<ProgrammeSummary | null> {
    try {
      const response = await apiClient.get<ProgrammeSummary>(
        `/AssignedProgramme/athlete/${athleteId}/active`
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
};
