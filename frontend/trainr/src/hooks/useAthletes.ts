/**
 * useAthletes Hook
 * Manages athletes following the DDD Athlete aggregate pattern
 */

import { useState, useCallback } from "react";
import { athleteApi } from "../services";
import {
  User,
  UserSummary,
  CreateUserRequest,
  UpdateUserRequest,
} from "../types";

export const useAthletes = () => {
  const [athletes, setAthletes] = useState<UserSummary[]>([]);
  const [currentAthlete, setCurrentAthlete] = useState<User | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadAthletes = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await athleteApi.getAll();
      setAthletes(response.data);
    } catch (err: any) {
      const message = err.response?.data || "Failed to load athletes";
      setError(message);
      console.error("Failed to load athletes:", err);
    } finally {
      setLoading(false);
    }
  }, []);

  const getAthlete = useCallback(async (id: string): Promise<User> => {
    try {
      setLoading(true);
      setError(null);
      const response = await athleteApi.getById(id);
      setCurrentAthlete(response.data);
      return response.data;
    } catch (err: any) {
      const message = err.response?.data || "Failed to load athlete";
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  }, []);

  const getAthleteByEmail = useCallback(
    async (email: string): Promise<User> => {
      try {
        setLoading(true);
        setError(null);
        const response = await athleteApi.getByEmail(email);
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to load athlete";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const createAthlete = useCallback(
    async (request: CreateUserRequest): Promise<User> => {
      try {
        setLoading(true);
        setError(null);
        const response = await athleteApi.create(request);
        await loadAthletes();
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to create athlete";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [loadAthletes]
  );

  const updateAthlete = useCallback(
    async (id: string, request: UpdateUserRequest): Promise<User> => {
      try {
        setLoading(true);
        setError(null);
        const response = await athleteApi.update(id, request);
        if (currentAthlete?.id === id) {
          setCurrentAthlete(response.data);
        }
        await loadAthletes();
        return response.data;
      } catch (err: any) {
        const message = err.response?.data || "Failed to update athlete";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [currentAthlete?.id, loadAthletes]
  );

  const deleteAthlete = useCallback(
    async (id: string): Promise<void> => {
      try {
        setLoading(true);
        setError(null);
        await athleteApi.delete(id);
        if (currentAthlete?.id === id) {
          setCurrentAthlete(null);
        }
        await loadAthletes();
      } catch (err: any) {
        const message = err.response?.data || "Failed to delete athlete";
        setError(message);
        throw new Error(message);
      } finally {
        setLoading(false);
      }
    },
    [currentAthlete?.id, loadAthletes]
  );

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    athletes,
    currentAthlete,
    loading,
    error,
    loadAthletes,
    getAthlete,
    getAthleteByEmail,
    createAthlete,
    updateAthlete,
    deleteAthlete,
    clearError,
    refresh: loadAthletes,
  };
};
