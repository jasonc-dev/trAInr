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
  createdBy: string;
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

export interface JobResponse {
  jobId: string;
  status: string;
  createdAt: string;
}

export interface JobStatusResponse {
  jobId: string;
  status: string; // "Pending" | "Processing" | "Completed" | "Failed"
  result?: ProgramTemplateResponse;
  errorMessage?: string;
  createdAt: string;
  completedAt?: string;
}

export const programGeneratorApi = {
  generateProgram: (request: GenerateProgramRequest) =>
    apiClient.post<JobResponse>("/programgenerator", request),

  getJobStatus: (jobId: string) =>
    apiClient.get<JobStatusResponse>(`/programgenerator/jobs/${jobId}`),
};
