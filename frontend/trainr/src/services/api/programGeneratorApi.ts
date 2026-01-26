/**
 * Program Generator API Service
 * Handles AI program generation operations
 */

import apiClient from "./client";

export interface GenerateProgramRequest {
  programName: string;
  description: string;
  durationWeeks: number;
  experienceLevel: number;
  workoutDayNames: string[];
}

export interface ProgramTemplateResponse {
  id: string;
  name: string;
  description: string;
  durationWeeks: number;
  experienceLevel: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  weeks: any[];
}

export const programGeneratorApi = {
  generateProgram: (request: GenerateProgramRequest) =>
    apiClient.post<ProgramTemplateResponse>("/programgenerator", request),
};
