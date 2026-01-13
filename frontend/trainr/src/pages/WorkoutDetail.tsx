import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { useParams, useNavigate, Link } from "react-router-dom";
import {
  Container,
  PageWrapper,
  Card,
  CardTitle,
  Button,
  Badge,
  Flex,
} from "../components/styled";
import { Navigation } from "../components/styled/Navigation";
import { useWorkouts } from "../hooks";
import {
  WorkoutExercise,
  Difficulty,
  Intensity,
  CompleteSetRequest,
} from "../types";

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes["3xl"]};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

const PageSubtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing["2xl"]};
`;

const ExerciseCard = styled(Card)<{
  $expanded: boolean;
  $hasGap?: boolean;
  $isInSuperset?: boolean;
  $supersetPosition?: "first" | "middle" | "last" | "single";
}>`
  margin-bottom: ${({ $hasGap, theme }) => ($hasGap ? theme.spacing.md : "0")};
  border-color: ${({ $expanded, $isInSuperset, theme }) =>
    $expanded
      ? theme.colors.primary
      : $isInSuperset
      ? theme.colors.border
      : theme.colors.border};
  border-radius: ${({ $supersetPosition }) =>
    $supersetPosition === "first"
      ? "0.5rem 0.5rem 0 0"
      : $supersetPosition === "last"
      ? "0 0 0.5rem 0.5rem"
      : $supersetPosition === "middle"
      ? "0"
      : "0.5rem"};
  border-bottom: ${({ $supersetPosition, $isInSuperset, theme }) =>
    $isInSuperset && $supersetPosition === "first"
      ? "none"
      : $isInSuperset && $supersetPosition === "middle"
      ? "none"
      : `1px solid ${theme.colors.border}`};
  border-top: ${({ $supersetPosition, $isInSuperset, theme }) =>
    $isInSuperset && $supersetPosition === "middle"
      ? "none"
      : $isInSuperset && $supersetPosition === "last"
      ? "none"
      : `1px solid ${theme.colors.border}`};

  &:hover {
    border-color: ${({ $isInSuperset, theme }) =>
      $isInSuperset ? theme.colors.primary : theme.colors.border};
  }
`;

const ExerciseHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  padding-bottom: ${({ theme }) => theme.spacing.sm};
`;

const SetRow = styled.div<{ $completed: boolean }>`
  display: grid;
  grid-template-columns: 60px 1fr 1fr 1fr 1fr auto;
  gap: ${({ theme }) => theme.spacing.sm};
  align-items: center;
  padding: ${({ theme }) => theme.spacing.sm} 0;
  border-top: 1px solid ${({ theme }) => theme.colors.border};
  opacity: ${({ $completed }) => ($completed ? 0.6 : 1)};

  @media (max-width: ${({ theme }) => theme.breakpoints.md}) {
    grid-template-columns: 50px 1fr 1fr auto;
  }
`;

const SetNumber = styled.span`
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  color: ${({ theme }) => theme.colors.textSecondary};
`;

const SetInput = styled.input`
  width: 100%;
  padding: ${({ theme }) => theme.spacing.sm};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border: 1px solid ${({ theme }) => theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.md};
  color: ${({ theme }) => theme.colors.text};
  font-size: ${({ theme }) => theme.fontSizes.sm};
  text-align: center;

  &:focus {
    outline: none;
    border-color: ${({ theme }) => theme.colors.primary};
  }
`;

const SetSelect = styled.select`
  padding: 0.5rem;
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border: 1px solid ${({ theme }) => theme.colors.border};
  border-radius: 0.5rem;
  color: ${({ theme }) => theme.colors.text};
  font-size: 0.75rem;

  &:focus {
    outline: none;
    border-color: ${({ theme }) => theme.colors.primary};
  }
`;

const CompleteButton = styled.button<{ $completed: boolean }>`
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: 2px solid
    ${({ $completed, theme }) =>
      $completed ? theme.colors.success : theme.colors.border};
  background: ${({ $completed, theme }) =>
    $completed ? theme.colors.success : "transparent"};
  color: ${({ $completed }) => ($completed ? "white" : "inherit")};
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  transition: all ${({ theme }) => theme.transitions.fast};

  &:hover:not(:disabled) {
    border-color: ${({ theme }) => theme.colors.success};
  }

  &:disabled {
    cursor: not-allowed;
  }
`;

