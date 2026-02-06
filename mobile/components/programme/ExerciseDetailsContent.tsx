import { useColorScheme, View, Text, ActivityIndicator } from "react-native";
import { Badge } from "../ui";
import { colors } from "../../theme";
import { useQuery } from "@tanstack/react-query";
import { exercisesApi } from "../../lib/api/exercises";
import type { WorkoutExerciseResponse } from "../../lib/types/programme";
 import {
  getExerciseTypeLabel,
  getMuscleGroupLabel,
} from "../../lib/utils/exerciseLabels";
import { createStyles } from "./Styles";

interface ExerciseDetailsContentProps {
  exercise: WorkoutExerciseResponse;
}

export default function ExerciseDetailsContent({
  exercise,
}: ExerciseDetailsContentProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  const { data: details, isLoading } = useQuery({
    queryKey: ["exercise", exercise.exerciseDefinitionId],
    queryFn: () => exercisesApi.getExerciseById(exercise.exerciseDefinitionId),
  });

  if (isLoading) {
    return (
      <View style={styles.detailsLoading}>
        <ActivityIndicator size="large" color={colors.primary} />
      </View>
    );
  }

  if (!details) {
    return (
      <View style={styles.detailsError}>
        <Text style={styles.detailsErrorText}>
          Failed to load exercise details
        </Text>
      </View>
    );
  }

  return (
    <View style={styles.detailsContent}>
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
          <Badge>{getExerciseTypeLabel(details.type)}</Badge>
          <Badge>{getMuscleGroupLabel(details.primaryMuscleGroup)}</Badge>
          {details.secondaryMuscleGroup != null && (
            <Badge variant="default">
              {getMuscleGroupLabel(details.secondaryMuscleGroup)}
            </Badge>
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
              <Text style={styles.targetValue}>{exercise.targetWeight}kg</Text>
            </View>
          )}
          {exercise.restSeconds && (
            <View style={styles.targetItem}>
              <Text style={styles.targetLabel}>Rest</Text>
              <Text style={styles.targetValue}>
                {Math.floor(exercise.restSeconds / 60)}:
                {String(exercise.restSeconds % 60).padStart(2, "0")}
              </Text>
            </View>
          )}
          {exercise.targetRpe && (
            <View style={styles.targetItem}>
              <Text style={styles.targetLabel}>RPE</Text>
              <Text style={styles.targetValue}>{exercise.targetRpe}/10</Text>
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
  );
}
