import { StyleSheet } from "react-native";
import { colors, spacing, typography, borderRadius } from "../../theme";

export const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    // ---- AddEditDayModal / shared form ----
    container: {
      flex: 1,
      padding: spacing.md,
    },
    formGroup: {
      gap: spacing.md,
    },
    footer: {
      flexDirection: "row",
      justifyContent: "flex-end",
      gap: spacing.sm,
    },
    checkboxContainer: {
      marginTop: spacing.sm,
    },
    checkbox: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.sm,
    },
    checkboxBox: {
      width: 24,
      height: 24,
      borderRadius: borderRadius.sm,
      borderWidth: 2,
      borderColor: isDark ? colors.dark.border : colors.border,
      alignItems: "center",
      justifyContent: "center",
    },
    checkboxBoxChecked: {
      backgroundColor: colors.primary,
      borderColor: colors.primary,
    },
    checkboxCheck: {
      color: "#FFFFFF",
      fontSize: 16,
      fontWeight: "bold",
    },
    checkboxLabel: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
    },

    // ---- EditExerciseForm ----
    editFormContainer: {
      flex: 1,
      justifyContent: "space-between",
    },
    editFormFooter: {
      flexDirection: "row",
      justifyContent: "flex-end",
      gap: spacing.sm,
      marginTop: spacing.md,
    },
    numberInputRow: {
      flexDirection: "row",
      gap: spacing.lg,
      alignItems: "flex-end",
    },
    select: {},

    // ---- AddExerciseModal ----
    addExerciseContent: {
      gap: spacing.md,
    },
    searchState: {
      padding: spacing.lg,
      alignItems: "center",
    },
    searchStateText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    searchResults: {
      maxHeight: 200,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
      borderRadius: borderRadius.md,
    },
    exerciseResult: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      padding: spacing.md,
      borderBottomWidth: 1,
      borderBottomColor: isDark ? colors.dark.border : colors.border,
    },
    exerciseResultSelected: {
      backgroundColor: isDark
        ? "rgba(99, 102, 241, 0.2)"
        : "rgba(99, 102, 241, 0.1)",
    },
    exerciseResultInfo: {
      flex: 1,
    },
    exerciseResultName: {
      ...typography.body,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    exerciseResultMeta: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    checkmark: {
      fontSize: 18,
      color: colors.primary,
      fontWeight: "bold",
    },
    configSection: {
      gap: spacing.md,
      paddingTop: spacing.md,
      borderTopWidth: 1,
      borderTopColor: isDark ? colors.dark.border : colors.border,
    },
    configTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
    },

    // ---- ExerciseDetailsContent / ExerciseDetailsModal ----
    detailsLoading: {
      padding: spacing.xl,
      alignItems: "center",
      justifyContent: "center",
    },
    detailsLoadingModal: {
      padding: spacing.xl,
      alignItems: "center",
    },
    detailsError: {
      padding: spacing.xl,
      alignItems: "center",
    },
    detailsErrorText: {
      ...typography.body,
      color: colors.error,
    },
    detailsContent: {
      gap: spacing.lg,
    },
    detailsFooter: {
      flexDirection: "row",
      justifyContent: "space-between",
      gap: spacing.sm,
    },
    section: {
      gap: spacing.sm,
    },
    sectionTitle: {
      ...typography.body,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
      textTransform: "uppercase",
      letterSpacing: 0.5,
      fontSize: 12,
    },
    sectionText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      lineHeight: 24,
    },
    badgeRow: {
      flexDirection: "row",
      flexWrap: "wrap",
      gap: spacing.xs,
    },
    targetsGrid: {
      flexDirection: "row",
      flexWrap: "wrap",
      gap: spacing.md,
    },
    targetItem: {
      flex: 1,
      minWidth: "30%",
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
      padding: spacing.sm,
      borderRadius: borderRadius.md,
    },
    targetItemContent: {
      flex: 1,
      minWidth: "30%",
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
      padding: spacing.sm,
      borderRadius: borderRadius.md,
    },
    targetLabel: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.xs,
    },
    targetValue: {
      ...typography.body,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
    },
    notesBox: {
      marginTop: spacing.sm,
      padding: spacing.sm,
      backgroundColor: isDark
        ? "rgba(99, 102, 241, 0.1)"
        : "rgba(99, 102, 241, 0.05)",
      borderRadius: borderRadius.sm,
    },
    notesLabel: {
      ...typography.caption,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    notesText: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.text : colors.text,
      fontStyle: "italic",
    },

    // ---- ExerciseListModal ----
    listEmptyState: {
      padding: spacing.xl,
      alignItems: "center",
    },
    listEmptyStateIcon: {
      fontSize: 64,
      marginBottom: spacing.md,
    },
    listEmptyStateTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    listEmptyStateText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      textAlign: "center",
    },
    exercisesList: {
      gap: spacing.sm,
    },
    exerciseItem: {
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
      borderRadius: borderRadius.md,
      padding: spacing.sm,
      flexDirection: "row",
      alignItems: "flex-start",
      gap: spacing.sm,
    },
    exerciseItemSuperset: {
      borderWidth: 1,
      borderColor: isDark
        ? "rgba(99, 102, 241, 0.3)"
        : "rgba(99, 102, 241, 0.2)",
    },
    exerciseContent: {
      flex: 1,
      flexDirection: "row",
      alignItems: "flex-start",
      gap: spacing.sm,
    },
    listCheckbox: {
      paddingTop: 2,
    },
    listCheckboxBox: {
      width: 20,
      height: 20,
      borderRadius: borderRadius.sm,
      borderWidth: 2,
      borderColor: isDark ? colors.dark.border : colors.border,
      alignItems: "center",
      justifyContent: "center",
    },
    listCheckboxBoxChecked: {
      backgroundColor: colors.primary,
      borderColor: colors.primary,
    },
    listCheckboxCheck: {
      color: "#FFFFFF",
      fontSize: 14,
      fontWeight: "bold",
    },
    exerciseInfo: {
      flex: 1,
    },
    exerciseHeader: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.xs,
      marginBottom: spacing.xs,
      flexWrap: "wrap",
    },
    exerciseName: {
      ...typography.body,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
    },
    supersetBadge: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.xs,
    },
    ungroupButton: {
      width: 18,
      height: 18,
      borderRadius: 9,
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
      alignItems: "center",
      justifyContent: "center",
    },
    ungroupButtonText: {
      fontSize: 12,
      color: isDark ? colors.dark.text : colors.text,
    },
    exerciseDetails: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.xs,
    },
    exerciseNotes: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      fontStyle: "italic",
    },
    exerciseActions: {
      flexDirection: "column",
      gap: spacing.xs,
    },
    listActionButton: {
      width: 32,
      height: 32,
      borderRadius: borderRadius.sm,
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      alignItems: "center",
      justifyContent: "center",
    },
    listActionButtonDisabled: {
      opacity: 0.3,
    },
    listActionButtonText: {
      fontSize: 16,
      color: isDark ? colors.dark.text : colors.text,
    },

    // ---- ProgrammeDetailModal ----
    programmeInfo: {
      marginBottom: spacing.lg,
      gap: spacing.sm,
    },
    programmeDescription: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    programmeBadge: {
      alignSelf: "flex-start",
    },
    loadingContainer: {
      padding: spacing.xl,
      alignItems: "center",
    },
    errorContainer: {
      padding: spacing.xl,
      alignItems: "center",
      gap: spacing.md,
    },
    errorText: {
      ...typography.body,
      color: colors.error,
    },
    weekTabs: {
      marginBottom: spacing.md,
      borderBottomWidth: 1,
      borderBottomColor: isDark ? colors.dark.border : colors.border,
    },
    weekTabsContent: {
      paddingHorizontal: 0,
      paddingBottom: spacing.sm,
      gap: spacing.xs,
    },
    weekTab: {
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.sm,
      borderRadius: borderRadius.md,
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
    },
    weekTabActive: {
      backgroundColor: colors.primary,
      borderColor: colors.primary,
    },
    weekTabCompleted: {
      backgroundColor: isDark
        ? "rgba(16, 185, 129, 0.2)"
        : "rgba(16, 185, 129, 0.1)",
      borderColor: colors.success,
    },
    weekTabPressed: {
      opacity: 0.7,
    },
    weekTabText: {
      ...typography.body,
      fontWeight: "500",
      color: isDark ? colors.dark.text : colors.text,
    },
    weekTabTextActive: {
      color: "#FFFFFF",
      fontWeight: "600",
    },
    weekActions: {
      flexDirection: "row",
      gap: spacing.sm,
      marginBottom: spacing.md,
      flexWrap: "wrap",
    },
    programmeEmptyState: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: spacing.xl,
      alignItems: "center",
      marginTop: spacing.md,
    },
    programmeEmptyStateIcon: {
      fontSize: 64,
      marginBottom: spacing.md,
    },
    programmeEmptyStateTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    programmeEmptyStateText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      textAlign: "center",
      marginBottom: spacing.md,
    },
    daysGrid: {
      gap: spacing.md,
    },
    dayCard: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: spacing.md,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
    },
    dayCardRest: {
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
    },
    dayCardCompleted: {
      backgroundColor: isDark
        ? "rgba(16, 185, 129, 0.1)"
        : "rgba(16, 185, 129, 0.05)",
      borderColor: colors.success,
    },
    dayCardHeader: {
      flexDirection: "row",
      justifyContent: "space-between",
      alignItems: "flex-start",
      marginBottom: spacing.sm,
    },
    dayCardInfo: {
      flex: 1,
    },
    dayCardName: {
      ...typography.body,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    dayCardDate: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    dayCardBadges: {
      flexDirection: "row",
      gap: spacing.xs,
    },
    dayCardExerciseCount: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.sm,
    },
    dayCardActions: {
      flexDirection: "row",
      gap: spacing.xs,
    },
    dayActionButton: {
      flex: 1,
    },
    actionButtonPrimary: {
      backgroundColor: colors.primary,
      paddingVertical: spacing.xs,
      paddingHorizontal: spacing.md,
      borderRadius: borderRadius.md,
      alignItems: "center",
    },
    actionButtonSecondary: {
      backgroundColor: isDark ? colors.dark.surface : colors.surfaceSecondary,
      paddingVertical: spacing.xs,
      paddingHorizontal: spacing.md,
      borderRadius: borderRadius.md,
      alignItems: "center",
    },
    actionButtonGhost: {
      backgroundColor: "transparent",
      paddingVertical: spacing.xs,
      paddingHorizontal: spacing.md,
      borderRadius: borderRadius.md,
      alignItems: "center",
    },
    actionButtonTextPrimary: {
      ...typography.button,
      fontSize: 14,
      color: "#FFFFFF",
    },
    actionButtonTextSecondary: {
      ...typography.button,
      fontSize: 14,
      color: isDark ? colors.dark.text : colors.text,
    },
    actionButtonTextGhost: {
      ...typography.button,
      fontSize: 14,
      color: colors.primary,
    },
  });
