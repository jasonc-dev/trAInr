/**
 * Exercise Details Modal
 * Shows full exercise information with actions
 */

import { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  useColorScheme,
  ActivityIndicator,
} from 'react-native';
import { useQuery } from '@tanstack/react-query';
import { exercisesApi } from '../lib/api/exercises';
import type { WorkoutExerciseResponse } from '../lib/types/programme';
import { colors, spacing, typography, borderRadius } from '../theme';
import { Modal, Button, Badge } from './ui';

interface ExerciseDetailsModalProps {
  visible: boolean;
  onClose: () => void;
  exercise: WorkoutExerciseResponse;
  programmeId: string;
  onEdit: () => void;
  onRemove: () => void;
}

export default function ExerciseDetailsModal({
  visible,
  onClose,
  exercise,
  programmeId,
  onEdit,
  onRemove,
}: ExerciseDetailsModalProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark);

  const { data: details, isLoading } = useQuery({
    queryKey: ['exercise', exercise.exerciseDefinitionId],
    queryFn: () => exercisesApi.getExerciseById(exercise.exerciseDefinitionId),
    enabled: visible,
  });

  return (
    <Modal
      visible={visible}
      onClose={onClose}
      title={exercise.exerciseName}
      footer={
        <View style={styles.footer}>
          <Button
            title="Remove"
            onPress={() => {
              onClose();
              onRemove();
            }}
            variant="danger"
          />
          <Button title="Edit" onPress={onEdit} variant="primary" />
        </View>
      }
    >
      {isLoading ? (
        <View style={styles.loading}>
          <ActivityIndicator size="large" color={colors.primary} />
        </View>
      ) : details ? (
        <View style={styles.content}>
          {/* Description */}
          {details.description && (
            <View style={styles.section}>
              <Text style={styles.sectionTitle}>Description</Text>
              <Text style={styles.sectionText}>{details.description}</Text>
            </View>
          )}

          {/* Exercise Info */}
          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Exercise Information</Text>
            <View style={styles.badgeRow}>
              <Badge>{details.type}</Badge>
              <Badge>{details.primaryMuscleGroup}</Badge>
              {details.secondaryMuscleGroup && (
                <Badge variant="default">{details.secondaryMuscleGroup}</Badge>
              )}
            </View>
          </View>

          {/* Instructions */}
          {details.instructions && (
            <View style={styles.section}>
              <Text style={styles.sectionTitle}>Instructions</Text>
              <Text style={styles.sectionText}>{details.instructions}</Text>
            </View>
          )}

          {/* Your Targets */}
          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Your Targets</Text>
            <View style={styles.targetsGrid}>
              <View style={styles.targetItem}>
                <Text style={styles.targetLabel}>Sets</Text>
                <Text style={styles.targetValue}>{exercise.targetSets}</Text>
              </View>
              <View style={styles.targetItem}>
                <Text style={styles.targetLabel}>Reps</Text>
                <Text style={styles.targetValue}>{exercise.targetReps}</Text>
              </View>
              {exercise.targetWeight && (
                <View style={styles.targetItem}>
                  <Text style={styles.targetLabel}>Weight</Text>
                  <Text style={styles.targetValue}>
                    {exercise.targetWeight}kg
                  </Text>
                </View>
              )}
              {exercise.restSeconds && (
                <View style={styles.targetItem}>
                  <Text style={styles.targetLabel}>Rest</Text>
                  <Text style={styles.targetValue}>
                    {Math.floor(exercise.restSeconds / 60)}:
                    {String(exercise.restSeconds % 60).padStart(2, '0')}
                  </Text>
                </View>
              )}
              {exercise.targetRpe && (
                <View style={styles.targetItem}>
                  <Text style={styles.targetLabel}>RPE</Text>
                  <Text style={styles.targetValue}>
                    {exercise.targetRpe}/10
                  </Text>
                </View>
              )}
            </View>
            {exercise.notes && (
              <View style={styles.notesBox}>
                <Text style={styles.notesLabel}>Notes:</Text>
                <Text style={styles.notesText}>{exercise.notes}</Text>
              </View>
            )}
          </View>
        </View>
      ) : (
        <View style={styles.error}>
          <Text style={styles.errorText}>Failed to load exercise details</Text>
        </View>
      )}
    </Modal>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    loading: {
      padding: spacing.xl,
      alignItems: 'center',
    },
    content: {
      gap: spacing.lg,
    },
    error: {
      padding: spacing.xl,
      alignItems: 'center',
    },
    errorText: {
      ...typography.body,
      color: colors.error,
    },
    footer: {
      flexDirection: 'row',
      justifyContent: 'space-between',
      gap: spacing.sm,
    },
    section: {
      gap: spacing.sm,
    },
    sectionTitle: {
      ...typography.body,
      fontWeight: '600',
      color: isDark ? colors.dark.text : colors.text,
      textTransform: 'uppercase',
      letterSpacing: 0.5,
      fontSize: 12,
    },
    sectionText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      lineHeight: 24,
    },
    badgeRow: {
      flexDirection: 'row',
      flexWrap: 'wrap',
      gap: spacing.xs,
    },
    targetsGrid: {
      flexDirection: 'row',
      flexWrap: 'wrap',
      gap: spacing.md,
    },
    targetItem: {
      flex: 1,
      minWidth: '30%',
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
      fontWeight: '600',
      color: isDark ? colors.dark.text : colors.text,
    },
    notesBox: {
      marginTop: spacing.sm,
      padding: spacing.sm,
      backgroundColor: isDark
        ? 'rgba(99, 102, 241, 0.1)'
        : 'rgba(99, 102, 241, 0.05)',
      borderRadius: borderRadius.sm,
    },
    notesLabel: {
      ...typography.caption,
      fontWeight: '600',
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    notesText: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.text : colors.text,
      fontStyle: 'italic',
    },
  });
