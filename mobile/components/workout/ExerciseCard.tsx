/**
 * ExerciseCard Component
 * Expandable card showing exercise details, sets, and controls
 */

import { View, Text, Pressable, useColorScheme } from "react-native";
import { Badge } from "../ui/Badge";
import { createExerciseCardStyles } from "./styles";
import { Button } from "../ui/Button";
import type {
  WorkoutExerciseResponse,
  Intensity,
} from "../../lib/types/programme";
import SetRow from "./SetRow";

interface ExerciseCardProps {
  exercise: WorkoutExerciseResponse;
  isExpanded: boolean;
  onToggleExpand: () => void;
  onAddSet: () => void;
  localSetData: Record<
    string,
    { reps?: number; weight?: number; intensity?: Intensity }
  >;
  onUpdateLocalSet: (
    setId: string,
    field: "reps" | "weight" | "intensity",
    value: any
  ) => void;
  onCompleteSet: (setId: string) => void;
  completingSetId: string | null;
  supersetLabel?: string;
  isInSuperset?: boolean;
  supersetPosition?: "first" | "middle" | "last" | "single";
}

export default function ExerciseCard({
  exercise,
  isExpanded,
  onToggleExpand,
  onAddSet,
  localSetData,
  onUpdateLocalSet,
  onCompleteSet,
  completingSetId,
  supersetLabel,
  isInSuperset = false,
  supersetPosition = "single",
}: ExerciseCardProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createExerciseCardStyles(
    isDark,
    isExpanded,
    isInSuperset,
    supersetPosition
  );

  const completedSetsCount = exercise.sets.filter((s) => s.isCompleted).length;

  return (
    <View style={styles.card}>
      <Pressable onPress={onToggleExpand} style={styles.header}>
        <View style={styles.headerContent}>
          <View style={styles.titleRow}>
            <Text style={styles.title} numberOfLines={1}>
              {exercise.exerciseName}
            </Text>
            <View style={styles.badges}>
              {supersetLabel && (
                <Badge variant="primary">{supersetLabel}</Badge>
              )}
              <Badge
                variant={
                  completedSetsCount === exercise.targetSets
                    ? "success"
                    : "default"
                }
              >
                {completedSetsCount}/{exercise.targetSets}
              </Badge>
            </View>
          </View>
          <Text style={styles.subtitle}>
            {exercise.targetSets} sets × {exercise.targetReps} reps
            {exercise.targetWeight ? ` @ ${exercise.targetWeight}kg` : ""}
          </Text>
        </View>
        <Text style={styles.expandIcon}>{isExpanded ? "−" : "+"}</Text>
      </Pressable>

      {isExpanded && (
        <View style={styles.expandedContent}>
          <View style={styles.setHeader}>
            <Text style={styles.setHeaderText}>Set</Text>
            <Text style={styles.setHeaderText}>Reps</Text>
            <Text style={styles.setHeaderText}>Weight</Text>
            <Text style={styles.setHeaderText}>RPE</Text>
            <View style={styles.setHeaderSpacer} />
          </View>

          {exercise.sets
            .sort((a, b) => a.setNumber - b.setNumber)
            .map((set) => (
              <SetRow
                key={set.id}
                setNumber={set.setNumber}
                reps={localSetData[set.id]?.reps ?? set.reps ?? null}
                weight={localSetData[set.id]?.weight ?? set.weight ?? null}
                intensity={
                  localSetData[set.id]?.intensity ?? set.intensity ?? null
                }
                isCompleted={set.isCompleted}
                onRepsChange={(value) =>
                  onUpdateLocalSet(set.id, "reps", value)
                }
                onWeightChange={(value) =>
                  onUpdateLocalSet(set.id, "weight", value)
                }
                onIntensityChange={(value) =>
                  onUpdateLocalSet(set.id, "intensity", value)
                }
                onComplete={() => onCompleteSet(set.id)}
                isCompleting={completingSetId === set.id}
              />
            ))}

          <Button
            title="+ Add Set"
            onPress={onAddSet}
            variant="ghost"
            size="sm"
            fullWidth
          />
        </View>
      )}
    </View>
  );
}
