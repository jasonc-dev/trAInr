import { useState, useEffect, useCallback } from 'react';
import { User, CreateUserRequest } from '../types';
import { usersApi } from '../api';

const USER_STORAGE_KEY = 'trainr_user_id';

export const useUser = () => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadUser = useCallback(async () => {
    const storedUserId = localStorage.getItem(USER_STORAGE_KEY);
    if (!storedUserId) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      const response = await usersApi.getById(storedUserId);
      setUser(response.data);
      setError(null);
    } catch (err) {
      console.error('Failed to load user:', err);
      localStorage.removeItem(USER_STORAGE_KEY);
      setError('Failed to load user');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadUser();
  }, [loadUser]);

  const createUser = async (request: CreateUserRequest): Promise<User> => {
    try {
      setLoading(true);
      const response = await usersApi.create(request);
      setUser(response.data);
      localStorage.setItem(USER_STORAGE_KEY, response.data.id);
      setError(null);
      return response.data;
    } catch (err: any) {
      const message = err.response?.data || 'Failed to create user';
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem(USER_STORAGE_KEY);
    setUser(null);
  };

  return {
    user,
    loading,
    error,
    createUser,
    logout,
    isAuthenticated: !!user,
  };
};

