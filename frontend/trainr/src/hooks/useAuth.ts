import { useState, useEffect, useCallback } from "react";
import { authApi } from "../api/auth";
import { setAuthToken, removeAuthToken, getAuthToken } from "../api/client";
import { AuthResponse, LoginRequest, RegisterRequest } from "../types";

const USER_STORAGE_KEY = "trainr_user";

interface StoredUser {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  expiresAt: string;
}

export const useAuth = () => {
  const [user, setUser] = useState<StoredUser | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  const logout = useCallback(() => {
    removeAuthToken();
    localStorage.removeItem(USER_STORAGE_KEY);
    setUser(null);
    setIsAuthenticated(false);
    setError(null);
  }, []);

  // Check for existing auth on mount
  useEffect(() => {
    const storedUser = localStorage.getItem(USER_STORAGE_KEY);
    const token = getAuthToken();

    if (storedUser && token) {
      try {
        const parsed = JSON.parse(storedUser) as StoredUser;
        const expiresAt = new Date(parsed.expiresAt);

        if (expiresAt > new Date()) {
          setUser(parsed);
          setIsAuthenticated(true);
        } else {
          // Token expired, clear storage
          logout();
        }
      } catch {
        logout();
      }
    }
  }, [logout]);

  const handleAuthSuccess = useCallback((response: AuthResponse) => {
    const storedUser: StoredUser = {
      id: response.id,
      username: response.username,
      email: response.email,
      firstName: response.firstName,
      lastName: response.lastName,
      expiresAt: response.expiresAt.toString(),
    };

    setAuthToken(response.token);
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(storedUser));
    setUser(storedUser);
    setIsAuthenticated(true);
    setError(null);
  }, []);

  const login = async (request: LoginRequest): Promise<AuthResponse> => {
    try {
      setLoading(true);
      setError(null);
      const response = await authApi.login(request);
      handleAuthSuccess(response.data);
      return response.data;
    } catch (err: any) {
      const message =
        err.response?.data?.message || "Invalid username or password";
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  const register = async (request: RegisterRequest): Promise<AuthResponse> => {
    try {
      setLoading(true);
      setError(null);
      const response = await authApi.register(request);
      handleAuthSuccess(response.data);
      return response.data;
    } catch (err: any) {
      const message = err.response?.data?.message || "Registration failed";
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  };

  const checkUsername = async (username: string): Promise<boolean> => {
    try {
      const response = await authApi.checkUsername(username);
      return response.data.available;
    } catch {
      return false;
    }
  };

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
    user,
    loading,
    error,
    isAuthenticated,
    login,
    register,
    checkUsername,
    logout,
    clearError,
  };
};
