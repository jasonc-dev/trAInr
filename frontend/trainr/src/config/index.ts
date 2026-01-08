/**
 * Application Configuration
 * Centralizes environment variables and app-wide configuration
 */

// API Configuration
export const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:8080/api';

// Storage Keys
export const STORAGE_KEYS = {
  user: 'trainr_user',
  authToken: 'trainr_auth_token',
} as const;

// Route Constants
export const ROUTES = {
  login: '/login',
  register: '/register',
  dashboard: '/dashboard',
  programmes: '/programmes',
  programmeDetail: '/programmes/:id',
  workout: '/workout',
  exercises: '/exercises',
} as const;

// App Configuration
export const APP_CONFIG = {
  appName: 'trAInr',
  defaultPageSize: 20,
  debounceMs: 300,
  toastDurationMs: 3000,
  minPasswordLength: 6,
} as const;

// Theme Configuration
export const THEME_CONFIG = {
  defaultTheme: 'dark' as const,
  storageKey: 'trainr_theme',
} as const;
