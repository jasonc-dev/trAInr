/**
 * SetRow Component
 * Displays a single set with controls for reps, weight, RPE, and completion
 */

import { View, Text, Pressable, useColorScheme } from "react-native";
import { NumberInput } from "../ui/NumberInput";
import { Select, type SelectOption } from "../ui/Select";
import { createSetRowStyles } from "./styles";
import type { Intensity } from "../../lib/types/programme";

const RPE_OPTIONS: SelectOption[] = [
  { label: "-", value: "" },
  ...Array.from({ length: 10 }, (_, i) => ({
    label: String(i + 1),
    value: String(i + 1),
  })),
];

interface SetRowProps {
  setNumber: number;
  reps: number | null;
  weight: number | null;
  intensity: Intensity | null;
  isCompleted: boolean;
  onRepsChange: (value: number | null) => void;
  onWeightChange: (value: number | null) => void;
  onIntensityChange: (value: Intensity | null) => void;
  onComplete: () => void;
  isCompleting?: boolean;
}

export default function SetRow({
  setNumber,
  reps,
  weight,
  intensity,
  isCompleted,
  onRepsChange,
  onWeightChange,
  onIntensityChange,
  onComplete,
  isCompleting = false,
}: SetRowProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createSetRowStyles(isDark);

  return (
    <View style={[styles.container, isCompleted && styles.containerCompleted]}>
      <Text style={styles.setNumber}>{setNumber}</Text>

      <View style={styles.inputColumn}>
        <NumberInput
          type="reps"
          value={reps}
          onChange={onRepsChange}
          disabled={isCompleted}
        />
      </View>

      <View style={styles.inputColumn}>
        <NumberInput
          type="weight"
          value={weight}
          onChange={onWeightChange}
          disabled={isCompleted}
        />
      </View>

      <View style={styles.inputColumn}>
        <Select
          options={RPE_OPTIONS}
          value={intensity !== null ? intensity.toString() : ""}
          onChange={(value) =>
            onIntensityChange(value ? (parseInt(value) as Intensity) : null)
          }
          disabled={isCompleted}
        />
      </View>

      <Pressable
        style={[
          styles.completeButton,
          isCompleted && styles.completeButtonCompleted,
        ]}
        onPress={onComplete}
        disabled={isCompleted || isCompleting}
      >
        {isCompleted ? (
          <Text style={styles.completeButtonTextCompleted}>✓</Text>
        ) : isCompleting ? (
          <Text style={styles.completeButtonText}>...</Text>
        ) : null}
      </Pressable>
    </View>
  );
}