const BackButton = styled(Button)`
  margin-bottom: ${({ theme }) => theme.spacing.lg};
`;

const EmptyState = styled.div`
  text-align: center;
  padding: ${({ theme }) => theme.spacing["3xl"]};

  .icon {
    font-size: 4rem;
    margin-bottom: ${({ theme }) => theme.spacing.lg};
  }

  h3 {
    font-size: ${({ theme }) => theme.fontSizes.xl};
    margin-bottom: ${({ theme }) => theme.spacing.sm};
  }

  p {
    color: ${({ theme }) => theme.colors.textSecondary};
    margin-bottom: ${({ theme }) => theme.spacing.xl};
  }
`;

const difficultyOptions = [
  { value: "", label: "Difficulty" },
  { value: Difficulty.VeryEasy.toString(), label: "Very Easy" },
  { value: Difficulty.Easy.toString(), label: "Easy" },
  { value: Difficulty.Moderate.toString(), label: "Moderate" },
  { value: Difficulty.Hard.toString(), label: "Hard" },
  { value: Difficulty.VeryHard.toString(), label: "Very Hard" },
];

const intensityOptions = [
  { value: "", label: "Intensity" },
  { value: Intensity.RPE6.toString(), label: "RPE 6" },
  { value: Intensity.RPE7.toString(), label: "RPE 7" },
  { value: Intensity.RPE8.toString(), label: "RPE 8" },
  { value: Intensity.RPE9.toString(), label: "RPE 9" },
  { value: Intensity.RPE10.toString(), label: "RPE 10" },
];

interface LocalSetData {
  reps?: number;
  weight?: number;
  difficulty?: Difficulty;
  intensity?: Intensity;
}

