/**
 * useUser Hook
 * Manages current user state
 */

import { useState, useEffect, useCallback } from 'react';
import { getAuthToken } from '../services';
import { StoredUser } from '../types';
import { STORAGE_KEYS } from '../config';

export const useUser = () => {
  const [user, setUser] = useState<StoredUser | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadUser = useCallback(() => {
    const token = getAuthToken();
    const storedUser = localStorage.getItem(STORAGE_KEYS.user);

    if (!token || !storedUser) {
      setUser(null);
      setLoading(false);
      return;
    }

    try {
      const parsed = JSON.parse(storedUser) as StoredUser;
      const expiresAt = new Date(parsed.expiresAt);

      if (expiresAt > new Date()) {
        setUser(parsed);
        setError(null);
      } else {
        localStorage.removeItem(STORAGE_KEYS.user);
        setUser(null);
      }
    } catch (err) {
      console.error('Failed to parse stored user:', err);
      localStorage.removeItem(STORAGE_KEYS.user);
      setUser(null);
      setError('Failed to load user');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadUser();
  }, [loadUser]);

  // Listen for storage changes (e.g., logout in another tab)
  useEffect(() => {
    const handleStorageChange = (e: StorageEvent) => {
      if (e.key === STORAGE_KEYS.user || e.key === STORAGE_KEYS.authToken) {
        loadUser();
      }
    };

    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, [loadUser]);

  return {
    user,
    loading,
    error,
    isAuthenticated: !!user,
    userId: user?.id ?? null,
  };
};
