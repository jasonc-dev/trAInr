/**
 * API Services barrel export
 * Re-exports all API services and utilities
 */

export { default as apiClient, setAuthToken, removeAuthToken, getAuthToken } from "./client";
export { authApi } from "./authApi";
export { athleteApi } from "./athleteApi";
export { programmeApi } from "./programmeApi";
export { workoutApi } from "./workoutApi";
export { exerciseApi } from "./exerciseApi";
export { dashboardApi } from "./dashboardApi";

// Aliases for backwards compatibility
export { programmeApi as programmesApi } from "./programmeApi";
export { workoutApi as workoutsApi } from "./workoutApi";
export { exerciseApi as exercisesApi } from "./exerciseApi";
