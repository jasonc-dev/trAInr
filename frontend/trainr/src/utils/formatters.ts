/**
 * Formatting Utilities
 * Helper functions for formatting data
 */

import { DAY_NAMES } from "./constants";

// Re-export exercise helpers from constants for backwards compatibility
export {
  getExerciseTypeLabel,
  getMuscleGroupLabel,
  getExerciseIcon,
} from "./constants";

/**
 * Format duration in seconds to human-readable string
 */
export const formatDuration = (durationSeconds: number): string => {
  if (durationSeconds < 60) {
    return `${durationSeconds}s`;
  }
  const minutes = Math.floor(durationSeconds / 60);
  const seconds = durationSeconds % 60;
  if (seconds === 0) {
    return `${minutes}m`;
  }
  return `${minutes}m ${seconds}s`;
};

/**
 * Format weight with unit
 */
export const formatWeight = (weight: number, unit: string = "kg"): string => {
  return `${weight}${unit}`;
};

/**
 * Capitalize first letter of a string
 */
export const capitalizeFirstLetter = (str: string): string => {
  return str.charAt(0).toUpperCase() + str.slice(1);
};

/**
 * Get day name from day of week number
 */
export const getDayName = (dayOfWeek: number): string => {
  return DAY_NAMES[dayOfWeek] || "";
};

/**
 * Format date to locale string
 */
export const formatDate = (date: string | Date): string => {
  const d = typeof date === "string" ? new Date(date) : date;
  return d.toLocaleDateString();
};

/**
 * Format number with thousands separator
 */
export const formatNumber = (num: number): string => {
  return num.toLocaleString();
};

/**
 * Format percentage
 */
export const formatPercentage = (
  value: number,
  decimals: number = 0
): string => {
  return `${value.toFixed(decimals)}%`;
};

/**
 * Format rest time (seconds to mm:ss)
 */
export const formatRestTime = (seconds: number): string => {
  const mins = Math.floor(seconds / 60);
  const secs = seconds % 60;
  return `${mins}:${secs.toString().padStart(2, "0")}`;
};
