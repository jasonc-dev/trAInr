import React, { useEffect, useMemo, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import styled from "styled-components";
import {
  Container,
  PageWrapper,
  Grid,
  Card,
  Button,
  Badge,
  Flex,
  Stack,
  Input,
  Select,
} from "../components/styled";
import { Navigation } from "../components/styled/Navigation";
import { useExercises } from "../hooks";
import { programmesApi, exercisesApi, workoutsApi } from "../services";
import {
  Programme,
  ExerciseSummary,
  ExerciseType,
  MuscleGroup,
  DayOfWeek,
  WorkoutDay,
  WorkoutExercise,
} from "../types";
import { DAY_NAMES, getExerciseTypeLabel, getMuscleGroupLabel } from "../utils";

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes["3xl"]};
`;

const BackButton = styled.button`
  display: flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.xs};
  background: none;
  border: none;
  color: ${({ theme }) => theme.colors.textSecondary};
  cursor: pointer;
  margin-bottom: ${({ theme }) => theme.spacing.md};
  font-size: ${({ theme }) => theme.fontSizes.sm};

  &:hover {
    color: ${({ theme }) => theme.colors.text};
  }
`;

const WeekTabs = styled.div`
  display: flex;
  gap: ${({ theme }) => theme.spacing.xs};
  overflow-x: auto;
  padding-bottom: ${({ theme }) => theme.spacing.sm};
  margin-bottom: ${({ theme }) => theme.spacing.xl};
`;

const WeekTab = styled.button<{ $active: boolean; $completed: boolean }>`
  padding: ${({ theme }) => theme.spacing.sm} ${({ theme }) => theme.spacing.lg};
  background: ${({ $active, $completed, theme }) =>
    $active
      ? theme.colors.primary
      : $completed
      ? theme.colors.successLight
      : theme.colors.surface};
  color: ${({ $active, theme }) =>
    $active ? theme.colors.background : theme.colors.text};
  border: 1px solid
    ${({ $active, $completed, theme }) =>
      $active
        ? theme.colors.primary
        : $completed
        ? theme.colors.success
        : theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.lg};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  cursor: pointer;
  white-space: nowrap;
  transition: all ${({ theme }) => theme.transitions.fast};

  &:hover {
    border-color: ${({ theme }) => theme.colors.primary};
  }
`;

const DayCard = styled(Card)<{ $isRestDay?: boolean; $isCompleted?: boolean }>`
  background: ${({ $isRestDay, $isCompleted, theme }) =>
    $isRestDay
      ? theme.colors.backgroundSecondary
      : $isCompleted
      ? "rgba(0, 214, 143, 0.1)"
      : theme.colors.surface};
  border-color: ${({ $isCompleted, theme }) =>
    $isCompleted ? theme.colors.success : theme.colors.border};
`;

const DayHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: ${({ theme }) => theme.spacing.md};
`;

const DayName = styled.h4`
  font-size: ${({ theme }) => theme.fontSizes.lg};
`;

const ExerciseList = styled.div`
  display: flex;
  flex-direction: column;
`;

const ExerciseItem = styled.div<{
  $hasGap?: boolean;
  $isInSuperset?: boolean;
  $isDraggable?: boolean;
  $supersetPosition?: "first" | "middle" | "last" | "single";
}>`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: ${({ theme }) => theme.spacing.sm} ${({ theme }) => theme.spacing.md};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border-radius: ${({ $supersetPosition }) =>
    $supersetPosition === "first"
      ? "0.5rem 0.5rem 0 0"
      : $supersetPosition === "last"
      ? "0 0 0.5rem 0.5rem"
      : $supersetPosition === "middle"
      ? "0"
      : "0.5rem"};
  transition: all ${({ theme }) => theme.transitions.fast};
  border: 1px solid
    ${({ $isInSuperset, theme }) =>
      $isInSuperset ? theme.colors.border : "transparent"};
  border-bottom: ${({ $supersetPosition, $isInSuperset, theme }) =>
    $isInSuperset && $supersetPosition === "first"
      ? "none"
      : $isInSuperset && $supersetPosition === "middle"
      ? "none"
      : $isInSuperset
      ? theme.colors.border
      : "transparent"};
  border-top: ${({ $supersetPosition, $isInSuperset, theme }) =>
    $isInSuperset && $supersetPosition === "middle"
      ? "none"
      : $isInSuperset && $supersetPosition === "last"
      ? "none"
      : $isInSuperset
      ? theme.colors.border
      : "transparent"};
  margin-bottom: ${({ $hasGap, theme }) => ($hasGap ? theme.spacing.sm : "0")};
  cursor: ${({ $isDraggable }) => ($isDraggable ? "grab" : "default")};

  &:hover {
    background: ${({ theme }) => theme.colors.surfaceHover};
    border-color: ${({ $isInSuperset, theme }) =>
      $isInSuperset ? theme.colors.primary : theme.colors.border};
    border-bottom-color: ${({ $supersetPosition, $isInSuperset, theme }) =>
      $isInSuperset &&
      ($supersetPosition === "first" || $supersetPosition === "middle")
        ? theme.colors.primary
        : theme.colors.border};
    border-top-color: ${({ $supersetPosition, $isInSuperset, theme }) =>
      $isInSuperset &&
      ($supersetPosition === "middle" || $supersetPosition === "last")
        ? theme.colors.primary
        : theme.colors.border};
  }
`;

const ExerciseOrderHandle = styled.div`
  cursor: grab;
  padding: ${({ theme }) => theme.spacing.xs};
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-right: ${({ theme }) => theme.spacing.xs};
  display: flex;
  align-items: center;

  &:active {
    cursor: grabbing;
  }

  &:hover {
    color: ${({ theme }) => theme.colors.text};
  }
`;

const Modal = styled.div`
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: ${({ theme }) => theme.spacing.lg};
`;

const ModalContent = styled(Card)`
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
`;

const ExerciseSearchList = styled.div`
  max-height: 300px;
  overflow-y: auto;
  margin-top: ${({ theme }) => theme.spacing.md};
`;

const ExerciseSearchItem = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: ${({ theme }) => theme.spacing.md};
  border-bottom: 1px solid ${({ theme }) => theme.colors.border};
  cursor: pointer;

  &:hover {
    background: ${({ theme }) => theme.colors.surfaceHover};
  }

  &:last-child {
    border-bottom: none;
  }
