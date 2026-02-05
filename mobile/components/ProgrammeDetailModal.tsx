/**
 * Programme Detail Modal
 * Shows full programme details with weeks, days, and exercises
 */

import { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  useColorScheme,
  Pressable,
  ActivityIndicator,
  Alert,
} from 'react-native';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { programmesApi } from '../lib/api/programmes';
import { workoutsApi } from '../lib/api/workouts';
import type { Programme, WorkoutDayResponse } from '../lib/types/programme';
import { colors, spacing, typography, borderRadius } from '../theme';
import { Modal, Badge, Button } from './ui';
import AddEditDayModal from './AddEditDayModal';
import ExerciseListModal from './ExerciseListModal';

interface ProgrammeDetailModalProps {
  programmeId: string;
  visible: boolean;
  onClose: () => void;
}

export default function ProgrammeDetailModal({
  programmeId,
  visible,
  onClose,
}: ProgrammeDetailModalProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const [selectedWeekIndex, setSelectedWeekIndex] = useState(0);
  const [showAddDayModal, setShowAddDayModal] = useState(false);
  const [showEditDayModal, setShowEditDayModal] = useState(false);
  const [selectedDay, setSelectedDay] = useState<WorkoutDayResponse | null>(null);
  const [showExerciseListModal, setShowExerciseListModal] = useState(false);

  // Query programme details
  const {
    data: programme,
    isLoading,
    error,
  } = useQuery({
    queryKey: ['programme', programmeId],
    queryFn: () => programmesApi.getById(programmeId),
    enabled: visible && !!programmeId,
  });

  // Mutation to add first week
  const addWeekMutation = useMutation({
    mutationFn: () =>
      programmesApi.addWeek(programmeId, {
        weekNumber: 1,
        notes: '',
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to add week');
    },
  });

  // Mutation to copy week content
  const copyWeekMutation = useMutation({
    mutationFn: ({
      sourceWeekId,
      targetWeekId,
    }: {
      sourceWeekId: string;
      targetWeekId: string;
    }) => programmesApi.copyWeekContent(sourceWeekId, targetWeekId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to copy week content');
    },
  });

  // Mutation to delete day
  const deleteDayMutation = useMutation({
    mutationFn: (dayId: string) => workoutsApi.deleteWorkoutDay(dayId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to delete day');
    },
  });

  const handleEditDay = (day: WorkoutDayResponse) => {
    setSelectedDay(day);
    setShowEditDayModal(true);
  };

  const handleDeleteDay = (day: WorkoutDayResponse) => {
    Alert.alert(
      'Delete Workout Day',
      `Are you sure you want to delete "${day.name}"? All exercises will be removed.`,
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Delete',
          style: 'destructive',
          onPress: () => deleteDayMutation.mutate(day.id),
        },
      ]
    );
  };

  const handleViewExercises = (day: WorkoutDayResponse) => {
    setSelectedDay(day);
    setShowExerciseListModal(true);
  };

  const handleCopyWeek = () => {
    if (!programme || selectedWeekIndex === 0) return;

    const currentWeek = programme.weeks[selectedWeekIndex];
    const previousWeek = programme.weeks[selectedWeekIndex - 1];

    if (!previousWeek || !currentWeek) return;

    // Check if previous week has exercises
    const prevHasExercises = previousWeek.workoutDays.some(
      (day) => day.exercises && day.exercises.length > 0
    );

    if (!prevHasExercises) {
      Alert.alert(
        'No Exercises',
        'The previous week has no exercises to copy.'
      );
      return;
    }

    Alert.alert(
      'Copy Week Content',
      `Copy all exercises from Week ${previousWeek.weekNumber} to Week ${currentWeek.weekNumber}?`,
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Copy',
          onPress: () =>
            copyWeekMutation.mutate({
              sourceWeekId: previousWeek.id,
              targetWeekId: currentWeek.id,
            }),
        },
      ]
    );
  };

  if (isLoading) {
    return (
      <Modal visible={visible} onClose={onClose} title="Loading...">
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={colors.primary} />
        </View>
      </Modal>
    );
  }

  if (error || !programme) {
    return (
      <Modal visible={visible} onClose={onClose} title="Error">
        <View style={styles.errorContainer}>
          <Text style={styles.errorText}>Failed to load programme</Text>
          <Button title="Close" onPress={onClose} variant="primary" />
        </View>
      </Modal>
    );
  }

  const currentWeek = programme.weeks[selectedWeekIndex];
  const previousWeek =
    selectedWeekIndex > 0 ? programme.weeks[selectedWeekIndex - 1] : null;
  const prevWeekHasExercises = previousWeek?.workoutDays.some(
    (day) => day.exercises && day.exercises.length > 0
  );

  return (
    <>
      <Modal visible={visible} onClose={onClose} title={programme.name}>
        {/* Programme Info */}
        <View style={styles.programmeInfo}>
          {programme.description && (
            <Text style={styles.programmeDescription}>
              {programme.description}
            </Text>
          )}
          {programme.isActive && (
            <Badge variant="primary" style={styles.badge}>
              Active
            </Badge>
          )}
        </View>

        {/* No weeks state */}
        {programme.weeks.length === 0 ? (
          <View style={styles.emptyState}>
            <Text style={styles.emptyStateIcon}>📅</Text>
            <Text style={styles.emptyStateTitle}>No Weeks Configured</Text>
            <Text style={styles.emptyStateText}>
              Add weeks to start planning your workouts
            </Text>
            <Button
              title={
                addWeekMutation.isPending ? 'Adding...' : 'Add First Week'
              }
              onPress={() => addWeekMutation.mutate()}
              variant="primary"
              loading={addWeekMutation.isPending}
            />
          </View>
        ) : (
          <>
            {/* Week Tabs */}
            <View style={styles.weekTabs}>
              <ScrollView
                horizontal
                showsHorizontalScrollIndicator={false}
                contentContainerStyle={styles.weekTabsContent}
              >
                {programme.weeks.map((week, index) => (
                  <Pressable
                    key={week.id}
                    style={({ pressed }) => [
                      styles.weekTab,
                      selectedWeekIndex === index && styles.weekTabActive,
                      week.isCompleted && styles.weekTabCompleted,
                      pressed && styles.weekTabPressed,
                    ]}
                    onPress={() => setSelectedWeekIndex(index)}
                  >
                    <Text
                      style={[
                        styles.weekTabText,
                        selectedWeekIndex === index &&
                        styles.weekTabTextActive,
                      ]}
                    >
                      Week {week.weekNumber}
                    </Text>
                  </Pressable>
                ))}
              </ScrollView>
            </View>

            {/* Week Actions */}
            <View style={styles.weekActions}>
              <Button
                title="+ Add Day"
                onPress={() => setShowAddDayModal(true)}
                variant="primary"
                size="sm"
              />
              {selectedWeekIndex > 0 && prevWeekHasExercises && (
                <Button
                  title={`📋 Copy Week ${currentWeek.weekNumber - 1}`}
                  onPress={handleCopyWeek}
                  variant="secondary"
                  size="sm"
                  loading={copyWeekMutation.isPending}
                />
              )}
            </View>

            {/* Workout Days */}
            {currentWeek.workoutDays.length === 0 ? (
              <View style={styles.emptyState}>
                <Text style={styles.emptyStateIcon}>📅</Text>
                <Text style={styles.emptyStateTitle}>No Workout Days</Text>
                <Text style={styles.emptyStateText}>
                  Add workout days to plan your week
                </Text>
                <Button
                  title="Add First Day"
                  onPress={() => setShowAddDayModal(true)}
                  variant="primary"
                />
              </View>
            ) : (
              <View style={styles.daysGrid}>
                {currentWeek.workoutDays
                  .sort(
                    (a, b) =>
                      new Date(a.scheduledDate).getTime() -
                      new Date(b.scheduledDate).getTime()
                  )
                  .map((day) => (
                    <View
                      key={day.id}
                      style={[
                        styles.dayCard,
                        day.isRestDay && styles.dayCardRest,
                        day.isCompleted && styles.dayCardCompleted,
                      ]}
                    >
                      <View style={styles.dayCardHeader}>
                        <View style={styles.dayCardInfo}>
                          <Text style={styles.dayCardName}>{day.name}</Text>
                          <Text style={styles.dayCardDate}>
                            {new Date(day.scheduledDate).toLocaleDateString(
                              'en-US',
                              { weekday: 'short' }
                            )}
                          </Text>
                        </View>
                        <View style={styles.dayCardBadges}>
                          {day.isRestDay && <Badge>Rest</Badge>}
                          {day.isCompleted && (
                            <Badge variant="success">Done</Badge>
                          )}
                        </View>
                      </View>

                      {!day.isRestDay && (
                        <Text style={styles.dayCardExerciseCount}>
                          {day.exercises.length} exercise
                          {day.exercises.length !== 1 ? 's' : ''}
                        </Text>
                      )}

                      <View style={styles.dayCardActions}>
                        {!day.isRestDay && (
                          <Button
                            title="Exercises"
                            onPress={() => handleViewExercises(day)}
                            variant="primary"
                            size="sm"
                          />
                        )}
                        <Button
                          title="Edit"
                          onPress={() => handleEditDay(day)}
                          variant="secondary"
                          size="sm"
                        />
                        <Button
                          title="Delete"
                          onPress={() => handleDeleteDay(day)}
                          variant="ghost"
                          size="sm"
                        />
                      </View>
                    </View>
                  ))}
              </View>
            )}
          </>
        )}
      </Modal>

      {/* Add/Edit Day Modal */}
      {currentWeek && (
        <>
          <AddEditDayModal
            visible={showAddDayModal}
            onClose={() => setShowAddDayModal(false)}
            weekId={currentWeek.id}
            programmeId={programmeId}
          />
          {selectedDay && (
            <AddEditDayModal
              visible={showEditDayModal}
              onClose={() => {
                setShowEditDayModal(false);
                setSelectedDay(null);
              }}
              weekId={currentWeek.id}
              programmeId={programmeId}
              day={selectedDay}
            />
          )}
        </>
      )}

      {/* Exercise List Modal */}
      {selectedDay && (
        <ExerciseListModal
          visible={showExerciseListModal}
          onClose={() => {
            setShowExerciseListModal(false);
            setSelectedDay(null);
          }}
          day={selectedDay}
          programmeId={programmeId}
        />
      )}
    </>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    programmeInfo: {
      marginBottom: spacing.lg,
      gap: spacing.sm,
    },
    programmeDescription: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    badge: {
      alignSelf: 'flex-start',
    },
    loadingContainer: {
      padding: spacing.xl,
      alignItems: 'center',
    },
    errorContainer: {
      padding: spacing.xl,
      alignItems: 'center',
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
        ? 'rgba(16, 185, 129, 0.2)'
        : 'rgba(16, 185, 129, 0.1)',
      borderColor: colors.success,
    },
    weekTabPressed: {
      opacity: 0.7,
    },
    weekTabText: {
      ...typography.body,
      fontWeight: '500',
      color: isDark ? colors.dark.text : colors.text,
    },
    weekTabTextActive: {
      color: '#FFFFFF',
      fontWeight: '600',
    },
    weekActions: {
      flexDirection: 'row',
      gap: spacing.sm,
      marginBottom: spacing.md,
      flexWrap: 'wrap',
    },
    emptyState: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: spacing.xl,
      alignItems: 'center',
      marginTop: spacing.md,
    },
    emptyStateIcon: {
      fontSize: 64,
      marginBottom: spacing.md,
    },
    emptyStateTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    emptyStateText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      textAlign: 'center',
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
        ? 'rgba(16, 185, 129, 0.1)'
        : 'rgba(16, 185, 129, 0.05)',
      borderColor: colors.success,
    },
    dayCardHeader: {
      flexDirection: 'row',
      justifyContent: 'space-between',
      alignItems: 'flex-start',
      marginBottom: spacing.sm,
    },
    dayCardInfo: {
      flex: 1,
    },
    dayCardName: {
      ...typography.body,
      fontWeight: '600',
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    dayCardDate: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    dayCardBadges: {
      flexDirection: 'row',
      gap: spacing.xs,
    },
    dayCardExerciseCount: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.sm,
    },
    dayCardActions: {
      flexDirection: 'row',
      gap: spacing.xs,
    },
  });
