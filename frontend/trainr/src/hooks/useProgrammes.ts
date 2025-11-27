import { useState, useEffect, useCallback } from 'react';
import { Programme, ProgrammeSummary, CreateProgrammeRequest } from '../types';
import { programmesApi } from '../api';

export const useProgrammes = (userId: string | undefined) => {
  const [programmes, setProgrammes] = useState<ProgrammeSummary[]>([]);
  const [activeProgramme, setActiveProgramme] = useState<Programme | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadProgrammes = useCallback(async () => {
    if (!userId) return;

    try {
      setLoading(true);
      const response = await programmesApi.getByUser(userId);
      setProgrammes(response.data);
      setError(null);
    } catch (err) {
      console.error('Failed to load programmes:', err);
      setError('Failed to load programmes');
    } finally {
      setLoading(false);
    }
  }, [userId]);

  const loadActiveProgramme = useCallback(async () => {
    if (!userId) return;

    try {
      const summaryResponse = await programmesApi.getActiveByUser(userId);
      if (summaryResponse.data) {
        const fullResponse = await programmesApi.getById(summaryResponse.data.id);
        setActiveProgramme(fullResponse.data);
      }
    } catch (err) {
      // No active programme is not an error
      console.log('No active programme found');
    }
  }, [userId]);

  useEffect(() => {
    loadProgrammes();
    loadActiveProgramme();
  }, [loadProgrammes, loadActiveProgramme]);

  const createProgramme = async (request: CreateProgrammeRequest): Promise<Programme> => {
    if (!userId) throw new Error('User not authenticated');

    try {
      setLoading(true);
      const response = await programmesApi.create(userId, request);
      await loadProgrammes();
      setActiveProgramme(response.data);
      return response.data;
    } catch (err: any) {
      const message = err.response?.data || 'Failed to create programme';
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  const getProgramme = async (id: string): Promise<Programme> => {
    const response = await programmesApi.getById(id);
    return response.data;
  };

  const deleteProgramme = async (id: string): Promise<void> => {
    await programmesApi.delete(id);
    await loadProgrammes();
    if (activeProgramme?.id === id) {
      setActiveProgramme(null);
    }
  };

  return {
    programmes,
    activeProgramme,
    loading,
    error,
    createProgramme,
    getProgramme,
    deleteProgramme,
    refresh: loadProgrammes,
  };
};

