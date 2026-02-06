import { Alert, Text, useColorScheme, View } from "react-native";
import { WorkoutExerciseResponse } from "../../lib/types/programme";
import { useEffect, useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutsApi } from "../../lib/api/workouts";
import {
  Input,
  ExerciseNumberInput,
  Button,
  Select,
  type SelectOption,
} from "../ui";
import { createStyles } from "./Styles";

const RPE_OPTIONS: SelectOption[] = [
  { label: "Not set", value: "" },
  ...Array.from({ length: 10 }, (_, i) => ({
    label: String(i + 1),
    value: String(i + 1),
  })),
];

interface EditExerciseFormProps {
  exercise: WorkoutExerciseResponse;
  programmeId: string;
  onSave: () => void;
  onCancel: () => void;
  includeFooter?: boolean;
}

export default function EditExerciseForm({
  exercise,
  programmeId,
  onSave,
  onCancel,
  includeFooter = true,
}: EditExerciseFormProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const [formData, setFormData] = useState({
    targetSets: exercise.targetSets,
    targetReps: exercise.targetReps,
    targetWeight: exercise.targetWeight || 0,
    restSeconds: exercise.restSeconds || 90,
    targetRpe: exercise.targetRpe,
    notes: exercise.notes || "",
  });

  useEffect(() => {
    setFormData({
      targetSets: exercise.targetSets,
      targetReps: exercise.targetReps,
      targetWeight: exercise.targetWeight || 0,
      restSeconds: exercise.restSeconds || 90,
      targetRpe: exercise.targetRpe,
      notes: exercise.notes || "",
    });
  }, [exercise]);

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
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
      onSave();
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to update exercise");
    },
  });

  return (
    <View style={styles.editFormContainer}>
      <View style={styles.formGroup}>
        <View style={styles.numberInputRow}>
          <ExerciseNumberInput
            label="Sets"
            value={formData.targetSets}
            onChange={(value) =>
              setFormData({ ...formData, targetSets: value ?? 1 })
            }
            min={1}
            max={20}
          />
          <ExerciseNumberInput
            label="Reps"
            value={formData.targetReps}
            onChange={(value) =>
              setFormData({ ...formData, targetReps: value ?? 1 })
            }
            min={1}
            max={100}
          />
          <ExerciseNumberInput
            label="Weight"
            value={formData.targetWeight}
            onChange={(value) =>
              setFormData({ ...formData, targetWeight: value ?? 0 })
            }
            min={0}
            max={500}
            suffix="kg"
          />
        </View>

        <View style={styles.numberInputRow}>
          <ExerciseNumberInput
            label="Rest (sec)"
            value={formData.restSeconds}
            onChange={(value) =>
              setFormData({ ...formData, restSeconds: value ?? 90 })
            }
            min={0}
            max={600}
            step={15}
          />

          <Select
            label="RPE (1-10)"
            value={formData.targetRpe != null ? String(formData.targetRpe) : ""}
            options={RPE_OPTIONS}
            onChange={(value) =>
              setFormData({
                ...formData,
                targetRpe: value === "" ? undefined : parseInt(value, 10),
              })
            }
            placeholder="Not set"
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

      {includeFooter && (
        <View style={styles.editFormFooter}>
          <Button title="Cancel" onPress={onCancel} variant="ghost" />
          <Button
            title={updateMutation.isPending ? "Saving..." : "Save Changes"}
            onPress={() => updateMutation.mutate()}
            variant="primary"
            loading={updateMutation.isPending}
          />
        </View>
      )}
    </View>
  );
}
