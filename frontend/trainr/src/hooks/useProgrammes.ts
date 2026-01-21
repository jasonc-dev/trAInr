/**
 * useProgrammes Hook
 * Manages programmes following the DDD AssignedProgram aggregate pattern
 */

import { useState, useEffect, useCallback } from "react";
import {
  Programme,
  ProgrammeSummary,
  ProgrammeWeek,
  CreateProgrammeRequest,
  UpdateProgrammeRequest,
  CreateProgrammeWeekRequest,
  UpdateProgrammeWeekRequest,
  CloneProgrammeRequest,
} from "../types";
import { programmeApi } from "../services";

export const useProgrammes = (athleteId: string | undefined) => {
  const [programmes, setProgrammes] = useState<ProgrammeSummary[]>([]);
  const [preMadeProgrammes, setPreMadeProgrammes] = useState<
    ProgrammeSummary[]
  >([]);
  const [activeProgramme, setActiveProgramme] = useState<Programme | null>(
    null
  );
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadProgrammes = useCallback(async () => {
    if (!athleteId) return;

    try {
      setLoading(true);
      const response = await programmeApi.getByAthlete(athleteId);
      setProgrammes(response.data);
      setError(null);
    } catch (err) {
      console.error("Failed to load programmes:", err);
      setError("Failed to load programmes");
    } finally {
      setLoading(false);
    }
  }, [athleteId]);

  const loadActiveProgramme = useCallback(async () => {
    if (!athleteId) return;

    try {
      const summaryResponse = await programmeApi.getActiveByAthlete(athleteId);
      if (summaryResponse.data) {
        const fullResponse = await programmeApi.getById(
          summaryResponse.data.id
        );
        setActiveProgramme(fullResponse.data);
      }
    } catch (err) {
      console.log("No active programme found");
      setActiveProgramme(null);
    }
  }, [athleteId]);

  const loadPreMadeProgrammes = useCallback(async () => {
    try {
      const response = await programmeApi.getPreMade();
      setPreMadeProgrammes(response.data);
    } catch (err) {
      console.error("Failed to load pre-made programmes:", err);
    }
  }, []);

  useEffect(() => {
    loadProgrammes();
    loadActiveProgramme();
    loadPreMadeProgrammes();
  }, [loadProgrammes, loadActiveProgramme, loadPreMadeProgrammes]);

  const getProgramme = useCallback(async (id: string): Promise<Programme> => {
    const response = await programmeApi.getById(id);
    return response.data;
  }, []);

  const createProgramme = useCallback(
    async (request: CreateProgrammeRequest): Promise<Programme> => {
      if (!athleteId) throw new Error("Athlete not authenticated");

      try {
        setLoading(true);
        const response = await programmeApi.create(athleteId, request);
        await loadProgrammes();
        setActiveProgramme(response.data);
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to create programme";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [athleteId, loadProgrammes]
  );

  const updateProgramme = useCallback(
    async (id: string, request: UpdateProgrammeRequest): Promise<Programme> => {
      try {
        setLoading(true);
        const response = await programmeApi.update(id, request);
        await loadProgrammes();
        if (activeProgramme?.id === id) {
          setActiveProgramme(response.data);
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update programme";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [loadProgrammes, activeProgramme?.id]
  );

  const deleteProgramme = useCallback(
    async (id: string): Promise<void> => {
      try {
        await programmeApi.delete(id);
        await loadProgrammes();
        if (activeProgramme?.id === id) {
          setActiveProgramme(null);
        }
      } catch (err: any) {
        const message = err.response?.data || "Failed to delete programme";
        setError(message);
        throw new Error(message);
      }
    },
    [loadProgrammes, activeProgramme?.id]
  );

  const cloneProgramme = useCallback(
    async (programmeId: string, request: CloneProgrammeRequest): Promise<Programme> => {
      if (!athleteId) throw new Error("Athlete not authenticated");

      try {
        setLoading(true);
        const response = await programmeApi.clone(programmeId, request);
        await loadProgrammes();
        setActiveProgramme(response.data);
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to clone programme";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [athleteId, loadProgrammes]
  );

  const addWeek = useCallback(
    async (
      programmeId: string,
      request: CreateProgrammeWeekRequest
    ): Promise<ProgrammeWeek> => {
      try {
        const response = await programmeApi.addWeek(programmeId, request);
        if (activeProgramme?.id === programmeId) {
          await loadActiveProgramme();
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to add week";
        setError(message);
        throw new Error(message);
      }
    },
    [activeProgramme?.id, loadActiveProgramme]
  );

  const updateWeek = useCallback(
    async (
      weekId: string,
      request: UpdateProgrammeWeekRequest
    ): Promise<ProgrammeWeek> => {
      try {
        const response = await programmeApi.updateWeek(weekId, request);
        if (activeProgramme) {
          await loadActiveProgramme();
        }
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update week";
        setError(message);
        throw new Error(message);
      }
    },
    [activeProgramme, loadActiveProgramme]
  );

  const completeWeek = useCallback(
    async (weekId: string): Promise<ProgrammeWeek> => {
      return updateWeek(weekId, { isCompleted: true });
    },
    [updateWeek]
  );

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    programmes,
    preMadeProgrammes,
    activeProgramme,
    loading,
    error,
    createProgramme,
    getProgramme,
    updateProgramme,
    deleteProgramme,
    cloneProgramme,
    addWeek,
    updateWeek,
    completeWeek,
    refresh: loadProgrammes,
    refreshActive: loadActiveProgramme,
    refreshPreMade: loadPreMadeProgrammes,
    clearError,
  };
};
