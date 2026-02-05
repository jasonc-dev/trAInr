/**
 * Exercise List Modal
 * Shows and manages all exercises for a workout day
 */

import { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  useColorScheme,
  Pressable,
  Alert,
} from 'react-native';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { workoutsApi } from '../lib/api/workouts';
import type { WorkoutDayResponse, WorkoutExerciseResponse } from '../lib/types/programme';
import { colors, spacing, typography, borderRadius } from '../theme';
import { Modal, Button, Badge } from './ui';
import AddExerciseModal from './AddExerciseModal';
import EditExerciseModal from './EditExerciseModal';
import ExerciseDetailsModal from './ExerciseDetailsModal';

interface ExerciseListModalProps {
  visible: boolean;
  onClose: () => void;
  day: WorkoutDayResponse;
  programmeId: string;
}

export default function ExerciseListModal({
  visible,
  onClose,
  day,
  programmeId,
}: ExerciseListModalProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [selectedExercise, setSelectedExercise] = useState<WorkoutExerciseResponse | null>(null);
  const [selectedExercises, setSelectedExercises] = useState<Set<string>>(new Set());

  // Mutations
  const removeMutation = useMutation({
    mutationFn: (exerciseId: string) => workoutsApi.removeExercise(exerciseId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to remove exercise');
    },
  });

  const reorderMutation = useMutation({
    mutationFn: ({ exerciseIds }: { exerciseIds: string[] }) =>
      workoutsApi.reorderExercises(day.id, exerciseIds),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to reorder exercises');
    },
  });

  const groupSupersetMutation = useMutation({
    mutationFn: ({ exerciseIds }: { exerciseIds: string[] }) =>
      workoutsApi.groupSuperset(day.id, {
        exerciseIds,
        supersetRestSeconds: 120,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
      setSelectedExercises(new Set());
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to create superset');
    },
  });

  const ungroupSupersetMutation = useMutation({
    mutationFn: (supersetGroupId: string) =>
      workoutsApi.ungroupSuperset(supersetGroupId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to ungroup superset');
    },
  });

  // Handlers
  const handleToggleSelect = (exerciseId: string) => {
    setSelectedExercises((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(exerciseId)) {
        newSet.delete(exerciseId);
      } else {
        newSet.add(exerciseId);
      }
      return newSet;
    });
  };

  const handleEdit = (exercise: WorkoutExerciseResponse) => {
    setSelectedExercise(exercise);
    setShowEditModal(true);
  };

  const handleViewDetails = (exercise: WorkoutExerciseResponse) => {
    setSelectedExercise(exercise);
    setShowDetailsModal(true);
  };

  const handleRemove = (exercise: WorkoutExerciseResponse) => {
    Alert.alert(
      'Remove Exercise',
      `Remove "${exercise.exerciseName}" from this workout?`,
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Remove',
          style: 'destructive',
          onPress: () => removeMutation.mutate(exercise.id),
        },
      ]
    );
  };

  const handleMoveUp = (exercise: WorkoutExerciseResponse) => {
    const sortedExercises = [...day.exercises].sort((a, b) => a.orderIndex - b.orderIndex);
    const currentIndex = sortedExercises.findIndex((e) => e.id === exercise.id);

    if (currentIndex <= 0) return;

    const newOrder = [...sortedExercises];
    [newOrder[currentIndex - 1], newOrder[currentIndex]] = [
      newOrder[currentIndex],
      newOrder[currentIndex - 1],
    ];

    reorderMutation.mutate({ exerciseIds: newOrder.map((e) => e.id) });
  };

  const handleMoveDown = (exercise: WorkoutExerciseResponse) => {
    const sortedExercises = [...day.exercises].sort((a, b) => a.orderIndex - b.orderIndex);
    const currentIndex = sortedExercises.findIndex((e) => e.id === exercise.id);

    if (currentIndex >= sortedExercises.length - 1) return;

    const newOrder = [...sortedExercises];
    [newOrder[currentIndex], newOrder[currentIndex + 1]] = [
      newOrder[currentIndex + 1],
      newOrder[currentIndex],
    ];

    reorderMutation.mutate({ exerciseIds: newOrder.map((e) => e.id) });
  };

  const handleGroupSuperset = () => {
    if (selectedExercises.size < 2) {
      Alert.alert('Error', 'Select at least 2 exercises to create a superset');
      return;
    }

    groupSupersetMutation.mutate({ exerciseIds: Array.from(selectedExercises) });
  };

  const handleUngroup = (supersetGroupId: string) => {
    Alert.alert(
      'Ungroup Superset',
      'Remove these exercises from the superset?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Ungroup',
          onPress: () => ungroupSupersetMutation.mutate(supersetGroupId),
        },
      ]
    );
  };

  // Get superset label
  const getSupersetLabel = (exercise: WorkoutExerciseResponse): string => {
    if (!exercise.supersetGroupId) return '';

    const supersetCount = day.exercises.filter(
      (e) => e.supersetGroupId === exercise.supersetGroupId
    ).length;

    if (supersetCount === 2) return 'Superset';
    if (supersetCount === 3) return 'Triset';
    return 'Giant Set';
  };

  const sortedExercises = [...day.exercises].sort((a, b) => a.orderIndex - b.orderIndex);

  return (
    <>
      <Modal
        visible={visible}
        onClose={onClose}
        title={`${day.name} - Exercises`}
        footer={
          <View style={styles.footer}>
            {selectedExercises.size >= 2 ? (
              <Button
                title={`Group as ${selectedExercises.size === 2
                    ? 'Superset'
                    : selectedExercises.size === 3
                      ? 'Triset'
                      : 'Giant Set'
                  } (${selectedExercises.size})`}
                onPress={handleGroupSuperset}
                variant="primary"
                loading={groupSupersetMutation.isPending}
              />
            ) : (
              <Button
                title="+ Add Exercise"
                onPress={() => setShowAddModal(true)}
                variant="primary"
              />
            )}
          </View>
        }
      >
        {sortedExercises.length === 0 ? (
          <View style={styles.emptyState}>
            <Text style={styles.emptyStateIcon}>💪</Text>
            <Text style={styles.emptyStateTitle}>No Exercises</Text>
            <Text style={styles.emptyStateText}>
              Add exercises to this workout day
            </Text>
          </View>
        ) : (
          <View style={styles.exercisesList}>
            {sortedExercises.map((exercise, index) => {
              const isSelected = selectedExercises.has(exercise.id);
              const isInSuperset = !!exercise.supersetGroupId;
              const isFirst = index === 0;
              const isLast = index === sortedExercises.length - 1;

              return (
                <View
                  key={exercise.id}
                  style={[
                    styles.exerciseItem,
                    isInSuperset && styles.exerciseItemSuperset,
                  ]}
                >
                  <Pressable
                    style={styles.exerciseContent}
                    onPress={() => handleViewDetails(exercise)}
                  >
                    {/* Checkbox for superset selection */}
                    <Pressable
                      style={styles.checkbox}
                      onPress={() => handleToggleSelect(exercise.id)}
                    >
                      <View
                        style={[
                          styles.checkboxBox,
                          isSelected && styles.checkboxBoxChecked,
                        ]}
                      >
                        {isSelected && (
                          <Text style={styles.checkboxCheck}>✓</Text>
                        )}
                      </View>
                    </Pressable>

                    {/* Exercise info */}
                    <View style={styles.exerciseInfo}>
                      <View style={styles.exerciseHeader}>
                        <Text style={styles.exerciseName}>
                          {exercise.exerciseName}
                        </Text>
                        {exercise.supersetGroupId && (
                          <View style={styles.supersetBadge}>
                            <Badge variant="primary">
                              {getSupersetLabel(exercise)}
                            </Badge>
                            <Pressable
                              onPress={() =>
                                handleUngroup(exercise.supersetGroupId!)
                              }
                              style={styles.ungroupButton}
                            >
                              <Text style={styles.ungroupButtonText}>✕</Text>
                            </Pressable>
                          </View>
                        )}
                      </View>
                      <Text style={styles.exerciseDetails}>
                        {exercise.targetSets} sets × {exercise.targetReps} reps
                        {exercise.targetWeight && ` @ ${exercise.targetWeight}kg`}
                        {exercise.restSeconds &&
                          ` • ${Math.floor(exercise.restSeconds / 60)}:${String(
                            exercise.restSeconds % 60
                          ).padStart(2, '0')} rest`}
                      </Text>
                      {exercise.notes && (
                        <Text style={styles.exerciseNotes}>
                          💡 {exercise.notes}
                        </Text>
                      )}
                    </View>
                  </Pressable>

                  {/* Action buttons */}
                  <View style={styles.exerciseActions}>
                    <Pressable
                      style={[styles.actionButton, isFirst && styles.actionButtonDisabled]}
                      onPress={() => handleMoveUp(exercise)}
                      disabled={isFirst}
                    >
                      <Text style={styles.actionButtonText}>↑</Text>
                    </Pressable>
                    <Pressable
                      style={[styles.actionButton, isLast && styles.actionButtonDisabled]}
                      onPress={() => handleMoveDown(exercise)}
                      disabled={isLast}
                    >
                      <Text style={styles.actionButtonText}>↓</Text>
                    </Pressable>
                    <Pressable
                      style={styles.actionButton}
                      onPress={() => handleEdit(exercise)}
                    >
                      <Text style={styles.actionButtonText}>✎</Text>
                    </Pressable>
                    <Pressable
                      style={styles.actionButton}
                      onPress={() => handleRemove(exercise)}
                    >
                      <Text style={styles.actionButtonText}>×</Text>
                    </Pressable>
                  </View>
                </View>
              );
            })}
          </View>
        )}
      </Modal>

      {/* Add Exercise Modal */}
      <AddExerciseModal
        visible={showAddModal}
        onClose={() => setShowAddModal(false)}
        dayId={day.id}
        programmeId={programmeId}
      />

      {/* Edit Exercise Modal */}
      {selectedExercise && (
        <EditExerciseModal
          visible={showEditModal}
          onClose={() => {
            setShowEditModal(false);
            setSelectedExercise(null);
          }}
          exercise={selectedExercise}
          programmeId={programmeId}
        />
      )}

      {/* Exercise Details Modal */}
      {selectedExercise && (
        <ExerciseDetailsModal
          visible={showDetailsModal}
          onClose={() => {
            setShowDetailsModal(false);
            setSelectedExercise(null);
          }}
          exercise={selectedExercise}
          programmeId={programmeId}
          onEdit={() => {
            setShowDetailsModal(false);
            setShowEditModal(true);
          }}
          onRemove={() => {
            setShowDetailsModal(false);
            handleRemove(selectedExercise);
          }}
        />
      )}
    </>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    footer: {
      flexDirection: 'row',
      justifyContent: 'flex-end',
    },
    emptyState: {
      padding: spacing.xl,
      alignItems: 'center',
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
      flexDirection: 'row',
      alignItems: 'flex-start',
      gap: spacing.sm,
    },
    exerciseItemSuperset: {
      borderWidth: 1,
      borderColor: isDark
        ? 'rgba(99, 102, 241, 0.3)'
        : 'rgba(99, 102, 241, 0.2)',
    },
    exerciseContent: {
      flex: 1,
      flexDirection: 'row',
      alignItems: 'flex-start',
      gap: spacing.sm,
    },
    checkbox: {
      paddingTop: 2,
    },
    checkboxBox: {
      width: 20,
      height: 20,
      borderRadius: borderRadius.sm,
      borderWidth: 2,
      borderColor: isDark ? colors.dark.border : colors.border,
      alignItems: 'center',
      justifyContent: 'center',
    },
    checkboxBoxChecked: {
      backgroundColor: colors.primary,
      borderColor: colors.primary,
    },
    checkboxCheck: {
      color: '#FFFFFF',
      fontSize: 14,
      fontWeight: 'bold',
    },
    exerciseInfo: {
      flex: 1,
    },
    exerciseHeader: {
      flexDirection: 'row',
      alignItems: 'center',
      gap: spacing.xs,
      marginBottom: spacing.xs,
      flexWrap: 'wrap',
    },
    exerciseName: {
      ...typography.body,
      fontWeight: '600',
      color: isDark ? colors.dark.text : colors.text,
    },
    supersetBadge: {
      flexDirection: 'row',
      alignItems: 'center',
      gap: spacing.xs,
    },
    ungroupButton: {
      width: 18,
      height: 18,
      borderRadius: 9,
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
      alignItems: 'center',
      justifyContent: 'center',
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
      fontStyle: 'italic',
    },
    exerciseActions: {
      flexDirection: 'column',
      gap: spacing.xs,
    },
    actionButton: {
      width: 32,
      height: 32,
      borderRadius: borderRadius.sm,
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      alignItems: 'center',
      justifyContent: 'center',
    },
    actionButtonDisabled: {
      opacity: 0.3,
    },
    actionButtonText: {
      fontSize: 16,
      color: isDark ? colors.dark.text : colors.text,
    },
  });
