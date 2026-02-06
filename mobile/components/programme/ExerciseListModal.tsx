/**
 * Exercise List Modal
 * Shows and manages all exercises for a workout day
 * Uses single modal with view mode switching for list/details/edit
 */

import { useState } from "react";
import { View, Text, useColorScheme, Pressable, Alert } from "react-native";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutsApi } from "../../lib/api/workouts";
import type {
  WorkoutDayResponse,
  WorkoutExerciseResponse,
} from "../../lib/types/programme";
import { Modal, Button, Badge } from "../ui";
import { createStyles } from "./Styles";
import AddExerciseModal from "./AddExerciseModal";
import EditExerciseForm from "./EditExerciseForm";
import ExerciseDetailsContent from "./ExerciseDetailsContent";
import AddExerciseForm from "./AddExerciseForm";

type ViewMode = "list" | "details" | "edit" | "add";

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
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const [showAddExerciseForm, setShowAddExerciseForm] = useState(false);
  const [selectedExercise, setSelectedExercise] =
    useState<WorkoutExerciseResponse | null>(null);
  const [selectedExercises, setSelectedExercises] = useState<Set<string>>(
    new Set(),
  );
  const [viewMode, setViewMode] = useState<ViewMode>("list");

  // Reset state when modal closes
  const handleModalClose = () => {
    setViewMode("list");
    setSelectedExercise(null);
    onClose();
  };

  // Mutations
  const removeMutation = useMutation({
    mutationFn: (exerciseId: string) => workoutsApi.removeExercise(exerciseId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to remove exercise");
    },
  });

  const reorderMutation = useMutation({
    mutationFn: ({ exerciseIds }: { exerciseIds: string[] }) =>
      workoutsApi.reorderExercises(day.id, exerciseIds),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to reorder exercises");
    },
  });

  const groupSupersetMutation = useMutation({
    mutationFn: ({ exerciseIds }: { exerciseIds: string[] }) =>
      workoutsApi.groupSuperset(day.id, {
        exerciseIds,
        supersetRestSeconds: 120,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
      setSelectedExercises(new Set());
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to create superset");
    },
  });

  const ungroupSupersetMutation = useMutation({
    mutationFn: (supersetGroupId: string) =>
      workoutsApi.ungroupSuperset(supersetGroupId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to ungroup superset");
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
    setShowAddExerciseForm(true);
    setSelectedExercise(exercise);
    setViewMode("edit");
  };

  const handleViewDetails = (exercise: WorkoutExerciseResponse) => {
    setSelectedExercise(exercise);
    setViewMode("details");
  };

  const handleBackToList = () => {
    setViewMode("list");
    setSelectedExercise(null);
  };

  const handleAddExercise = () => {
    setShowAddExerciseForm(true);
    setViewMode("add");
  };

  const handleRemove = (exercise: WorkoutExerciseResponse) => {
    Alert.alert(
      "Remove Exercise",
      `Remove "${exercise.exerciseName}" from this workout?`,
      [
        { text: "Cancel", style: "cancel" },
        {
          text: "Remove",
          style: "destructive",
          onPress: () => removeMutation.mutate(exercise.id),
        },
      ],
    );
  };

  const handleMoveUp = (exercise: WorkoutExerciseResponse) => {
    const sortedExercises = [...day.exercises].sort(
      (a, b) => a.orderIndex - b.orderIndex,
    );
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
    const sortedExercises = [...day.exercises].sort(
      (a, b) => a.orderIndex - b.orderIndex,
    );
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
      Alert.alert("Error", "Select at least 2 exercises to create a superset");
      return;
    }

    groupSupersetMutation.mutate({
      exerciseIds: Array.from(selectedExercises),
    });
  };

  const handleUngroup = (supersetGroupId: string) => {
    Alert.alert(
      "Ungroup Superset",
      "Remove these exercises from the superset?",
      [
        { text: "Cancel", style: "cancel" },
        {
          text: "Ungroup",
          onPress: () => ungroupSupersetMutation.mutate(supersetGroupId),
        },
      ],
    );
  };

  // Get superset label
  const getSupersetLabel = (exercise: WorkoutExerciseResponse): string => {
    if (!exercise.supersetGroupId) return "";

    const supersetCount = day.exercises.filter(
      (e) => e.supersetGroupId === exercise.supersetGroupId,
    ).length;

    if (supersetCount === 2) return "Superset";
    if (supersetCount === 3) return "Triset";
    return "Giant Set";
  };

  const sortedExercises = [...day.exercises].sort(
    (a, b) => a.orderIndex - b.orderIndex,
  );

  // Determine modal title and footer based on viewMode
  const getModalTitle = (): string => {
    switch (viewMode) {
      case "edit":
        return `Edit: ${selectedExercise?.exerciseName || "Exercise"}`;
      case "details":
        return selectedExercise?.exerciseName || "Exercise Details";
      default:
        return `${day.name} - Exercises`;
    }
  };

  const getModalFooter = (): React.ReactNode => {
    // Edit view: footer is handled by EditExerciseForm component
    if (viewMode === "edit") {
      return null;
    }

    // Add view: footer is handled by AddExerciseForm (its own Add Exercise button)
    if (viewMode === "add") {
      return null;
    }

    if (viewMode === "details" && selectedExercise) {
      return (
        <View style={styles.footer}>
          <Button
            title="Remove"
            onPress={() => {
              handleBackToList();
              handleRemove(selectedExercise);
            }}
            variant="danger"
          />
          <Button
            title="Edit"
            onPress={() => handleEdit(selectedExercise)}
            variant="primary"
          />
        </View>
      );
    }

    return (
      <View style={styles.footer}>
        {selectedExercises.size >= 2 ? (
          <Button
            title={`Group as ${
              selectedExercises.size === 2
                ? "Superset"
                : selectedExercises.size === 3
                  ? "Triset"
                  : "Giant Set"
            } (${selectedExercises.size})`}
            onPress={handleGroupSuperset}
            variant="primary"
            loading={groupSupersetMutation.isPending}
          />
        ) : (
          <Button
            title="+ Add Exercise"
            onPress={() => handleAddExercise()}
            variant="primary"
          />
        )}
      </View>
    );
  };

  const getModalContent = (): React.ReactNode => {
    if (viewMode === "add" && showAddExerciseForm) {
      return (
        <AddExerciseForm
          visible={showAddExerciseForm}
          onClose={() => setShowAddExerciseForm(false)}
          dayId={day.id}
          programmeId={programmeId}
        />
      );
    }

    if (viewMode === "edit" && selectedExercise) {
      return (
        <EditExerciseForm
          exercise={selectedExercise}
          programmeId={programmeId}
          onSave={handleBackToList}
          onCancel={handleBackToList}
        />
      );
    }

    if (viewMode === "details" && selectedExercise) {
      return <ExerciseDetailsContent exercise={selectedExercise} />;
    }

    // List view (default)
    if (sortedExercises.length === 0) {
      return (
        <View style={styles.listEmptyState}>
          <Text style={styles.listEmptyStateIcon}>💪</Text>
          <Text style={styles.listEmptyStateTitle}>No Exercises</Text>
          <Text style={styles.listEmptyStateText}>
            Add exercises to this workout day
          </Text>
        </View>
      );
    }

    return (
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
                  style={styles.listCheckbox}
                  onPress={() => handleToggleSelect(exercise.id)}
                >
                  <View
                    style={[
                      styles.listCheckboxBox,
                      isSelected && styles.listCheckboxBoxChecked,
                    ]}
                  >
                    {isSelected && (
                      <Text style={styles.listCheckboxCheck}>✓</Text>
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
                        exercise.restSeconds % 60,
                      ).padStart(2, "0")} rest`}
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
                  style={[
                    styles.listActionButton,
                    isFirst && styles.listActionButtonDisabled,
                  ]}
                  onPress={() => handleMoveUp(exercise)}
                  disabled={isFirst}
                >
                  <Text style={styles.listActionButtonText}>↑</Text>
                </Pressable>
                <Pressable
                  style={[
                    styles.listActionButton,
                    isLast && styles.listActionButtonDisabled,
                  ]}
                  onPress={() => handleMoveDown(exercise)}
                  disabled={isLast}
                >
                  <Text style={styles.listActionButtonText}>↓</Text>
                </Pressable>
                <Pressable
                  style={styles.listActionButton}
                  onPress={() => handleRemove(exercise)}
                >
                  <Text style={styles.listActionButtonText}>×</Text>
                </Pressable>
              </View>
            </View>
          );
        })}
      </View>
    );
  };

  return (
    <>
      <Modal
        visible={visible}
        onClose={handleModalClose}
        title={getModalTitle()}
        footer={getModalFooter()}
      >
        {getModalContent()}
      </Modal>
    </>
  );
}
