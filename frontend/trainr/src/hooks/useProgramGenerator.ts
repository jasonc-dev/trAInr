/**
 * useProgramGenerator Hook
 * Manages AI program generation
 */

import { useState, useCallback } from "react";
import { programGeneratorApi, GenerateProgramRequest, ProgramTemplateResponse } from "../services/api/programGeneratorApi";

export const useProgramGenerator = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const generateProgram = useCallback(
    async (request: GenerateProgramRequest): Promise<ProgramTemplateResponse> => {
      try {
        setLoading(true);
        setError(null);
        const response = await programGeneratorApi.generateProgram(request);
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to generate program";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    loading,
    error,
    generateProgram,
    clearError,
  };
};
