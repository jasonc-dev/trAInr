/**
 * useProfile Hook
 * Manages user profile fetching and updating
 */

import { useState, useCallback } from 'react';
import { athleteApi } from '../services/api/athleteApi';
import { User, UpdateUserRequest, StoredUser } from '../types';
import { STORAGE_KEYS } from '../config';

export const useProfile = () => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const updateStoredUser = useCallback((userData: User) => {
    // Update the extended stored user data in localStorage
    const storedUser: StoredUser = {
      id: userData.id,
      username: userData.username,
      email: userData.email,
      firstName: userData.firstName,
      lastName: userData.lastName,
      fitnessLevel: userData.fitnessLevel,
      primaryGoal: userData.primaryGoal,
      workoutDaysPerWeek: userData.workoutDaysPerWeek,
      expiresAt: localStorage.getItem(STORAGE_KEYS.user) 
        ? JSON.parse(localStorage.getItem(STORAGE_KEYS.user)!).expiresAt 
        : new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
    };
    localStorage.setItem(STORAGE_KEYS.user, JSON.stringify(storedUser));
  }, []);

  const fetchProfile = useCallback(async (): Promise<User | null> => {
    try {
      setLoading(true);
      setError(null);
      const response = await athleteApi.getCurrentUser();
      setUser(response.data);
      updateStoredUser(response.data);
      return response.data;
    } catch (err: any) {
      const message = err.response?.data?.message || 'Failed to fetch profile';
      setError(message);
      return null;
    } finally {
      setLoading(false);
    }
  }, [updateStoredUser]);

  const updateProfile = useCallback(async (request: UpdateUserRequest): Promise<User | null> => {
    try {
      setLoading(true);
      setError(null);
      const response = await athleteApi.updateCurrentUser(request);
      setUser(response.data);
      updateStoredUser(response.data);
      return response.data;
    } catch (err: any) {
      const message = err.response?.data?.message || 'Failed to update profile';
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  }, [updateStoredUser]);

  const refreshProfile = useCallback(() => {
    return fetchProfile();
  }, [fetchProfile]);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    user,
    loading,
    error,
    fetchProfile,
    updateProfile,
    refreshProfile,
    clearError,
  };
};
