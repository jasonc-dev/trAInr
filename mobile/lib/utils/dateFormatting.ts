/**
 * Date Formatting Utilities
 * Helpers for displaying dates in various formats
 */

/**
 * Format a date for workout display
 * Returns "Today", "Tomorrow", "Yesterday", or formatted date
 */
export function formatWorkoutDate(date: Date | string): string {
  const workoutDate = new Date(date);
  workoutDate.setHours(0, 0, 0, 0);

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const tomorrow = new Date(today);
  tomorrow.setDate(tomorrow.getDate() + 1);

  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);

  const diffTime = workoutDate.getTime() - today.getTime();
  const diffDays = Math.round(diffTime / (1000 * 60 * 60 * 24));

  if (workoutDate.getTime() === today.getTime()) {
    return "Today";
  } else if (workoutDate.getTime() === tomorrow.getTime()) {
    return "Tomorrow";
  } else if (workoutDate.getTime() === yesterday.getTime()) {
    return "Yesterday";
  } else if (diffDays > 0 && diffDays <= 7) {
    // Future dates within a week
    const dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    return dayNames[workoutDate.getDay()];
  } else if (diffDays < 0 && diffDays >= -7) {
    // Past dates within a week
    return `${Math.abs(diffDays)} days ago`;
  } else {
    // Format as "Mon, Feb 10" or "Feb 10, 2026" if different year
    const monthNames = [
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
      "Oct",
      "Nov",
      "Dec",
    ];
    const dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    const currentYear = today.getFullYear();
    const workoutYear = workoutDate.getFullYear();

    if (currentYear === workoutYear) {
      return `${dayNames[workoutDate.getDay()]}, ${
        monthNames[workoutDate.getMonth()]
      } ${workoutDate.getDate()}`;
    } else {
      return `${
        monthNames[workoutDate.getMonth()]
      } ${workoutDate.getDate()}, ${workoutYear}`;
    }
  }
}

/**
 * Get relative time description
 * Returns "3 days ago", "in 5 days", etc.
 */
export function getRelativeTime(date: Date | string): string {
  const targetDate = new Date(date);
  targetDate.setHours(0, 0, 0, 0);

  const today = new Date();
  today.setHours(0, 0, 0, 0);

  const diffTime = targetDate.getTime() - today.getTime();
  const diffDays = Math.round(diffTime / (1000 * 60 * 60 * 24));

  if (diffDays === 0) {
    return "today";
  } else if (diffDays === 1) {
    return "tomorrow";
  } else if (diffDays === -1) {
    return "yesterday";
  } else if (diffDays > 0) {
    return `in ${diffDays} days`;
  } else {
    return `${Math.abs(diffDays)} days ago`;
  }
}

/**
 * Format date for display in list (e.g., "Mon, Feb 10, 2026")
 */
export function formatFullDate(date: Date | string): string {
  const targetDate = new Date(date);

  const monthNames = [
    "Jan",
    "Feb",
    "Mar",
    "Apr",
    "May",
    "Jun",
    "Jul",
    "Aug",
    "Sep",
    "Oct",
    "Nov",
    "Dec",
  ];
  const dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

  return `${dayNames[targetDate.getDay()]}, ${
    monthNames[targetDate.getMonth()]
  } ${targetDate.getDate()}, ${targetDate.getFullYear()}`;
}