`;

export const ProgrammeDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { searchExercises } = useExercises();

  const [programme, setProgramme] = useState<Programme | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedWeek, setSelectedWeek] = useState(0);
  const [draggedExercise, setDraggedExercise] = useState<string | null>(null);
  const [dragOverExercise, setDragOverExercise] = useState<string | null>(null);

  // Modal states
  const [showAddDayModal, setShowAddDayModal] = useState(false);
  const [showEditDayModal, setShowEditDayModal] = useState(false);
  const [showDeleteDayModal, setShowDeleteDayModal] = useState(false);
  const [showAddExerciseModal, setShowAddExerciseModal] = useState(false);
  const [showEditExerciseModal, setShowEditExerciseModal] = useState(false);
  const [showExerciseDetailsModal, setShowExerciseDetailsModal] =
    useState(false);
  const [selectedWorkoutDayId, setSelectedWorkoutDayId] = useState<
    string | null
  >(null);
  const [selectedExercise, setSelectedExercise] = useState<any | null>(null);
  const [selectedExercises, setSelectedExercises] = useState<Set<string>>(
    new Set()
  );
  const [showDropSetModal, setShowDropSetModal] = useState(false);

  // Form states
  const [newDay, setNewDay] = useState({
    dayOfWeek: DayOfWeek.Monday,
    name: "",
    description: "",
    isRestDay: false,
  });

  const [editDay, setEditDay] = useState({
    id: "",
    dayOfWeek: DayOfWeek.Monday,
    name: "",
    description: "",
    isRestDay: false,
  });

  const firstWorkoutDay = useMemo(() => {
    return programme?.weeks[selectedWeek]?.workoutDays.length === 0;
  }, [programme?.weeks, selectedWeek]);

  const firstWorkoutDayDayOfWeek = useMemo(() => {
    var dayOfWeek = programme?.weeks[selectedWeek]?.weekStartDate;
    const numberDayOfWeek = new Date(dayOfWeek || "").getDay();
    return numberDayOfWeek;
  }, [programme?.weeks, selectedWeek]);

  const [searchQuery, setSearchQuery] = useState("");
  const [searchResults, setSearchResults] = useState<ExerciseSummary[]>([]);
  const [newExercise, setNewExercise] = useState({
    exerciseId: "",
    targetSets: 3,
    targetReps: 10,
    targetWeight: 0,
    restSeconds: 90,
    targetRpe: undefined as number | undefined,
    notes: "",
  });

  const [editExercise, setEditExercise] = useState({
    id: "",
    targetSets: 3,
    targetReps: 10,
    targetWeight: 0,
    restSeconds: 90,
    targetRpe: undefined as number | undefined,
    notes: "",
    orderIndex: 0,
  });

  const [dropSetConfig, setDropSetConfig] = useState({
    exerciseId: "",
    startingWeight: 20,
    startingReps: 10,
    numberOfDrops: 2,
    dropPercentage: 20,
    repsAdjustment: 2,
  });

  useEffect(() => {
    const loadProgramme = async () => {
      if (!id) return;
      try {
        const response = await programmesApi.getById(id);
        setProgramme(response.data);
      } catch (err) {
        console.error("Failed to load programme:", err);
      } finally {
        setLoading(false);
      }
    };
    loadProgramme();
  }, [id]);

  useEffect(() => {
    const search = async () => {
      if (searchQuery.length >= 2) {
        const results = await searchExercises(searchQuery);
        setSearchResults(results);
      } else {
        setSearchResults([]);
      }
    };
    const debounce = setTimeout(search, 300);
    return () => clearTimeout(debounce);
  }, [searchQuery, searchExercises]);

  // Update newDay.dayOfWeek when it's the first workout day
  useEffect(() => {
    if (firstWorkoutDay && programme?.weeks[selectedWeek]) {
      setNewDay((prev) => ({
        ...prev,
        dayOfWeek: firstWorkoutDayDayOfWeek as DayOfWeek,
      }));
    }
  }, [
    firstWorkoutDay,
    firstWorkoutDayDayOfWeek,
    programme?.weeks,
    selectedWeek,
  ]);

  const handleAddFirstWeek = (
    programmeId: string
  ): React.MouseEventHandler<HTMLButtonElement> | undefined => {
    return async () => {
      try {
        await programmesApi.addWeek(programmeId, {
          weekNumber: 1,
          notes: "",
        });
        const response = await programmesApi.getById(programmeId);
        setProgramme(response.data);
      } catch (err) {
        console.error("Failed to add week:", err);
      }
    };
  };

  const handleAddDay = async () => {
    if (!programme || !programme.weeks[selectedWeek]) return;

    try {
      const weekId = programme.weeks[selectedWeek].id;
      const existingDays = programme.weeks[selectedWeek].workoutDays.length;

      await workoutsApi.createWorkoutDay(weekId, {
        dayOfWeek: newDay.dayOfWeek as unknown as DayOfWeek,
        name: newDay.name || `Day ${existingDays + 1}`,
        description: newDay.description,
        isRestDay: newDay.isRestDay,
      });

      // Reload programme
      const response = await programmesApi.getById(programme.id);
      setProgramme(response.data);
      setShowAddDayModal(false);
      setNewDay({
        dayOfWeek: DayOfWeek.Monday,
        name: "",
        description: "",
        isRestDay: false,
      });
    } catch (err) {
      console.error("Failed to add day:", err);
    }
  };

  const handleEditDay = (workoutDayId: string) => {
    const workoutDay = programme?.weeks[selectedWeek]?.workoutDays.find(
      (d) => d.id === workoutDayId
    );
    if (!workoutDay) return;

    setEditDay({
      id: workoutDay.id,
      dayOfWeek: workoutDay.dayOfWeek,
      name: workoutDay.name,
      description: workoutDay.description || "",
      isRestDay: workoutDay.isRestDay,
    });
    setSelectedWorkoutDayId(workoutDayId);
    setShowEditDayModal(true);
  };

  const handleUpdateDay = async () => {
    if (!editDay.id) return;

    try {
      await workoutsApi.updateWorkoutDay(editDay.id, {
        dayOfWeek: editDay.dayOfWeek,
        name: editDay.name,
        description: editDay.description,
        isRestDay: editDay.isRestDay,
      });

      // Reload programme
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
      setShowEditDayModal(false);
      setSelectedWorkoutDayId(null);
    } catch (err) {
      console.error("Failed to update day:", err);
    }
  };

  const handleDeleteDay = (workoutDayId: string) => {
    setSelectedWorkoutDayId(workoutDayId);
    setShowDeleteDayModal(true);
  };

  const handleConfirmDeleteDay = async () => {
    if (!selectedWorkoutDayId) return;

    try {
      await workoutsApi.deleteWorkoutDay(selectedWorkoutDayId);

      // Reload programme
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
      setShowDeleteDayModal(false);
      setSelectedWorkoutDayId(null);
    } catch (err) {
      console.error("Failed to delete day:", err);
    }
  };

  const handleAddExercise = async (exerciseId: string) => {
    if (!selectedWorkoutDayId) return;

    try {
      const workoutDay = programme?.weeks[selectedWeek]?.workoutDays.find(
        (d) => d.id === selectedWorkoutDayId
      );
      const orderIndex = workoutDay?.exercises.length || 0;

      await workoutsApi.addExercise(selectedWorkoutDayId, {
        exerciseId,
        orderIndex,
        targetSets: newExercise.targetSets,
        targetReps: newExercise.targetReps,
        targetWeight: newExercise.targetWeight || undefined,
        restSeconds: newExercise.restSeconds || undefined,
        targetRpe: newExercise.targetRpe || undefined,
        notes: newExercise.notes || undefined,
      });

      // Reload programme
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
      setShowAddExerciseModal(false);
      setSearchQuery("");
      setSearchResults([]);
      // Reset form
      setNewExercise({
        exerciseId: "",
        targetSets: 3,
        targetReps: 10,
        targetWeight: 0,
        restSeconds: 90,
        targetRpe: undefined,
        notes: "",
      });
    } catch (err) {
      console.error("Failed to add exercise:", err);
    }
  };

  const handleOpenEditExercise = (exercise: any) => {
    setEditExercise({
      id: exercise.id,
      targetSets: exercise.targetSets,
      targetReps: exercise.targetReps,
      targetWeight: exercise.targetWeight || 0,
      restSeconds: exercise.restSeconds || 90,
      targetRpe: exercise.targetRpe,
      notes: exercise.notes || "",
      orderIndex: exercise.orderIndex,
    });
    setSelectedExercise(exercise);
    setShowEditExerciseModal(true);
  };

  const handleUpdateExercise = async () => {
    if (!editExercise.id) return;

    try {
      await workoutsApi.updateExercise(editExercise.id, {
        orderIndex: editExercise.orderIndex,
        targetSets: editExercise.targetSets,
        targetReps: editExercise.targetReps,
        targetWeight: editExercise.targetWeight || undefined,
        restSeconds: editExercise.restSeconds || undefined,
        targetRpe: editExercise.targetRpe || undefined,
        notes: editExercise.notes || undefined,
      });

      // Reload programme
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
      setShowEditExerciseModal(false);
      setSelectedExercise(null);
    } catch (err) {
      console.error("Failed to update exercise:", err);
    }
  };

  const handleOpenExerciseDetails = async (exercise: any) => {
    try {
      const response = await exercisesApi.getById(exercise.exerciseId);
      setSelectedExercise({ ...exercise, details: response.data });
      setShowExerciseDetailsModal(true);
    } catch (err) {
      console.error("Failed to load exercise details:", err);
    }
  };

  const handleDragStart = (exerciseId: string) => {
    setDraggedExercise(exerciseId);
  };

  const handleDragOver = (e: React.DragEvent, exerciseId: string) => {
    e.preventDefault();
    setDragOverExercise(exerciseId);
  };

  const handleDragEnd = () => {
    setDraggedExercise(null);
    setDragOverExercise(null);
  };

  const handleDrop = async (
    e: React.DragEvent,
    workoutDayId: string,
    targetExerciseId: string
  ) => {
    e.preventDefault();

    if (!draggedExercise || draggedExercise === targetExerciseId) {
      setDraggedExercise(null);
      setDragOverExercise(null);
      return;
    }

    try {
      const workoutDay = programme?.weeks[selectedWeek]?.workoutDays.find(
        (d) => d.id === workoutDayId
      );

      if (!workoutDay) return;

      const exercises = [...workoutDay.exercises].sort(
        (a, b) => a.orderIndex - b.orderIndex
      );
      const draggedIndex = exercises.findIndex((e) => e.id === draggedExercise);
      const targetIndex = exercises.findIndex((e) => e.id === targetExerciseId);

      if (draggedIndex === -1 || targetIndex === -1) return;

      // Reorder the exercises
      const [removed] = exercises.splice(draggedIndex, 1);
      exercises.splice(targetIndex, 0, removed);

      // Get the ordered exercise IDs
      const orderedIds = exercises.map((e) => e.id);

      // Call the reorder API
      await workoutsApi.reorderExercises(workoutDayId, orderedIds);

      // Reload programme
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
    } catch (err) {
      console.error("Failed to reorder exercises:", err);
    } finally {
      setDraggedExercise(null);
      setDragOverExercise(null);
    }
  };

  const handleRemoveExercise = async (workoutExerciseId: string) => {
    try {
      await workoutsApi.removeExercise(workoutExerciseId);
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
    } catch (err) {
      console.error("Failed to remove exercise:", err);
    }
  };

  const handleToggleExerciseSelection = (exerciseId: string) => {
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

  const handleGroupAsSuperset = async (workoutDayId: string) => {
    if (selectedExercises.size < 2) {
      console.error("Need at least 2 exercises for a superset");
      return;
    }
    try {
      await workoutsApi.groupSuperset(workoutDayId, {
        exerciseIds: Array.from(selectedExercises),
        supersetRestSeconds: 120,
      });
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
      setSelectedExercises(new Set());
    } catch (err) {
      console.error("Failed to group superset:", err);
    }
  };

  const handleUngroupSuperset = async (supersetGroupId: string) => {
    try {
      await workoutsApi.ungroupSuperset(supersetGroupId);
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
    } catch (err) {
      console.error("Failed to ungroup superset:", err);
    }
  };

  const handleOpenDropSetModal = (exercise: any) => {
    setDropSetConfig({
      exerciseId: exercise.id,
      startingWeight: exercise.targetWeight || 20,
      startingReps: exercise.targetReps || 10,
      numberOfDrops: 2,
      dropPercentage: 20,
      repsAdjustment: 2,
    });
    setShowDropSetModal(true);
  };

  const handleCreateDropSet = async () => {
    if (!dropSetConfig.exerciseId) return;
    try {
      await workoutsApi.createDropSetSequence(dropSetConfig.exerciseId, {
        startingWeight: dropSetConfig.startingWeight,
        startingReps: dropSetConfig.startingReps,
        numberOfDrops: dropSetConfig.numberOfDrops,
        dropPercentage: dropSetConfig.dropPercentage,
        repsAdjustment: dropSetConfig.repsAdjustment,
      });
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
      setShowDropSetModal(false);
    } catch (err) {
      console.error("Failed to create drop set:", err);
    }
  };

  const handleCopyWeek = ():
    | React.MouseEventHandler<HTMLButtonElement>
    | undefined => {
    return async () => {
      try {
        // Find the previous week
        const previousWeek = programme?.weeks.find(
          (w) => w.weekNumber === currentWeek.weekNumber - 1
        );
        if (!previousWeek) return;
        // Copy content from previous week to current week
        await programmesApi.copyWeekContent(previousWeek.id, currentWeek.id);
        // Reload the programme to get updated data
        const response = await programmesApi.getById(programme?.id ?? "");
        setProgramme(response.data);
      } catch (err) {
        console.error("Failed to copy week:", err);
      }
    };
  };

  if (loading) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <div style={{ textAlign: "center", padding: "4rem" }}>
              Loading programme...
            </div>
          </Container>
        </PageWrapper>
      </>
    );
  }

  if (!programme) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <div style={{ textAlign: "center", padding: "4rem" }}>
              Programme not found
            </div>
          </Container>
        </PageWrapper>
      </>
    );
  }

  const currentWeek = programme.weeks[selectedWeek];

  const prevWeekHasExercises =
    programme.weeks[selectedWeek - 1]?.workoutDays?.some(
      (day) => day.exercises && day.exercises.length > 0
    ) ?? false;

  function createSupersetLabel(
    day: WorkoutDay,
    exercise: WorkoutExercise
  ): React.ReactNode {
    return (() => {
      const supersetCount = day.exercises
        .map((e) => e.supersetGroupId)
        .filter((id) => id === exercise.supersetGroupId).length;
      if (supersetCount === 2) return "Superset";
      if (supersetCount === 3) return "Triset";
      if (supersetCount >= 4) return "Giant set";
      return `${supersetCount} exercises`;
    })();
  }

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <BackButton onClick={() => navigate("/programmes")}>
            ← Back to Programmes
          </BackButton>

          <Flex
            $justify="space-between"
            $align="flex-start"
            style={{ marginBottom: "2rem" }}
          >
            <div>
              <PageTitle>{programme.name}</PageTitle>
              <p style={{ color: "#A0AEC0", marginTop: "0.5rem" }}>
                {programme.description || "No description"}
              </p>
            </div>
            {programme.isActive && <Badge $variant="primary">Active</Badge>}
          </Flex>

          {programme.weeks.length === 0 ? (
            <Card>
              <div style={{ textAlign: "center", padding: "3rem" }}>
                <div style={{ fontSize: "3rem", marginBottom: "1rem" }}>📅</div>
                <h4 style={{ marginBottom: "0.5rem" }}>No Weeks Configured</h4>
                <p style={{ color: "#A0AEC0", marginBottom: "1rem" }}>
                  This programme doesn't have any weeks set up yet. Add weeks to
                  start planning your workouts.
                </p>
                <Button onClick={handleAddFirstWeek(programme?.id ?? "")}>
                  Add First Week
                </Button>
              </div>
            </Card>
          ) : (
            <>
              <WeekTabs>
                {programme.weeks.map((week, index) => (
                  <WeekTab
                    key={week.id}
                    $active={selectedWeek === index}
                    $completed={week.isCompleted}
                    onClick={() => setSelectedWeek(index)}
                  >
                    Week {week.weekNumber}
                  </WeekTab>
                ))}
              </WeekTabs>

              {currentWeek && (
                <>
                  <Flex
                    $justify="space-between"
                    $align="center"
                    style={{ marginBottom: "1.5rem" }}
                  >
                    <h3>Week {currentWeek.weekNumber} Workouts</h3>
                    <Flex $gap="0.5rem">
                      <Button
                        size="sm"
                        onClick={() => setShowAddDayModal(true)}
                      >
                        + Add Day
                      </Button>
                      {currentWeek.weekNumber > 1 && prevWeekHasExercises && (
                        <Button
                          size="sm"
                          variant="secondary"
                          onClick={handleCopyWeek()}
                          title={`Copy all exercises from Week ${
                            currentWeek.weekNumber - 1
                          }`}
                        >
                          📋 Copy from Week {currentWeek.weekNumber - 1}
                        </Button>
                      )}
                    </Flex>
                  </Flex>

                  {currentWeek.workoutDays.length === 0 ? (
                    <Card>
                      <div style={{ textAlign: "center", padding: "3rem" }}>
                        <div style={{ fontSize: "3rem", marginBottom: "1rem" }}>
                          📅
                        </div>
                        <h4 style={{ marginBottom: "0.5rem" }}>
                          No Workout Days
                        </h4>
                        <p style={{ color: "#A0AEC0", marginBottom: "1rem" }}>
                          Add workout days to plan your week
                        </p>
                        <Button onClick={() => setShowAddDayModal(true)}>
                          Add First Day
                        </Button>
                      </div>
                    </Card>
                  ) : (
                    <Grid $columns={2} $gap="1.5rem">
                      {currentWeek.workoutDays
                        .sort((a, b) => a.dayOfWeek - b.dayOfWeek)
                        .map((day) => (
                          <DayCard
                            key={day.id}
                            $isRestDay={day.isRestDay}
                            $isCompleted={day.isCompleted}
                          >
                            <DayHeader>
                              <div>
                                <DayName>{day.name}</DayName>

                                <span
                                  style={{
                                    fontSize: "0.875rem",
                                    color: "#64748B",
                                  }}
                                >
                                  {DAY_NAMES[day.dayOfWeek]}
                                </span>
                              </div>
                              <Flex $gap="0.5rem">
                                {day.isRestDay && <Badge>Rest Day</Badge>}
                                {day.isCompleted && (
                                  <Badge $variant="success">Done</Badge>
                                )}
                              </Flex>
                              <Flex $gap="0.5rem">
                                <Button
                                  variant="primary"
                                  size="sm"
                                  onClick={() => handleEditDay(day.id)}
                                >
                                  Edit day
                                </Button>
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  onClick={() => handleDeleteDay(day.id)}
                                >
                                  Remove day
                                </Button>
                              </Flex>
                            </DayHeader>

                            {!day.isRestDay && (
                              <>
                                <ExerciseList>
                                  {day.exercises.length === 0 ? (
                                    <p
                                      style={{
                                        color: "#64748B",
                                        fontSize: "0.875rem",
                                        textAlign: "center",
                                        padding: "1rem",
                                      }}
                                    >
                                      No exercises added
                                    </p>
                                  ) : (
                                    day.exercises
                                      .sort(
                                        (a, b) => a.orderIndex - b.orderIndex
                                      )
                                      .map((exercise, index, exercises) => {
                                        const nextExercise =
                                          exercises[index + 1];
                                        const hasGap =
                                          !exercise.supersetGroupId ||
                                          !nextExercise?.supersetGroupId ||
                                          exercise.supersetGroupId !==
                                            nextExercise.supersetGroupId;
                                        const isInSuperset =
                                          !!exercise.supersetGroupId;
                                        const isDraggable =
                                          !exercise.supersetGroupId;

                                        // Determine superset position
                                        let supersetPosition:
                                          | "first"
                                          | "middle"
                                          | "last"
                                          | "single"
                                          | undefined;
                                        if (isInSuperset) {
                                          const supersetExercises =
                                            exercises.filter(
                                              (ex) =>
                                                ex.supersetGroupId ===
                                                exercise.supersetGroupId
                                            );
                                          const exerciseIndexInSuperset =
                                            supersetExercises.findIndex(
                                              (ex) => ex.id === exercise.id
                                            );

                                          if (supersetExercises.length === 1) {
                                            supersetPosition = "single";
                                          } else if (
                                            exerciseIndexInSuperset === 0
                                          ) {
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
                                          <ExerciseItem
                                            key={exercise.id}
                                            $hasGap={hasGap}
                                            $isInSuperset={isInSuperset}
                                            $isDraggable={isDraggable}
                                            $supersetPosition={supersetPosition}
                                            draggable={isDraggable}
                                            onDragStart={
                                              isDraggable
                                                ? () =>
                                                    handleDragStart(exercise.id)
                                                : undefined
                                            }
                                            onDragOver={
                                              isDraggable
                                                ? (e) =>
                                                    handleDragOver(
                                                      e,
                                                      exercise.id
                                                    )
                                                : undefined
                                            }
                                            onDragEnd={
                                              isDraggable
                                                ? handleDragEnd
                                                : undefined
                                            }
                                            onDrop={
                                              isDraggable
                                                ? (e) =>
                                                    handleDrop(
                                                      e,
                                                      day.id,
                                                      exercise.id
                                                    )
                                                : undefined
                                            }
                                            style={{
                                              opacity:
                                                draggedExercise === exercise.id
                                                  ? 0.5
                                                  : 1,
                                              borderColor:
                                                dragOverExercise === exercise.id
                                                  ? "#3B82F6"
                                                  : isInSuperset
                                                  ? "rgba(59, 130, 246, 0.3)"
                                                  : "transparent",
                                            }}
                                          >
                                            <Flex $align="center" $gap="0.5rem">
                                              <input
                                                type="checkbox"
                                                checked={selectedExercises.has(
                                                  exercise.id
                                                )}
                                                onChange={() =>
                                                  handleToggleExerciseSelection(
                                                    exercise.id
                                                  )
                                                }
                                                onClick={(e) =>
                                                  e.stopPropagation()
                                                }
                                                style={{ cursor: "pointer" }}
                                              />
                                              <ExerciseOrderHandle
                                                style={{
                                                  cursor: isDraggable
                                                    ? "pointer"
                                                    : "default",
                                                }}
                                              >
                                                {isDraggable ? "⋮⋮" : "•"}
                                              </ExerciseOrderHandle>
                                              <div
                                                style={{
                                                  flex: 1,
                                                  cursor: "pointer",
                                                }}
                                                onClick={() =>
                                                  handleOpenExerciseDetails(
                                                    exercise
                                                  )
                                                }
                                              >
                                                <div
                                                  style={{
                                                    fontWeight: 500,
                                                    display: "flex",
                                                    alignItems: "center",
                                                    gap: "0.5rem",
                                                  }}
                                                >
                                                  <span>
                                                    {exercise.exerciseName}
                                                  </span>
                                                  {exercise.supersetGroupId && (
                                                    <Flex
                                                      $align="center"
                                                      $gap="0.25rem"
                                                    >
                                                      <Badge $variant="primary">
                                                        {createSupersetLabel(
                                                          day,
                                                          exercise
                                                        )}
                                                      </Badge>
                                                      <Button
                                                        variant="ghost"
                                                        size="sm"
                                                        onClick={(e) => {
                                                          e.stopPropagation();
                                                          handleUngroupSuperset(
                                                            exercise.supersetGroupId!
                                                          );
                                                        }}
                                                        title="Ungroup from superset"
                                                        style={{
                                                          padding:
                                                            "0.125rem 0.25rem",
                                                          fontSize: "0.625rem",
                                                        }}
                                                      >
                                                        ✕
                                                      </Button>
                                                    </Flex>
                                                  )}
                                                  {exercise.targetRpe && (
                                                    <Badge
                                                      style={{
                                                        fontSize: "0.65rem",
                                                        padding:
                                                          "0.125rem 0.375rem",
                                                      }}
                                                    >
                                                      RPE {exercise.targetRpe}
                                                    </Badge>
                                                  )}
                                                </div>
                                                <div
                                                  style={{
                                                    fontSize: "0.75rem",
                                                    color: "#64748B",
                                                    marginTop: "0.25rem",
                                                  }}
                                                >
                                                  {exercise.targetSets} sets ×{" "}
                                                  {exercise.targetReps} reps
                                                  {exercise.targetWeight &&
                                                    ` @ ${exercise.targetWeight}kg`}
                                                  {exercise.restSeconds &&
                                                    ` • ${Math.floor(
                                                      exercise.restSeconds / 60
                                                    )}:${String(
                                                      exercise.restSeconds % 60
                                                    ).padStart(2, "0")} rest`}
                                                </div>
                                                {exercise.notes && (
                                                  <div
                                                    style={{
                                                      fontSize: "0.7rem",
                                                      color: "#94A3B8",
                                                      fontStyle: "italic",
                                                      marginTop: "0.25rem",
                                                    }}
                                                  >
                                                    💡 {exercise.notes}
                                                  </div>
                                                )}
                                              </div>
                                            </Flex>
                                            <Flex $gap="0.25rem">
                                              <Button
                                                variant="ghost"
                                                size="sm"
                                                onClick={(e) => {
                                                  e.stopPropagation();
                                                  handleOpenEditExercise(
                                                    exercise
                                                  );
                                                }}
                                                title="Edit exercise"
                                              >
                                                ✎
                                              </Button>
                                              <Button
                                                variant="ghost"
                                                size="sm"
                                                onClick={(e) => {
                                                  e.stopPropagation();
                                                  handleRemoveExercise(
                                                    exercise.id
                                                  );
                                                }}
                                                title="Remove exercise"
                                              >
                                                ×
                                              </Button>
                                            </Flex>
                                          </ExerciseItem>
                                        );
                                      })
                                  )}
                                </ExerciseList>
                                <Flex
                                  $gap="0.5rem"
                                  style={{ marginTop: "1rem" }}
                                  $wrap
                                >
                                  <Button
                                    variant="ghost"
                                    size="sm"
                                    style={{ flex: 1 }}
                                    onClick={() => {
                                      setSelectedWorkoutDayId(day.id);
                                      setShowAddExerciseModal(true);
                                    }}
                                  >
                                    + Add Exercise
                                  </Button>
                                  {selectedExercises.size >= 2 &&
                                    day.exercises.some((e) =>
                                      selectedExercises.has(e.id)
                                    ) && (
                                      <Button
                                        variant="primary"
                                        size="sm"
                                        onClick={() =>
                                          handleGroupAsSuperset(day.id)
                                        }
                                      >
                                        Group as{" "}
                                        {selectedExercises.size === 2
                                          ? "Superset"
                                          : selectedExercises.size === 3
                                          ? "Triset"
                                          : "Giant Set"}{" "}
                                        ({selectedExercises.size})
                                      </Button>
                                    )}
                                </Flex>
                              </>
                            )}
                          </DayCard>
                        ))}
                    </Grid>
                  )}
                </>
              )}
            </>
          )}
        </Container>

        {/* Add Day Modal */}
        {showAddDayModal && (
          <Modal onClick={() => setShowAddDayModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: "1.5rem" }}>Add Workout Day</h2>
              <Stack $gap="1rem">
                <Input
                  label="Day Name"
                  placeholder="e.g., Push Day, Leg Day"
                  value={newDay.name}
                  onChange={(e) =>
                    setNewDay({ ...newDay, name: e.target.value })
                  }
                />
                <Select
                  label="Day of Week"
                  options={DAY_NAMES.map((name, index) => ({
                    value: index.toString(),
                    label: name,
                  }))}
                  value={
                    firstWorkoutDay
                      ? firstWorkoutDayDayOfWeek.toString()
                      : newDay.dayOfWeek.toString()
                  }
                  onChange={(e) =>
                    setNewDay({
                      ...newDay,
                      dayOfWeek: parseInt(e.target.value) as DayOfWeek,
                    })
                  }
                />
                <Input
                  label="Description (optional)"
                  placeholder="Workout focus or notes"
                  value={newDay.description}
                  onChange={(e) =>
                    setNewDay({ ...newDay, description: e.target.value })
                  }
                />
                <Flex $gap="1rem">
                  <label
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "0.5rem",
                    }}
                  >
                    <input
                      type="checkbox"
                      checked={newDay.isRestDay}
                      onChange={(e) =>
                        setNewDay({ ...newDay, isRestDay: e.target.checked })
                      }
                    />
                    Rest Day
                  </label>
                </Flex>
                <Flex
                  $justify="flex-end"
                  $gap="1rem"
                  style={{ marginTop: "1rem" }}
                >
                  <Button
                    variant="ghost"
                    onClick={() => setShowAddDayModal(false)}
                  >
                    Cancel
                  </Button>
                  <Button onClick={handleAddDay}>Add Day</Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}

        {/* Edit Day Modal */}
        {showEditDayModal && (
          <Modal onClick={() => setShowEditDayModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: "1.5rem" }}>Edit Workout Day</h2>
              <Stack $gap="1rem">
                <Input
                  label="Day Name"
                  placeholder="e.g., Push Day, Leg Day"
                  value={editDay.name}
                  onChange={(e) =>
                    setEditDay({ ...editDay, name: e.target.value })
                  }
                />
                <Select
                  label="Day of Week"
                  options={DAY_NAMES.map((name, index) => ({
                    value: index.toString(),
                    label: name,
                  }))}
                  value={editDay.dayOfWeek.toString()}
                  onChange={(e) =>
                    setEditDay({
                      ...editDay,
                      dayOfWeek: parseInt(e.target.value) as DayOfWeek,
                    })
                  }
                />
                <Input
                  label="Description (optional)"
                  placeholder="Workout focus or notes"
                  value={editDay.description}
                  onChange={(e) =>
                    setEditDay({ ...editDay, description: e.target.value })
                  }
                />
                <Flex $gap="1rem">
                  <label
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "0.5rem",
                    }}
                  >
                    <input
                      type="checkbox"
                      checked={editDay.isRestDay}
                      onChange={(e) =>
                        setEditDay({ ...editDay, isRestDay: e.target.checked })
                      }
                    />
                    Rest Day
                  </label>
                </Flex>
                <Flex
                  $justify="flex-end"
                  $gap="1rem"
                  style={{ marginTop: "1rem" }}
                >
                  <Button
                    variant="ghost"
                    onClick={() => setShowEditDayModal(false)}
                  >
                    Cancel
                  </Button>
                  <Button onClick={handleUpdateDay}>Save Changes</Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}

        {/* Delete Day Modal */}
        {showDeleteDayModal && (
          <Modal onClick={() => setShowDeleteDayModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: "1rem" }}>Delete Workout Day</h2>
              <p style={{ marginBottom: "1.5rem", color: "#64748B" }}>
                Are you sure you want to delete this workout day? This action
                cannot be undone. All exercises in this day will also be
                removed.
              </p>
              <Flex $justify="flex-end" $gap="1rem">
                <Button
                  variant="ghost"
                  onClick={() => setShowDeleteDayModal(false)}
                >
                  Cancel
                </Button>
                <Button variant="danger" onClick={handleConfirmDeleteDay}>
                  Delete Day
                </Button>
              </Flex>
            </ModalContent>
          </Modal>
        )}

        {/* Add Exercise Modal */}
        {showAddExerciseModal && (
          <Modal onClick={() => setShowAddExerciseModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: "1.5rem" }}>Add Exercise</h2>
              <Input
                placeholder="Search exercises..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />

              <Flex $gap="1rem" style={{ marginTop: "1rem" }}>
                <Input
                  label="Sets"
                  type="number"
                  value={newExercise.targetSets}
                  onChange={(e) =>
                    setNewExercise({
                      ...newExercise,
                      targetSets: parseInt(e.target.value) || 3,
                    })
                  }
                />
                <Input
                  label="Reps"
                  type="number"
                  value={newExercise.targetReps}
                  onChange={(e) =>
                    setNewExercise({
                      ...newExercise,
                      targetReps: parseInt(e.target.value) || 10,
                    })
                  }
                />
                <Input
                  label="Weight (kg)"
                  type="number"
                  value={newExercise.targetWeight}
                  onChange={(e) =>
                    setNewExercise({
                      ...newExercise,
                      targetWeight: parseFloat(e.target.value) || 0,
                    })
                  }
                />
              </Flex>

              <Flex $gap="1rem" style={{ marginTop: "1rem" }}>
                <Input
                  label="Rest (seconds)"
                  type="number"
                  value={newExercise.restSeconds}
                  onChange={(e) =>
                    setNewExercise({
                      ...newExercise,
                      restSeconds: parseInt(e.target.value) || 90,
                    })
                  }
                />
                <Input
                  label="Target RPE (1-10)"
                  type="number"
                  min="1"
                  max="10"
                  value={newExercise.targetRpe || ""}
                  onChange={(e) =>
                    setNewExercise({
                      ...newExercise,
                      targetRpe: e.target.value
                        ? parseInt(e.target.value)
                        : undefined,
                    })
                  }
                  placeholder="Optional"
                />
              </Flex>

              <div style={{ marginTop: "1rem" }}>
                <Input
                  label="Notes (optional)"
                  placeholder="Any special instructions..."
                  value={newExercise.notes}
                  onChange={(e) =>
                    setNewExercise({ ...newExercise, notes: e.target.value })
                  }
                />
              </div>
              <ExerciseSearchList>
                {searchResults.length > 0 ? (
                  searchResults.map((exercise) => (
                    <ExerciseSearchItem
                      key={exercise.id}
                      onClick={() => handleAddExercise(exercise.id)}
                    >
                      <div>
                        <div style={{ fontWeight: 500 }}>{exercise.name}</div>
                        <div style={{ fontSize: "0.75rem", color: "#64748B" }}>
                          {getExerciseTypeLabel(exercise.type)} •{" "}
                          {getMuscleGroupLabel(exercise.primaryMuscleGroup)}
                          {exercise.secondaryMuscleGroup &&
                            ` • ${getMuscleGroupLabel(
                              exercise.secondaryMuscleGroup
                            )}`}
                        </div>
                      </div>
                      <Button variant="primary" size="sm">
                        Add
                      </Button>
                    </ExerciseSearchItem>
                  ))
                ) : searchQuery.length >= 2 ? (
                  <div
                    style={{
                      textAlign: "center",
                      padding: "2rem",
                      color: "#64748B",
                    }}
                  >
                    No exercises found
                  </div>
                ) : (
                  <div
                    style={{
                      textAlign: "center",
                      padding: "2rem",
                      color: "#64748B",
                    }}
                  >
                    Type to search exercises
                  </div>
                )}
              </ExerciseSearchList>

              <Flex $justify="flex-end" style={{ marginTop: "1rem" }}>
                <Button
                  variant="ghost"
                  onClick={() => setShowAddExerciseModal(false)}
                >
                  Close
                </Button>
              </Flex>
            </ModalContent>
          </Modal>
        )}

        {/* Edit Exercise Modal */}
        {showEditExerciseModal && selectedExercise && (
          <Modal onClick={() => setShowEditExerciseModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: "1.5rem" }}>
                Edit Exercise: {selectedExercise.exerciseName}
              </h2>
              <Stack $gap="1rem">
                <Flex $gap="1rem">
                  <Input
                    label="Sets"
                    type="number"
                    value={editExercise.targetSets}
                    onChange={(e) =>
                      setEditExercise({
                        ...editExercise,
                        targetSets: parseInt(e.target.value) || 3,
                      })
                    }
                  />
                  <Input
                    label="Reps"
                    type="number"
                    value={editExercise.targetReps}
                    onChange={(e) =>
                      setEditExercise({
                        ...editExercise,
                        targetReps: parseInt(e.target.value) || 10,
                      })
                    }
                  />
                  <Input
                    label="Weight (kg)"
                    type="number"
                    value={editExercise.targetWeight}
                    onChange={(e) =>
                      setEditExercise({
                        ...editExercise,
                        targetWeight: parseFloat(e.target.value) || 0,
                      })
                    }
                  />
                </Flex>

                <Flex $gap="1rem">
                  <Input
                    label="Rest (seconds)"
                    type="number"
                    value={editExercise.restSeconds}
                    onChange={(e) =>
                      setEditExercise({
                        ...editExercise,
                        restSeconds: parseInt(e.target.value) || 90,
                      })
                    }
                  />
                  <Input
                    label="Target RPE (1-10)"
                    type="number"
                    min="1"
                    max="10"
                    value={editExercise.targetRpe || ""}
                    onChange={(e) =>
                      setEditExercise({
                        ...editExercise,
                        targetRpe: e.target.value
                          ? parseInt(e.target.value)
                          : undefined,
                      })
                    }
                    placeholder="Optional"
                  />
                </Flex>

                <Flex>
                  <Button
                    size="sm"
                    variant="ghost"
                    onClick={() => handleOpenDropSetModal(editExercise)}
                  >
                    Create Drop Set
                  </Button>
                </Flex>

                <Input
                  label="Notes (optional)"
                  placeholder="Any special instructions..."
                  value={editExercise.notes}
                  onChange={(e) =>
                    setEditExercise({ ...editExercise, notes: e.target.value })
                  }
                />

                <Flex
                  $justify="flex-end"
                  $gap="1rem"
                  style={{ marginTop: "1rem" }}
                >
                  <Button
                    variant="ghost"
                    onClick={() => setShowEditExerciseModal(false)}
                  >
                    Cancel
                  </Button>
                  <Button onClick={handleUpdateExercise}>Save Changes</Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}

        {/* Exercise Details Modal */}
        {showExerciseDetailsModal && selectedExercise && (
          <Modal onClick={() => setShowExerciseDetailsModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: "1rem" }}>
                {selectedExercise.exerciseName}
              </h2>

              {selectedExercise.details && (
                <Stack $gap="1.5rem">
                  <div>
                    <h4
                      style={{
                        fontSize: "0.875rem",
                        color: "#64748B",
                        textTransform: "uppercase",
                        letterSpacing: "0.05em",
                        marginBottom: "0.5rem",
                      }}
                    >
                      Description
                    </h4>
                    <p>{selectedExercise.details.description}</p>
                  </div>

                  <Flex $gap="1rem" $wrap>
                    <Badge>{ExerciseType[selectedExercise.details.type]}</Badge>
                    <Badge>
                      {MuscleGroup[selectedExercise.details.primaryMuscleGroup]}
                    </Badge>
                    {selectedExercise.details.secondaryMuscleGroup && (
                      <Badge $variant="default">
                        Secondary:{" "}
                        {
                          MuscleGroup[
                            selectedExercise.details.secondaryMuscleGroup
                          ]
                        }
                      </Badge>
                    )}
                  </Flex>

                  {selectedExercise.details.instructions && (
                    <div>
                      <h4
                        style={{
                          fontSize: "0.875rem",
                          color: "#64748B",
                          textTransform: "uppercase",
                          letterSpacing: "0.05em",
                          marginBottom: "0.5rem",
                        }}
                      >
                        Instructions
                      </h4>
                      <p style={{ whiteSpace: "pre-line" }}>
                        {selectedExercise.details.instructions}
                      </p>
                    </div>
                  )}

                  {selectedExercise.details.videoUrl && (
                    <div>
                      <h4
                        style={{
                          fontSize: "0.875rem",
                          color: "#64748B",
                          textTransform: "uppercase",
                          letterSpacing: "0.05em",
                          marginBottom: "0.5rem",
                        }}
                      >
                        Video
                      </h4>
                      <a
                        href={selectedExercise.details.videoUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        style={{ color: "#3B82F6" }}
                      >
                        Watch demonstration →
                      </a>
                    </div>
                  )}

                  <div
                    style={{
                      borderTop: "1px solid #2D3748",
                      paddingTop: "1rem",
                    }}
                  >
                    <h4
                      style={{
                        fontSize: "0.875rem",
                        color: "#64748B",
                        textTransform: "uppercase",
                        letterSpacing: "0.05em",
                        marginBottom: "0.5rem",
                      }}
                    >
                      Your Targets
                    </h4>
                    <div
                      style={{
                        display: "grid",
                        gridTemplateColumns: "repeat(2, 1fr)",
                        gap: "0.5rem",
                      }}
                    >
                      <div>
                        <strong>Sets:</strong> {selectedExercise.targetSets}
                      </div>
                      <div>
                        <strong>Reps:</strong> {selectedExercise.targetReps}
                      </div>
                      {selectedExercise.targetWeight && (
                        <div>
                          <strong>Weight:</strong>{" "}
                          {selectedExercise.targetWeight}kg
                        </div>
                      )}
                      {selectedExercise.restSeconds && (
                        <div>
                          <strong>Rest:</strong>{" "}
                          {Math.floor(selectedExercise.restSeconds / 60)}:
                          {String(selectedExercise.restSeconds % 60).padStart(
                            2,
                            "0"
                          )}
                        </div>
                      )}
                      {selectedExercise.targetRpe && (
                        <div>
                          <strong>Target RPE:</strong>{" "}
                          {selectedExercise.targetRpe}/10
                        </div>
                      )}
                    </div>
                    {selectedExercise.notes && (
                      <div style={{ marginTop: "0.5rem" }}>
                        <strong>Notes:</strong> {selectedExercise.notes}
                      </div>
                    )}
                  </div>
                </Stack>
              )}

              <Flex $justify="space-between" style={{ marginTop: "1.5rem" }}>
                <Button
                  variant="primary"
                  onClick={() => {
                    setShowExerciseDetailsModal(false);
                    handleOpenEditExercise(selectedExercise);
                  }}
                >
                  Edit Exercise
                </Button>
                <Button
                  variant="ghost"
                  onClick={() => setShowExerciseDetailsModal(false)}
                >
                  Close
                </Button>
              </Flex>
            </ModalContent>
          </Modal>
        )}

        {/* Drop Set Configuration Modal */}
        {showDropSetModal && (
          <Modal onClick={() => setShowDropSetModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: "1rem" }}>Configure Drop Set</h2>
              <Stack $gap="1rem">
                <p style={{ fontSize: "0.875rem", color: "#94A3B8" }}>
                  Drop sets involve reducing weight after each set while
                  increasing reps. Configure the starting parameters and
                  progression.
                </p>
                <Flex $gap="1rem">
                  <Input
                    label="Starting Weight (kg)"
                    type="number"
                    value={dropSetConfig.startingWeight}
                    onChange={(e) =>
                      setDropSetConfig({
                        ...dropSetConfig,
                        startingWeight: parseFloat(e.target.value) || 0,
                      })
                    }
                  />
                  <Input
                    label="Starting Reps"
                    type="number"
                    value={dropSetConfig.startingReps}
                    onChange={(e) =>
                      setDropSetConfig({
                        ...dropSetConfig,
                        startingReps: parseInt(e.target.value) || 10,
                      })
                    }
                  />
                </Flex>
                <Flex $gap="1rem">
                  <Input
                    label="Number of Drops"
                    type="number"
                    min="1"
                    max="5"
                    value={dropSetConfig.numberOfDrops}
                    onChange={(e) =>
                      setDropSetConfig({
                        ...dropSetConfig,
                        numberOfDrops: parseInt(e.target.value) || 2,
                      })
                    }
                  />
                  <Input
                    label="Drop Percentage (%)"
                    type="number"
                    min="10"
                    max="50"
                    value={dropSetConfig.dropPercentage}
                    onChange={(e) =>
                      setDropSetConfig({
                        ...dropSetConfig,
                        dropPercentage: parseFloat(e.target.value) || 20,
                      })
                    }
                  />
                </Flex>
                <Input
                  label="Reps Increase Per Drop"
                  type="number"
                  value={dropSetConfig.repsAdjustment}
                  onChange={(e) =>
                    setDropSetConfig({
                      ...dropSetConfig,
                      repsAdjustment: parseInt(e.target.value) || 2,
                    })
                  }
                />
                <div
                  style={{
                    padding: "1rem",
                    background: "#1E293B",
                    borderRadius: "0.5rem",
                  }}
                >
                  <strong>Preview:</strong>
                  <ul style={{ marginTop: "0.5rem", paddingLeft: "1.5rem" }}>
                    {Array.from({
                      length: dropSetConfig.numberOfDrops + 1,
                    }).map((_, i) => {
                      const weight =
                        dropSetConfig.startingWeight *
                        Math.pow(1 - dropSetConfig.dropPercentage / 100, i);
                      const reps =
                        dropSetConfig.startingReps +
                        i * dropSetConfig.repsAdjustment;
                      return (
                        <li key={i}>
                          Set {i + 1}: {weight.toFixed(1)}kg × {reps} reps{" "}
                          {i > 0 ? "(Drop)" : ""}
                        </li>
                      );
                    })}
                  </ul>
                </div>
                <Flex
                  $justify="flex-end"
                  $gap="1rem"
                  style={{ marginTop: "1rem" }}
                >
                  <Button
                    variant="ghost"
                    onClick={() => setShowDropSetModal(false)}
                  >
                    Cancel
                  </Button>
                  <Button onClick={handleCreateDropSet}>Create Drop Set</Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}
      </PageWrapper>
    </>
  );
};
