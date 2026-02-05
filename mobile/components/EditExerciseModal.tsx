/**
 * Edit Exercise Modal
 * Edit exercise parameters
 */

import { useState, useEffect } from 'react';
import {
  View,
  StyleSheet,
  useColorScheme,
  Alert,
} from 'react-native';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { workoutsApi } from '../lib/api/workouts';
import type { WorkoutExerciseResponse } from '../lib/types/programme';
import { colors, spacing } from '../theme';
import { Modal, Button, Input, NumberInput } from './ui';

interface EditExerciseModalProps {
  visible: boolean;
  onClose: () => void;
  exercise: WorkoutExerciseResponse;
  programmeId: string;
}

export default function EditExerciseModal({
  visible,
  onClose,
  exercise,
  programmeId,
}: EditExerciseModalProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const [formData, setFormData] = useState({
    targetSets: exercise.targetSets,
    targetReps: exercise.targetReps,
    targetWeight: exercise.targetWeight || 0,
    restSeconds: exercise.restSeconds || 90,
    targetRpe: exercise.targetRpe,
    notes: exercise.notes || '',
  });

  useEffect(() => {
    if (visible) {
      setFormData({
        targetSets: exercise.targetSets,
        targetReps: exercise.targetReps,
        targetWeight: exercise.targetWeight || 0,
        restSeconds: exercise.restSeconds || 90,
        targetRpe: exercise.targetRpe,
        notes: exercise.notes || '',
      });
    }
  }, [exercise, visible]);

  const updateMutation = useMutation({
    mutationFn: () =>
      workoutsApi.updateExercise(exercise.id, {
        targetSets: formData.targetSets,
        targetReps: formData.targetReps,
        targetWeight: formData.targetWeight || undefined,
        restSeconds: formData.restSeconds || undefined,
        targetRpe: formData.targetRpe || undefined,
        notes: formData.notes || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programme', programmeId] });
      onClose();
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to update exercise');
    },
  });

  return (
    <Modal
      visible={visible}
      onClose={onClose}
      title={`Edit: ${exercise.exerciseName}`}
      footer={
        <View style={styles.footer}>
          <Button title="Cancel" onPress={onClose} variant="ghost" />
          <Button
            title={updateMutation.isPending ? 'Saving...' : 'Save Changes'}
            onPress={() => updateMutation.mutate()}
            variant="primary"
            loading={updateMutation.isPending}
          />
        </View>
      }
    >
      <View style={styles.formGroup}>
        <View style={styles.numberInputRow}>
          <NumberInput
            label="Sets"
            value={formData.targetSets}
            onChange={(value) =>
              setFormData({ ...formData, targetSets: value })
            }
            min={1}
            max={20}
          />
          <NumberInput
            label="Reps"
            value={formData.targetReps}
            onChange={(value) =>
              setFormData({ ...formData, targetReps: value })
            }
            min={1}
            max={100}
          />
          <NumberInput
            label="Weight"
            value={formData.targetWeight}
            onChange={(value) =>
              setFormData({ ...formData, targetWeight: value })
            }
            min={0}
            max={500}
            suffix="kg"
          />
        </View>

        <View style={styles.numberInputRow}>
          <NumberInput
            label="Rest (sec)"
            value={formData.restSeconds}
            onChange={(value) =>
              setFormData({ ...formData, restSeconds: value })
            }
            min={0}
            max={600}
            step={15}
            suffix="s"
          />
          <NumberInput
            label="RPE"
            value={formData.targetRpe}
            onChange={(value) =>
              setFormData({ ...formData, targetRpe: value })
            }
            min={1}
            max={10}
          />
        </View>

        <Input
          label="Notes (optional)"
          placeholder="Any special instructions..."
          value={formData.notes}
          onChangeText={(notes) => setFormData({ ...formData, notes })}
          multiline
        />
      </View>
    </Modal>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    formGroup: {
      gap: spacing.md,
    },
    footer: {
      flexDirection: 'row',
      justifyContent: 'flex-end',
      gap: spacing.sm,
    },
    numberInputRow: {
      flexDirection: 'row',
      gap: spacing.sm,
    },
  });
