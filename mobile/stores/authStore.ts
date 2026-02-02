/**
 * Auth Store
 * Zustand store for authentication state
 */

import { create } from "zustand";
import { secureStorage } from "../lib/storage/secureStorage";
import {
  authApi,
  LoginRequest,
  RegisterRequest,
  StoredUser,
} from "../lib/api/auth";

interface AuthState {
  user: StoredUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;

  // Actions
  initialize: () => Promise<void>;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  clearError: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: false,
  isLoading: true,
  error: null,

  initialize: async () => {
    try {
      const token = await secureStorage.getAccessToken();
      const user = await secureStorage.getUser<StoredUser>();

      set({
        user,
        isAuthenticated: !!token && !!user,
        isLoading: false,
      });
    } catch {
      set({ isLoading: false });
    }
  },

  login: async (request: LoginRequest) => {
    set({ isLoading: true, error: null });
    try {
      const response = await authApi.login(request);
      const user = await secureStorage.getUser<StoredUser>();
      set({
        user,
        isAuthenticated: true,
        isLoading: false,
      });
    } catch (error) {
      set({
        error: error instanceof Error ? error.message : "Login failed",
        isLoading: false,
      });
      throw error;
    }
  },

  register: async (request: RegisterRequest) => {
    set({ isLoading: true, error: null });
    try {
      const response = await authApi.register(request);
      const user = await secureStorage.getUser<StoredUser>();
      set({
        user,
        isAuthenticated: true,
        isLoading: false,
      });
    } catch (error) {
      set({
        error: error instanceof Error ? error.message : "Registration failed",
        isLoading: false,
      });
      throw error;
    }
  },

  logout: async () => {
    try {
      await authApi.logout();
    } catch {
      // Ignore errors
    }
    set({ user: null, isAuthenticated: false });
  },

  clearError: () => set({ error: null }),
}));