export const WorkoutDetail: React.FC = () => {
  const { workoutId } = useParams<{ workoutId: string }>();
  const navigate = useNavigate();
  const {
    currentWorkout,
    loadWorkout,
    completeWorkout,
    addSet,
    completeSet,
    loading: workoutLoading,
  } = useWorkouts();

  const [loading, setLoading] = useState(true);
  const [expandedExercise, setExpandedExercise] = useState<string | null>(null);
  const [localSets, setLocalSets] = useState<Record<string, LocalSetData>>({});
  const [completingSet, setCompletingSet] = useState<string | null>(null);

  useEffect(() => {
    const fetchWorkout = async () => {
      if (!workoutId) return;

      try {
        setLoading(true);
        await loadWorkout(workoutId);
      } catch (err) {
        console.error("Failed to load workout:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchWorkout();
  }, [workoutId, loadWorkout]);

  // Initialize local sets when workout loads
  useEffect(() => {
    if (currentWorkout) {
      const initialSets: Record<string, LocalSetData> = {};
      currentWorkout.exercises.forEach((exercise) => {
        exercise.sets.forEach((set) => {
          initialSets[set.id] = {
            reps: set.reps,
            weight: set.weight,
            difficulty: set.difficulty,
            intensity: set.intensity,
          };
        });
      });
      setLocalSets(initialSets);
    }
  }, [currentWorkout]);

  const handleAddSet = async (
    workoutExerciseId: string,
    exercise: WorkoutExercise
  ) => {
    const setNumber = exercise.sets.length + 1;

    try {
      await addSet(workoutExerciseId, {
        setNumber,
        reps: exercise.targetReps,
        weight: exercise.targetWeight,
      });
    } catch (err) {
      console.error("Failed to add set:", err);
    }
  };

  const handleCompleteSet = async (setId: string) => {
    const localSet = localSets[setId];
    if (!localSet) return;

    setCompletingSet(setId);

    try {
      const request: CompleteSetRequest = {
        reps: localSet.reps,
        weight: localSet.weight,
        difficulty: localSet.difficulty,
        intensity: localSet.intensity,
      };
      await completeSet(setId, request);
    } catch (err) {
      console.error("Failed to complete set:", err);
    } finally {
      setCompletingSet(null);
    }
  };

  const handleCompleteWorkout = async () => {
    if (!currentWorkout) return;

    try {
      await completeWorkout(currentWorkout.id);
      navigate("/workout");
    } catch (err) {
      console.error("Failed to complete workout:", err);
    }
  };

  const updateLocalSet = (
    setId: string,
    field: keyof LocalSetData,
    value: number | undefined
  ) => {
    setLocalSets((prev) => ({
      ...prev,
      [setId]: {
        ...prev[setId],
        [field]: value,
      },
    }));
  };

  if (loading) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <div style={{ textAlign: "center", padding: "4rem" }}>
              Loading workout...
            </div>
          </Container>
        </PageWrapper>
      </>
    );
  }

  if (!currentWorkout) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <Card>
              <EmptyState>
                <div className="icon">❌</div>
                <h3>Workout Not Found</h3>
                <p>The workout you're looking for doesn't exist</p>
                <Link to="/workout">
                  <Button size="lg">Back to Timeline</Button>
                </Link>
              </EmptyState>
            </Card>
          </Container>
        </PageWrapper>
      </>
    );
  }

  const completedSets = currentWorkout.exercises.reduce(
    (acc, e) => acc + e.sets.filter((s) => s.isCompleted).length,
    0
  );
  const totalSets = currentWorkout.exercises.reduce(
    (acc, e) => acc + e.targetSets,
    0
  );

  function createSupersetLabel(exercise: WorkoutExercise): React.ReactNode {
    if (!currentWorkout) return "";
    const supersetCount = currentWorkout.exercises.filter(
      (e) => e.supersetGroupId === exercise.supersetGroupId
    ).length;
    if (supersetCount === 2) return "Superset";
    if (supersetCount === 3) return "Triset";
    if (supersetCount >= 4) return "Giant set";
    return `${supersetCount} exercises`;
  }

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <BackButton
            variant="ghost"
            size="sm"
            onClick={() => navigate("/workout")}
          >
            ← Back to Timeline
          </BackButton>

          <Flex $justify="space-between" $align="flex-start">
            <div>
              <PageTitle>{currentWorkout.name}</PageTitle>
              <PageSubtitle>
                {completedSets} / {totalSets} sets completed
                {currentWorkout.completedDate && (
                  <span>
                    {" "}
                    • Completed on{" "}
                    {new Date(
                      currentWorkout.completedDate
                    ).toLocaleDateString()}
                  </span>
                )}
              </PageSubtitle>
            </div>
            {currentWorkout.isCompleted ? (
              <Badge $variant="success">Completed!</Badge>
            ) : (
              <Button onClick={handleCompleteWorkout} disabled={workoutLoading}>
                {workoutLoading ? "Saving..." : "Complete Workout"}
              </Button>
            )}
          </Flex>

          {currentWorkout.exercises.length === 0 ? (
            <Card>
              <EmptyState>
                <div className="icon">📝</div>
                <h3>No Exercises</h3>
                <p>Add exercises to this workout in the programme builder</p>
              </EmptyState>
            </Card>
          ) : (
            currentWorkout.exercises
              .sort((a, b) => a.orderIndex - b.orderIndex)
              .map((exercise, index, exercises) => {
                const nextExercise = exercises[index + 1];
                const hasGap =
                  !exercise.supersetGroupId ||
                  !nextExercise?.supersetGroupId ||
                  exercise.supersetGroupId !== nextExercise.supersetGroupId;
                const isInSuperset = !!exercise.supersetGroupId;

                // Determine superset position
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
                  const exerciseIndexInSuperset = supersetExercises.findIndex(
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
                    $expanded={expandedExercise === exercise.id}
                    $hasGap={hasGap}
                    $isInSuperset={isInSuperset}
                    $supersetPosition={supersetPosition}
                  >
                    <ExerciseHeader
                      onClick={() =>
                        setExpandedExercise(
                          expandedExercise === exercise.id ? null : exercise.id
                        )
                      }
                    >
                      <div>
                        <CardTitle style={{ fontSize: "1.125rem" }}>
                          <Flex $align="center" $gap="0.5rem">
                            <span>{exercise.exerciseName}</span>
                            {exercise.supersetGroupId && (
                              <Badge $variant="primary">
                                {createSupersetLabel(exercise)}
                              </Badge>
                            )}
                          </Flex>
                        </CardTitle>
                        <p style={{ color: "#64748B", fontSize: "0.875rem" }}>
                          {exercise.targetSets} sets × {exercise.targetReps}{" "}
                          reps
                          {exercise.targetWeight &&
                            ` @ ${exercise.targetWeight}kg`}
                        </p>
                      </div>
                      <Flex $gap="0.5rem" $align="center">
                        <Badge
                          $variant={
                            exercise.sets.filter((s) => s.isCompleted)
                              .length === exercise.targetSets
                              ? "success"
                              : "default"
                          }
                        >
                          {exercise.sets.filter((s) => s.isCompleted).length}/
                          {exercise.targetSets}
                        </Badge>
                        <span style={{ fontSize: "1.5rem" }}>
                          {expandedExercise === exercise.id ? "−" : "+"}
                        </span>
                      </Flex>
                    </ExerciseHeader>

                    {expandedExercise === exercise.id && (
                      <div style={{ paddingTop: "0.5rem" }}>
                        <div
                          style={{
                            display: "grid",
                            gridTemplateColumns: "60px 1fr 1fr 1fr 1fr auto",
                            gap: "0.5rem",
                            fontSize: "0.75rem",
                            color: "#64748B",
                            textTransform: "uppercase",
                            letterSpacing: "0.05em",
                            paddingBottom: "0.5rem",
                          }}
                        >
                          <span>Set</span>
                          <span>Reps</span>
                          <span>Weight</span>
                          <span>Difficulty</span>
                          <span>Intensity</span>
                          <span></span>
                        </div>

                        {exercise.sets
                          .sort((a, b) => a.setNumber - b.setNumber)
                          .map((set) => (
                            <SetRow key={set.id} $completed={set.isCompleted}>
                              <SetNumber>{set.setNumber}</SetNumber>
                              <SetInput
                                type="number"
                                value={
                                  localSets[set.id]?.reps ?? set.reps ?? ""
                                }
                                onChange={(e) =>
                                  updateLocalSet(
                                    set.id,
                                    "reps",
                                    e.target.value
                                      ? parseInt(e.target.value)
                                      : undefined
                                  )
                                }
                                placeholder={exercise.targetReps.toString()}
                                disabled={set.isCompleted}
                              />
                              <SetInput
                                type="number"
                                step="0.5"
                                value={
                                  localSets[set.id]?.weight ?? set.weight ?? ""
                                }
                                onChange={(e) =>
                                  updateLocalSet(
                                    set.id,
                                    "weight",
                                    e.target.value
                                      ? parseFloat(e.target.value)
                                      : undefined
                                  )
                                }
                                placeholder={
                                  exercise.targetWeight?.toString() || "0"
                                }
                                disabled={set.isCompleted}
                              />
                              <SetSelect
                                disabled={set.isCompleted}
                                value={
                                  localSets[set.id]?.difficulty?.toString() ??
                                  set.difficulty?.toString() ??
                                  ""
                                }
                                onChange={(e) =>
                                  updateLocalSet(
                                    set.id,
                                    "difficulty",
                                    e.target.value
                                      ? (parseInt(e.target.value) as Difficulty)
                                      : undefined
                                  )
                                }
                              >
                                {difficultyOptions.map((opt) => (
                                  <option key={opt.value} value={opt.value}>
                                    {opt.label}
                                  </option>
                                ))}
                              </SetSelect>
                              <SetSelect
                                disabled={set.isCompleted}
                                value={
                                  localSets[set.id]?.intensity?.toString() ??
                                  set.intensity?.toString() ??
                                  ""
                                }
                                onChange={(e) =>
                                  updateLocalSet(
                                    set.id,
                                    "intensity",
                                    e.target.value
                                      ? (parseInt(e.target.value) as Intensity)
                                      : undefined
                                  )
                                }
                              >
                                {intensityOptions.map((opt) => (
                                  <option key={opt.value} value={opt.value}>
                                    {opt.label}
                                  </option>
                                ))}
                              </SetSelect>
                              <CompleteButton
                                $completed={set.isCompleted}
                                onClick={() =>
                                  !set.isCompleted && handleCompleteSet(set.id)
                                }
                                disabled={
                                  set.isCompleted || completingSet === set.id
                                }
                              >
                                {set.isCompleted
                                  ? "✓"
                                  : completingSet === set.id
                                  ? "..."
                                  : ""}
                              </CompleteButton>
                            </SetRow>
                          ))}

                        <Button
                          variant="ghost"
                          size="sm"
                          fullWidth
                          style={{ marginTop: "1rem" }}
                          onClick={() => handleAddSet(exercise.id, exercise)}
                        >
                          + Add Set
                        </Button>
                      </div>
                    )}
                  </ExerciseCard>
                );
              })
          )}
        </Container>
      </PageWrapper>
    </>
  );
};
