/**
 * Workout Components Styles
 * Shared styles for ExerciseCard and SetRow
 */

import { StyleSheet } from "react-native";
import { colors, spacing, typography, borderRadius } from "../../theme";

export function createExerciseCardStyles(
  isDark: boolean,
  isExpanded: boolean,
  isInSuperset: boolean,
  supersetPosition: "first" | "middle" | "last" | "single"
) {
  let borderRadiusValue: number = borderRadius.lg;
  let borderTopWidth = 1;
  let borderBottomWidth = 1;

  if (isInSuperset) {
    if (supersetPosition === "first") {
      borderRadiusValue = borderRadius.lg;
      borderBottomWidth = 0;
    } else if (supersetPosition === "middle") {
      borderRadiusValue = 0;
      borderTopWidth = 0;
      borderBottomWidth = 0;
    } else if (supersetPosition === "last") {
      borderRadiusValue = borderRadius.lg;
      borderTopWidth = 0;
    }
  }

  return StyleSheet.create({
    card: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadiusValue,
      marginBottom:
        isInSuperset && supersetPosition !== "last" ? 0 : spacing.sm,
      borderWidth: 1,
      borderColor: isExpanded
        ? colors.primary
        : isDark
        ? colors.dark.border
        : colors.border,
      borderTopWidth,
      borderBottomWidth,
    },
    header: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      padding: spacing.md,
      paddingBottom: spacing.sm,
    },
    headerContent: {
      flex: 1,
      marginRight: spacing.sm,
    },
    titleRow: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      marginBottom: spacing.xs,
      gap: spacing.sm,
    },
    title: {
      ...typography.body,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
      flex: 1,
    },
    badges: {
      flexDirection: "row",
      gap: spacing.xs,
    },
    subtitle: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    expandIcon: {
      fontSize: 24,
      color: isDark ? colors.dark.text : colors.text,
    },
    expandedContent: {
      paddingHorizontal: spacing.md,
      paddingBottom: spacing.md,
    },
    setHeader: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.sm,
      paddingBottom: spacing.sm,
    },
    setHeaderText: {
      ...typography.caption,
      textTransform: "uppercase",
      letterSpacing: 0.5,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      flex: 1,
      textAlign: "center",
    },
    setHeaderSpacer: {
      width: 32,
    },
    addSetButton: {
      marginTop: spacing.md,
    },
  });
}

export function createSetRowStyles(isDark: boolean) {
  return StyleSheet.create({
    container: {
      flexDirection: "row",
      alignItems: "center",
      paddingVertical: spacing.sm,
      gap: spacing.sm,
      borderTopWidth: 1,
      borderTopColor: isDark ? colors.dark.border : colors.border,
    },
    containerCompleted: {
      opacity: 0.6,
    },
    setNumber: {
      ...typography.bodySmall,
      fontWeight: "600",
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      width: 32,
      textAlign: "center",
    },
    inputColumn: {
      flex: 1,
      justifyContent: "center",
    },
    completeButton: {
      width: 32,
      height: 32,
      borderRadius: 16,
      borderWidth: 2,
      borderColor: isDark ? colors.dark.border : colors.border,
      backgroundColor: "transparent",
      alignItems: "center",
      justifyContent: "center",
    },
    completeButtonCompleted: {
      backgroundColor: colors.success,
      borderColor: colors.success,
    },
    completeButtonText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
    },
    completeButtonTextCompleted: {
      color: "#FFFFFF",
      fontSize: 20,
      fontWeight: "bold",
    },
  });
}
