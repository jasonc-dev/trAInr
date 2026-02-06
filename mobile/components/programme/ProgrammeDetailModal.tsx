/**
 * Programme Detail Modal
 * Shows full programme details with weeks, days, and exercises
 */

import { useState } from "react";
import {
  View,
  Text,
  ScrollView,
  useColorScheme,
  Pressable,
  ActivityIndicator,
  Alert,
  TouchableOpacity,
} from "react-native";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { programmesApi } from "../../lib/api/programmes";
import { workoutsApi } from "../../lib/api/workouts";
import type { Programme, WorkoutDayResponse } from "../../lib/types/programme";
import { colors } from "../../theme";
import { Modal, Badge, Button } from "../ui";
import AddEditDayModal from "./AddEditDayModal";
import ExerciseListModal from "./ExerciseListModal";
import { createStyles } from "./Styles";

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
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const [selectedDayId, setSelectedDayId] = useState<string | null>(null);

  const [selectedWeekIndex, setSelectedWeekIndex] = useState(0);
  const [showAddDayModal, setShowAddDayModal] = useState(false);
  const [showEditDayModal, setShowEditDayModal] = useState(false);
  const [selectedDay, setSelectedDay] = useState<WorkoutDayResponse | null>(
    null,
  );
  const [showExerciseListModal, setShowExerciseListModal] = useState(false);

  // Reset child modal states when parent modal closes
  const handleClose = () => {
    setShowAddDayModal(false);
    setShowEditDayModal(false);
    setShowExerciseListModal(false);
    setSelectedDay(null);
    onClose();
  };

  // Query programme details
  const {
    data: programme,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["programme", programmeId],
    queryFn: () => programmesApi.getById(programmeId),
    enabled: visible && !!programmeId,
  });

  // Mutation to add first week
  const addWeekMutation = useMutation({
    mutationFn: () =>
      programmesApi.addWeek(programmeId, {
        weekNumber: 1,
        notes: "",
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to add week");
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
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to copy week content");
    },
  });

  // Mutation to delete day
  const deleteDayMutation = useMutation({
    mutationFn: (dayId: string) => workoutsApi.deleteWorkoutDay(dayId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to delete day");
    },
  });

  const handleEditDay = (day: WorkoutDayResponse) => {
    console.log("Edit day pressed:", day.id, day.name);
    setSelectedDay(day);
    setShowEditDayModal(true);
  };

  const handleDeleteDay = (day: WorkoutDayResponse) => {
    console.log("Delete day pressed:", day.id, day.name);
    Alert.alert(
      "Delete Workout Day",
      `Are you sure you want to delete "${day.name}"? All exercises will be removed.`,
      [
        { text: "Cancel", style: "cancel" },
        {
          text: "Delete",
          style: "destructive",
          onPress: () => deleteDayMutation.mutate(day.id),
        },
      ],
    );
  };

  const handleViewExercises = (day: WorkoutDayResponse) => {
    console.log("View exercises pressed:", day.id, day.name);
    setSelectedDay(day);
    setSelectedDayId(day.id);
    setShowExerciseListModal(true);
  };

  const handleCopyWeek = () => {
    if (!programme || selectedWeekIndex === 0) return;

    const currentWeek = programme.weeks[selectedWeekIndex];
    const previousWeek = programme.weeks[selectedWeekIndex - 1];

    if (!previousWeek || !currentWeek) return;

    // Check if previous week has exercises
    const prevHasExercises = previousWeek.workoutDays.some(
      (day) => day.exercises && day.exercises.length > 0,
    );

    if (!prevHasExercises) {
      Alert.alert(
        "No Exercises",
        "The previous week has no exercises to copy.",
      );
      return;
    }

    Alert.alert(
      "Copy Week Content",
      `Copy all exercises from Week ${previousWeek.weekNumber} to Week ${currentWeek.weekNumber}?`,
      [
        { text: "Cancel", style: "cancel" },
        {
          text: "Copy",
          onPress: () =>
            copyWeekMutation.mutate({
              sourceWeekId: previousWeek.id,
              targetWeekId: currentWeek.id,
            }),
        },
      ],
    );
  };

  if (isLoading) {
    return (
      <Modal visible={visible} onClose={handleClose} title="Loading...">
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={colors.primary} />
        </View>
      </Modal>
    );
  }

  if (error || !programme) {
    return (
      <Modal visible={visible} onClose={handleClose} title="Error">
        <View style={styles.errorContainer}>
          <Text style={styles.errorText}>Failed to load programme</Text>
          <Button title="Close" onPress={handleClose} variant="primary" />
        </View>
      </Modal>
    );
  }

  const currentWeek = programme.weeks[selectedWeekIndex];
  const previousWeek =
    selectedWeekIndex > 0 ? programme.weeks[selectedWeekIndex - 1] : null;
  const prevWeekHasExercises = previousWeek?.workoutDays.some(
    (day) => day.exercises && day.exercises.length > 0,
  );
  const dayForExerciseList = currentWeek?.workoutDays.find(
    (day) => day.id === selectedDayId,
  );

  // Hide parent modal when child modals are open
  const parentModalVisible =
    visible && !showAddDayModal && !showEditDayModal && !showExerciseListModal;

  return (
    <>
      <Modal
        visible={parentModalVisible}
        onClose={handleClose}
        title={programme.name}
      >
        {/* Programme Info */}
        <View style={styles.programmeInfo}>
          {programme.description && (
            <Text style={styles.programmeDescription}>
              {programme.description}
            </Text>
          )}
          {programme.isActive && (
            <Badge variant="primary" style={styles.programmeBadge}>
              Active
            </Badge>
          )}
        </View>

        {/* No weeks state */}
        {programme.weeks.length === 0 ? (
          <View style={styles.programmeEmptyState}>
            <Text style={styles.programmeEmptyStateIcon}>📅</Text>
            <Text style={styles.programmeEmptyStateTitle}>
              No Weeks Configured
            </Text>
            <Text style={styles.programmeEmptyStateText}>
              Add weeks to start planning your workouts
            </Text>
            <Button
              title={addWeekMutation.isPending ? "Adding..." : "Add First Week"}
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
                        selectedWeekIndex === index && styles.weekTabTextActive,
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
              <View style={styles.programmeEmptyState}>
                <Text style={styles.programmeEmptyStateIcon}>📅</Text>
                <Text style={styles.programmeEmptyStateTitle}>
                  No Workout Days
                </Text>
                <Text style={styles.programmeEmptyStateText}>
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
                      new Date(b.scheduledDate).getTime(),
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
                              "en-US",
                              { weekday: "short" },
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
                          {day.exercises.length !== 1 ? "s" : ""}
                        </Text>
                      )}

                      <View style={styles.dayCardActions}>
                        {!day.isRestDay && (
                          <TouchableOpacity
                            style={styles.dayActionButton}
                            onPress={() => handleViewExercises(day)}
                            activeOpacity={0.7}
                          >
                            <View style={styles.actionButtonPrimary}>
                              <Text style={styles.actionButtonTextPrimary}>
                                Exercises
                              </Text>
                            </View>
                          </TouchableOpacity>
                        )}
                        <TouchableOpacity
                          style={styles.dayActionButton}
                          onPress={() => handleEditDay(day)}
                          activeOpacity={0.7}
                        >
                          <View style={styles.actionButtonSecondary}>
                            <Text style={styles.actionButtonTextSecondary}>
                              Edit
                            </Text>
                          </View>
                        </TouchableOpacity>
                        <TouchableOpacity
                          style={styles.dayActionButton}
                          onPress={() => handleDeleteDay(day)}
                          activeOpacity={0.7}
                        >
                          <View style={styles.actionButtonGhost}>
                            <Text style={styles.actionButtonTextGhost}>
                              Delete
                            </Text>
                          </View>
                        </TouchableOpacity>
                      </View>
                    </View>
                  ))}
              </View>
            )}
          </>
        )}
      </Modal>

      {/* Add/Edit Day Modal */}
      {currentWeek && visible && (
        <>
          <AddEditDayModal
            key="add-day"
            visible={showAddDayModal}
            onClose={() => setShowAddDayModal(false)}
            weekId={currentWeek.id}
            programmeId={programmeId}
          />
          {selectedDay && (
            <AddEditDayModal
              key="edit-day"
              visible={showEditDayModal}
              onClose={() => {
                setShowEditDayModal(false);
                setSelectedDay(null);
              }}
              weekId={currentWeek.id}
              programmeId={programmeId}
              day={selectedDay!}
            />
          )}
        </>
      )}

      {/* Exercise List Modal */}
      {dayForExerciseList && visible && (
        <ExerciseListModal
          visible={showExerciseListModal}
          onClose={() => {
            setShowExerciseListModal(false);
            setSelectedDay(null);
            setSelectedDayId(null);
          }}
          day={dayForExerciseList}
          programmeId={programmeId}
        />
      )}
    </>
  );
}
