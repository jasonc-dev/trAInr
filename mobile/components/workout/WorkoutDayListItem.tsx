/**
 * WorkoutDayListItem Component
 * Shows a summary of a workout day in the timeline
 */

import {
  View,
  Text,
  Pressable,
  StyleSheet,
  useColorScheme,
} from "react-native";
import {
  colors,
  spacing,
  typography,
  borderRadius,
  shadows,
} from "../../theme";
import { Badge } from "../ui/Badge";
import { formatWorkoutDate } from "../../lib/utils/dateFormatting";
import type { WorkoutDayResponse } from "../../lib/types/programme";

interface WorkoutDayListItemProps {
  workout: WorkoutDayResponse;
  isSelected: boolean;
  isToday: boolean;
  onPress: () => void;
}

export default function WorkoutDayListItem({
  workout,
  isSelected,
  isToday,
  onPress,
}: WorkoutDayListItemProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark, isSelected, isToday);

  const completedSets = workout.exercises.reduce(
    (acc, e) => acc + e.sets.filter((s) => s.isCompleted).length,
    0
  );
  const totalSets = workout.exercises.reduce((acc, e) => acc + e.targetSets, 0);

  return (
    <Pressable
      style={({ pressed }) => [
        styles.container,
        pressed && styles.containerPressed,
      ]}
      onPress={onPress}
    >
      <View style={styles.header}>
        <View style={styles.titleRow}>
          <Text style={styles.name} numberOfLines={1}>
            {workout.name}
          </Text>
          <View style={styles.badges}>
            {isToday && <Badge variant="primary">Today</Badge>}
            {workout.isCompleted && <Badge variant="success">✓</Badge>}
            {workout.isRestDay && <Badge variant="default">Rest</Badge>}
          </View>
        </View>
        <Text style={styles.date}>
          {formatWorkoutDate(workout.scheduledDate)}
        </Text>
      </View>

      {!workout.isRestDay && (
        <View style={styles.details}>
          <Text style={styles.detailText}>
            {workout.exercises.length} exercises
          </Text>
          {totalSets > 0 && (
            <Text style={styles.detailText}>
              {completedSets}/{totalSets} sets
            </Text>
          )}
        </View>
      )}
    </Pressable>
  );
}

const createStyles = (isDark: boolean, isSelected: boolean, isToday: boolean) =>
  StyleSheet.create({
    container: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.md,
      padding: spacing.md,
      marginBottom: spacing.sm,
      borderWidth: 2,
      borderColor: isSelected
        ? colors.primary
        : isToday
        ? isDark
          ? colors.dark.border
          : colors.border
        : "transparent",
      ...shadows.sm,
    },
    containerPressed: {
      opacity: 0.8,
      transform: [{ scale: 0.98 }],
    },
    header: {
      marginBottom: spacing.xs,
    },
    titleRow: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      marginBottom: spacing.xs,
    },
    name: {
      ...typography.h4,
      color: isDark ? colors.dark.text : colors.text,
      flex: 1,
      marginRight: spacing.sm,
    },
    badges: {
      flexDirection: "row",
      gap: spacing.xs,
    },
    date: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    details: {
      flexDirection: "row",
      gap: spacing.md,
    },
    detailText: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
  });
