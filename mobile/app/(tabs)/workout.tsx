/**
 * Workout Screen
 * Displays workout timeline with past, present, and future workouts
 */

import { useState, useEffect } from "react";
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  useColorScheme,
  ActivityIndicator,
  RefreshControl,
  Alert,
} from "react-native";
import { Link } from "expo-router";
import { colors, spacing, typography, borderRadius } from "../../theme";
import { useAuthStore } from "../../stores/authStore";
import { programmesApi } from "../../lib/api/programmes";
import { workoutsApi } from "../../lib/api/workouts";
import { Button, CollapsibleSection } from "../../components/ui";
import {
  ExerciseCard,
  WorkoutDayListItem,
} from "../../components/workout/index";
import type {
  Programme,
  WorkoutDayResponse,
  WorkoutExerciseResponse,
  Intensity,
} from "../../lib/types/programme";

interface LocalSetData {
  reps?: number;
  weight?: number;
  intensity?: Intensity;
}

export default function WorkoutScreen() {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);
  const { user } = useAuthStore();

  // Programme and workout data
  const [activeProgramme, setActiveProgramme] = useState<Programme | null>(
    null
  );
  const [allWorkouts, setAllWorkouts] = useState<WorkoutDayResponse[]>([]);
  const [pastWorkouts, setPastWorkouts] = useState<WorkoutDayResponse[]>([]);
  const [todayWorkout, setTodayWorkout] = useState<WorkoutDayResponse | null>(
    null
  );
  const [futureWorkouts, setFutureWorkouts] = useState<WorkoutDayResponse[]>(
    []
  );

  // UI state
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [pastExpanded, setPastExpanded] = useState(false);
  const [futureExpanded, setFutureExpanded] = useState(false);

  // Selected workout state
  const [selectedWorkoutId, setSelectedWorkoutId] = useState<string | null>(
    null
  );
  const [selectedWorkout, setSelectedWorkout] =
    useState<WorkoutDayResponse | null>(null);
  const [loadingWorkout, setLoadingWorkout] = useState(false);

  // Exercise interaction state
  const [expandedExerciseId, setExpandedExerciseId] = useState<string | null>(
    null
  );
  const [localSetData, setLocalSetData] = useState<
    Record<string, LocalSetData>
  >({});
  const [completingSetId, setCompletingSetId] = useState<string | null>(null);
  const [completingWorkout, setCompletingWorkout] = useState(false);

  const loadWorkouts = async () => {
    if (!user?.id) return;

    try {
      setLoading(true);

      // Fetch active programme summary
      const activeProgrammeSummary = await programmesApi.getActiveProgramme(
        user.id
      );
      if (!activeProgrammeSummary) {
        setActiveProgramme(null);
        setAllWorkouts([]);
        setPastWorkouts([]);
        setTodayWorkout(null);
        setFutureWorkouts([]);
        return;
      }

      // Fetch full programme details with weeks and workout days
      const programme = await programmesApi.getById(activeProgrammeSummary.id);
      setActiveProgramme(programme);

      // Collect all workout days
      const allWorkoutDays: WorkoutDayResponse[] = [];
      for (const week of programme.weeks) {
        allWorkoutDays.push(...week.workoutDays);
      }
      setAllWorkouts(allWorkoutDays);

      // Categorize by date
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      const past = allWorkoutDays
        .filter((day) => {
          const scheduledDate = new Date(day.scheduledDate);
          scheduledDate.setHours(0, 0, 0, 0);
          return scheduledDate.getTime() < today.getTime();
        })
        .sort(
          (a, b) =>
            new Date(b.scheduledDate).getTime() -
            new Date(a.scheduledDate).getTime()
        );

      const todayDay =
        allWorkoutDays.find((day) => {
          const scheduledDate = new Date(day.scheduledDate);
          scheduledDate.setHours(0, 0, 0, 0);
          return scheduledDate.getTime() === today.getTime();
        }) || null;

      const future = allWorkoutDays
        .filter((day) => {
          const scheduledDate = new Date(day.scheduledDate);
          scheduledDate.setHours(0, 0, 0, 0);
          return scheduledDate.getTime() > today.getTime();
        })
        .sort(
          (a, b) =>
            new Date(a.scheduledDate).getTime() -
            new Date(b.scheduledDate).getTime()
        );

      setPastWorkouts(past);
      setTodayWorkout(todayDay);
      setFutureWorkouts(future);

      // Auto-select today's workout if it exists and not a rest day
      if (todayDay && !todayDay.isRestDay) {
        setSelectedWorkoutId(todayDay.id);
        await loadWorkoutDetails(todayDay.id);
      }
    } catch (error) {
      console.error("Failed to load workouts:", error);
      Alert.alert("Error", "Failed to load workouts");
    } finally {
      setLoading(false);
    }
  };

  const loadWorkoutDetails = async (workoutId: string) => {
    try {
      setLoadingWorkout(true);
      const fullWorkout = await workoutsApi.getWorkoutDay(workoutId);
      setSelectedWorkout(fullWorkout);

      // Initialize local set data
      const initialSetData: Record<string, LocalSetData> = {};
      fullWorkout.exercises.forEach((exercise) => {
        exercise.sets.forEach((set) => {
          initialSetData[set.id] = {
            reps: set.reps ?? undefined,
            weight: set.weight ?? undefined,
            intensity: set.intensity ?? undefined,
          };
        });
      });
      setLocalSetData(initialSetData);
    } catch (error) {
      console.error("Failed to load workout details:", error);
      Alert.alert("Error", "Failed to load workout details");
    } finally {
      setLoadingWorkout(false);
    }
  };

  const handleSelectWorkout = async (workoutId: string) => {
    setSelectedWorkoutId(workoutId);
    setExpandedExerciseId(null);
    await loadWorkoutDetails(workoutId);
  };

  useEffect(() => {
    loadWorkouts();
  }, [user?.id]);

  const handleRefresh = async () => {
    setRefreshing(true);
    await loadWorkouts();
    setRefreshing(false);
  };

  const updateLocalSetData = (
    setId: string,
    field: keyof LocalSetData,
    value: number | Intensity | undefined
  ) => {
    setLocalSetData((prev) => ({
      ...prev,
      [setId]: {
        ...prev[setId],
        [field]: value,
      },
    }));
  };

  const handleCompleteSet = async (setId: string) => {
    if (!selectedWorkout) return;

    const localSet = localSetData[setId];
    if (!localSet) return;

    setCompletingSetId(setId);

    try {
      const updatedExercise = await workoutsApi.completeSet(setId, {
        reps: localSet.reps,
        weight: localSet.weight,
        intensity: localSet.intensity,
      });

      // Update the selected workout with the updated exercise
      setSelectedWorkout((prev) => {
        if (!prev) return prev;
        return {
          ...prev,
          exercises: prev.exercises.map((ex) =>
            ex.id === updatedExercise.id ? updatedExercise : ex
          ),
        };
      });

      // Also update in the appropriate list
      const updateWorkoutInList = (workout: WorkoutDayResponse) => {
        if (workout.id === selectedWorkout.id) {
          return {
            ...workout,
            exercises: workout.exercises.map((ex) =>
              ex.id === updatedExercise.id ? updatedExercise : ex
            ),
          };
        }
        return workout;
      };

      setPastWorkouts((prev) => prev.map(updateWorkoutInList));
      if (todayWorkout?.id === selectedWorkout.id) {
        setTodayWorkout((prev) => (prev ? updateWorkoutInList(prev) : prev));
      }
      setFutureWorkouts((prev) => prev.map(updateWorkoutInList));
    } catch (error) {
      console.error("Failed to complete set:", error);
      Alert.alert("Error", "Failed to complete set");
    } finally {
      setCompletingSetId(null);
    }
  };

  const handleAddSet = async (exercise: WorkoutExerciseResponse) => {
    if (!selectedWorkout) return;

    const setNumber = exercise.sets.length + 1;

    try {
      const updatedExercise = await workoutsApi.addSet(exercise.id, {
        setNumber,
        reps: exercise.targetReps,
        weight: exercise.targetWeight,
      });

      // Update the selected workout with the updated exercise
      setSelectedWorkout((prev) => {
        if (!prev) return prev;
        return {
          ...prev,
          exercises: prev.exercises.map((ex) =>
            ex.id === updatedExercise.id ? updatedExercise : ex
          ),
        };
      });

      // Initialize local set data for new set
      updatedExercise.sets.forEach((set) => {
        if (!localSetData[set.id]) {
          setLocalSetData((prev) => ({
            ...prev,
            [set.id]: {
              reps: set.reps ?? undefined,
              weight: set.weight ?? undefined,
              intensity: set.intensity ?? undefined,
            },
          }));
        }
      });
    } catch (error) {
      console.error("Failed to add set:", error);
      Alert.alert("Error", "Failed to add set");
    }
  };

  const handleCompleteWorkout = async () => {
    if (!selectedWorkout) return;

    Alert.alert(
      "Complete Workout",
      "Are you sure you want to mark this workout as complete?",
      [
        { text: "Cancel", style: "cancel" },
        {
          text: "Complete",
          onPress: async () => {
            setCompletingWorkout(true);
            try {
              await workoutsApi.completeWorkout(selectedWorkout.id, {
                completedAt: new Date().toISOString(),
              });

              Alert.alert("Success", "Workout completed!", [
                {
                  text: "OK",
                  onPress: () => {
                    // Reload workouts
                    loadWorkouts();
                  },
                },
              ]);
            } catch (error) {
              console.error("Failed to complete workout:", error);
              Alert.alert("Error", "Failed to complete workout");
            } finally {
              setCompletingWorkout(false);
            }
          },
        },
      ]
    );
  };

  const createSupersetLabel = (exercise: WorkoutExerciseResponse): string => {
    if (!selectedWorkout || !exercise.supersetGroupId) return "";

    const supersetCount = selectedWorkout.exercises.filter(
      (e) => e.supersetGroupId === exercise.supersetGroupId
    ).length;

    if (supersetCount === 2) return "Superset";
    if (supersetCount === 3) return "Triset";
    if (supersetCount >= 4) return "Giant set";
    return `${supersetCount} exercises`;
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={colors.primary} />
        <Text style={styles.loadingText}>Loading workouts...</Text>
      </View>
    );
  }

  // No active programme
  if (!activeProgramme) {
    return (
      <ScrollView
        style={styles.container}
        contentContainerStyle={styles.content}
        contentInsetAdjustmentBehavior="automatic"
      >
        <View style={styles.header}>
          <Text style={styles.title}>Workouts</Text>
        </View>
        <View style={styles.placeholder}>
          <Text style={styles.placeholderIcon}>🏋️</Text>
          <Text style={styles.placeholderTitle}>No Active Programme</Text>
          <Text style={styles.placeholderText}>
            Set a programme to active to start tracking your workouts
          </Text>
          <Link href="/(tabs)/programs" asChild>
            <Button
              title="Browse Programmes"
              variant="primary"
              onPress={() => {}}
            />
          </Link>
        </View>
      </ScrollView>
    );
  }

  // Programme not started
  if (!activeProgramme.startDate) {
    return (
      <ScrollView
        style={styles.container}
        contentContainerStyle={styles.content}
        contentInsetAdjustmentBehavior="automatic"
      >
        <View style={styles.header}>
          <Text style={styles.title}>Workouts</Text>
        </View>
        <View style={styles.placeholder}>
          <Text style={styles.placeholderIcon}>📅</Text>
          <Text style={styles.placeholderTitle}>Programme Not Started</Text>
          <Text style={styles.placeholderText}>
            Set a start date for your programme to view workouts
          </Text>
          <Link href="/(tabs)/programs" asChild>
            <Button
              title="Go to Programmes"
              variant="primary"
              onPress={() => {}}
            />
          </Link>
        </View>
      </ScrollView>
    );
  }

  // Render workout timeline
  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.content}
      contentInsetAdjustmentBehavior="automatic"
      refreshControl={
        <RefreshControl refreshing={refreshing} onRefresh={handleRefresh} />
      }
    >
      <View style={styles.header}>
        <Text style={styles.title}>Workouts</Text>
        <Text style={styles.subtitle}>{activeProgramme.name}</Text>
      </View>

      {/* Past Workouts Section */}
      {pastWorkouts.length > 0 && (
        <CollapsibleSection
          title="Past Workouts"
          count={pastWorkouts.length}
          isExpanded={pastExpanded}
          onToggle={() => setPastExpanded(!pastExpanded)}
        >
          {pastWorkouts.map((workout) => (
            <WorkoutDayListItem
              key={workout.id}
              workout={workout}
              isSelected={selectedWorkoutId === workout.id}
              isToday={false}
              onPress={() => handleSelectWorkout(workout.id)}
            />
          ))}
        </CollapsibleSection>
      )}

      {/* Today Section */}
      <CollapsibleSection
        title="Today"
        isExpanded={true}
        onToggle={() => {}}
        disableToggle={true}
      >
        {todayWorkout ? (
          todayWorkout.isRestDay ? (
            <View style={styles.restDayCard}>
              <Text style={styles.restDayIcon}>😴</Text>
              <Text style={styles.restDayTitle}>{todayWorkout.name}</Text>
              <Text style={styles.restDayText}>
                Rest and recovery are just as important as training
              </Text>
            </View>
          ) : (
            <WorkoutDayListItem
              workout={todayWorkout}
              isSelected={selectedWorkoutId === todayWorkout.id}
              isToday={true}
              onPress={() => handleSelectWorkout(todayWorkout.id)}
            />
          )
        ) : (
          <View style={styles.restDayCard}>
            <Text style={styles.restDayIcon}>🌟</Text>
            <Text style={styles.restDayTitle}>No Workout Scheduled</Text>
            <Text style={styles.restDayText}>
              Enjoy your rest day or check future workouts
            </Text>
          </View>
        )}
      </CollapsibleSection>

      {/* Future Workouts Section */}
      {futureWorkouts.length > 0 && (
        <CollapsibleSection
          title="Future Workouts"
          count={futureWorkouts.length}
          isExpanded={futureExpanded}
          onToggle={() => setFutureExpanded(!futureExpanded)}
        >
          {futureWorkouts.map((workout) => (
            <WorkoutDayListItem
              key={workout.id}
              workout={workout}
              isSelected={selectedWorkoutId === workout.id}
              isToday={false}
              onPress={() => handleSelectWorkout(workout.id)}
            />
          ))}
        </CollapsibleSection>
      )}

      {/* Selected Workout Details */}
      {selectedWorkout && !selectedWorkout.isRestDay && (
        <>
          <View style={styles.workoutDetailsContainer}>
            <View style={styles.workoutDetailsHeader}>
              <Text style={styles.workoutDetailsTitle}>
                {selectedWorkout.name}
              </Text>
              <Text style={styles.workoutDetailsSubtitle}>
                {selectedWorkout.exercises.reduce(
                  (acc, e) => acc + e.sets.filter((s) => s.isCompleted).length,
                  0
                )}{" "}
                /{" "}
                {selectedWorkout.exercises.reduce(
                  (acc, e) => acc + e.targetSets,
                  0
                )}{" "}
                sets completed
                {selectedWorkout.isCompleted && " • Completed ✓"}
              </Text>
            </View>

            {loadingWorkout ? (
              <View style={styles.loadingWorkoutContainer}>
                <ActivityIndicator size="large" color={colors.primary} />
              </View>
            ) : selectedWorkout.exercises.length === 0 ? (
              <View style={styles.placeholder}>
                <Text style={styles.placeholderIcon}>📝</Text>
                <Text style={styles.placeholderTitle}>No Exercises</Text>
                <Text style={styles.placeholderText}>
                  Add exercises to this workout in your programme
                </Text>
              </View>
            ) : (
              <>
                {selectedWorkout.exercises
                  .sort((a, b) => a.orderIndex - b.orderIndex)
                  .map((exercise, index, exercises) => {
                    const nextExercise = exercises[index + 1];
                    const hasGap =
                      !exercise.supersetGroupId ||
                      !nextExercise?.supersetGroupId ||
                      exercise.supersetGroupId !== nextExercise.supersetGroupId;
                    const isInSuperset = !!exercise.supersetGroupId;

                    let supersetPosition:
                      | "first"
                      | "middle"
                      | "last"
                      | "single"
                      | undefined;
                    if (isInSuperset) {
                      const supersetExercises = exercises.filter(
                        (ex) => ex.supersetGroupId === exercise.supersetGroupId
                      );
                      const exerciseIndexInSuperset =
                        supersetExercises.findIndex(
                          (ex) => ex.id === exercise.id
                        );

                      if (supersetExercises.length === 1) {
                        supersetPosition = "single";
                      } else if (exerciseIndexInSuperset === 0) {
                        supersetPosition = "first";
                      } else if (
                        exerciseIndexInSuperset ===
                        supersetExercises.length - 1
                      ) {
                        supersetPosition = "last";
                      } else {
                        supersetPosition = "middle";
                      }
                    }

                    return (
                      <ExerciseCard
                        key={exercise.id}
                        exercise={exercise}
                        isExpanded={expandedExerciseId === exercise.id}
                        onToggleExpand={() =>
                          setExpandedExerciseId(
                            expandedExerciseId === exercise.id
                              ? null
                              : exercise.id
                          )
                        }
                        onAddSet={() => handleAddSet(exercise)}
                        localSetData={localSetData}
                        onUpdateLocalSet={updateLocalSetData}
                        onCompleteSet={handleCompleteSet}
                        completingSetId={completingSetId}
                        supersetLabel={
                          isInSuperset && supersetPosition === "first"
                            ? createSupersetLabel(exercise)
                            : undefined
                        }
                        isInSuperset={isInSuperset}
                        supersetPosition={supersetPosition}
                      />
                    );
                  })}

                <View style={styles.completeWorkoutContainer}>
                  {selectedWorkout.isCompleted ? (
                    <View style={styles.completedBadge}>
                      <Text style={styles.completedBadgeText}>
                        ✓ Workout Completed!
                      </Text>
                    </View>
                  ) : (
                    <Button
                      title={
                        completingWorkout ? "Completing..." : "Complete Workout"
                      }
                      onPress={handleCompleteWorkout}
                      variant="primary"
                      disabled={completingWorkout}
                      loading={completingWorkout}
                    />
                  )}
                </View>
              </>
            )}
          </View>
        </>
      )}
    </ScrollView>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      flex: 1,
      backgroundColor: isDark ? colors.dark.background : colors.background,
    },
    content: {
      padding: spacing.md,
      paddingBottom: spacing.xl,
    },
    loadingContainer: {
      flex: 1,
      alignItems: "center",
      justifyContent: "center",
      gap: spacing.md,
      backgroundColor: isDark ? colors.dark.background : colors.background,
    },
    loadingText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    header: {
      marginBottom: spacing.lg,
    },
    title: {
      ...typography.h1,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    subtitle: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    placeholder: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: spacing.xl,
      alignItems: "center",
      marginTop: spacing.lg,
    },
    placeholderIcon: {
      fontSize: 64,
      marginBottom: spacing.md,
    },
    placeholderTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
      textAlign: "center",
    },
    placeholderText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      textAlign: "center",
      marginBottom: spacing.md,
    },
    restDayCard: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.md,
      padding: spacing.lg,
      alignItems: "center",
    },
    restDayIcon: {
      fontSize: 48,
      marginBottom: spacing.sm,
    },
    restDayTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
      textAlign: "center",
    },
    restDayText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      textAlign: "center",
    },
    workoutDetailsContainer: {
      marginTop: spacing.lg,
    },
    workoutDetailsHeader: {
      marginBottom: spacing.md,
    },
    workoutDetailsTitle: {
      ...typography.h2,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    workoutDetailsSubtitle: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    loadingWorkoutContainer: {
      padding: spacing.xl,
      alignItems: "center",
    },
    completeWorkoutContainer: {
      marginTop: spacing.xl,
      alignItems: "center",
    },
    completedBadge: {
      backgroundColor: colors.success,
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.md,
      borderRadius: borderRadius.md,
    },
    completedBadgeText: {
      ...typography.button,
      color: "#FFFFFF",
    },
  });
