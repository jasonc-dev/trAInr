/**
 * Add Exercise Modal
 * Search and add exercises to a workout day
 */

import { useState, useEffect } from "react";
import {
  View,
  Text,
  useColorScheme,
  Pressable,
  ScrollView,
  ActivityIndicator,
  Alert,
} from "react-native";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutsApi } from "../../lib/api/workouts";
import { exercisesApi } from "../../lib/api/exercises";
import type { ExerciseSummary } from "../../lib/types/programme";
import { colors } from "../../theme";
import { Input, NumberInput, Button } from "../ui";
import { createStyles } from "./Styles";

interface AddExerciseFormProps {
  visible: boolean;
  onClose: () => void;
  dayId: string;
  programmeId: string;
}

export default function AddExerciseForm({
  visible,
  onClose,
  dayId,
  programmeId,
}: AddExerciseFormProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState<ExerciseSummary[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [selectedExerciseId, setSelectedExerciseId] = useState<number | null>(
    null,
  );

  const [formData, setFormData] = useState({
    targetSets: 3,
    targetReps: 10,
    targetWeight: 0,
    restSeconds: 90,
    targetRpe: null as number | null,
    notes: "",
  });

  // Search exercises with debounce
  useEffect(() => {
    if (!searchQuery || searchQuery.length < 2) {
      setSearchResults([]);
      return;
    }

    const search = async () => {
      setIsSearching(true);
      try {
        const results = await exercisesApi.searchExercises(searchQuery);
        setSearchResults(results);
      } catch (error) {
        console.error("Search error:", error);
      } finally {
        setIsSearching(false);
      }
    };

    const timer = setTimeout(search, 300);
    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Reset form when modal closes
  useEffect(() => {
    if (!visible) {
      setSearchQuery("");
      setSearchResults([]);
      setSelectedExerciseId(null);
      setFormData({
        targetSets: 3,
        targetReps: 10,
        targetWeight: 0,
        restSeconds: 90,
        targetRpe: null,
        notes: "",
      });
    }
  }, [visible]);

  // Add exercise mutation
  const addMutation = useMutation({
    mutationFn: (exerciseDefinitionId: number) => {
      const orderIndex = 0; // API will handle proper ordering
      return workoutsApi.addExercise(dayId, {
        exerciseDefinitionId,
        orderIndex,
        targetSets: formData.targetSets,
        targetReps: formData.targetReps,
        targetWeight: formData.targetWeight || undefined,
        restSeconds: formData.restSeconds || undefined,
        targetRpe: formData.targetRpe || undefined,
        notes: formData.notes || undefined,
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
      onClose();
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to add exercise");
    },
  });

  const handleAdd = () => {
    if (!selectedExerciseId) {
      Alert.alert("Validation Error", "Please select an exercise");
      return;
    }

    addMutation.mutate(selectedExerciseId);
  };

  return (
    <View>
      <View style={styles.addExerciseContent}>
        {/* Search */}
        <Input
          label="Search Exercises"
          placeholder="Type to search..."
          value={searchQuery}
          onChangeText={setSearchQuery}
        />

        {/* Search Results */}
        {isSearching ? (
          <View style={styles.searchState}>
            <ActivityIndicator size="small" color={colors.primary} />
          </View>
        ) : searchQuery.length >= 2 ? (
          searchResults.length > 0 ? (
            <ScrollView style={styles.searchResults}>
              {searchResults.map((exercise) => (
                <Pressable
                  key={exercise.id}
                  style={[
                    styles.exerciseResult,
                    selectedExerciseId === exercise.id &&
                      styles.exerciseResultSelected,
                  ]}
                  onPress={() => setSelectedExerciseId(exercise.id)}
                >
                  <View style={styles.exerciseResultInfo}>
                    <Text style={styles.exerciseResultName}>
                      {exercise.name}
                    </Text>
                    <Text style={styles.exerciseResultMeta}>
                      Type: {exercise.type} • Primary:{" "}
                      {exercise.primaryMuscleGroup}
                    </Text>
                  </View>
                  {selectedExerciseId === exercise.id && (
                    <Text style={styles.checkmark}>✓</Text>
                  )}
                </Pressable>
              ))}
            </ScrollView>
          ) : (
            <View style={styles.searchState}>
              <Text style={styles.searchStateText}>No exercises found</Text>
            </View>
          )
        ) : (
          <View style={styles.searchState}>
            <Text style={styles.searchStateText}>
              Type at least 2 characters to search
            </Text>
          </View>
        )}

        {/* Configuration */}
        {selectedExerciseId && (
          <View style={styles.configSection}>
            <Text style={styles.configTitle}>Exercise Configuration</Text>

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

            <Button
              title="Add Exercise"
              onPress={handleAdd}
              variant="primary"
              disabled={addMutation.isPending}
            />
          </View>
        )}
      </View>
    </View>
  );
}
