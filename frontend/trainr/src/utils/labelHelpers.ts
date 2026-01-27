export const getWorkoutDayNames = (daysPerWeek: number): string[] => {
  const allDays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
  return allDays.slice(0, daysPerWeek);
};

export const getFitnessLevelLabel = (level: number): string => {
  const labels = ["Beginner", "Intermediate", "Advanced", "Elite"];
  return labels[level] || "Unknown";
};

export const getFitnessGoalLabel = (goal: number): string => {
  const labels = ["Build Muscle", "Lose Weight", "Improve Endurance", "Increase Strength", "General Fitness"];
  return labels[goal] || "Unknown";
};
