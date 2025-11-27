import React, { useState, useEffect } from 'react';
import styled from 'styled-components';
import { Link } from 'react-router-dom';
import {
  Container,
  PageWrapper,
  Card,
  CardHeader,
  CardTitle,
  Button,
  Badge,
  Flex,
  Stack,
  Input,
  Select,
} from '../components/styled';
import { Navigation } from '../components/Navigation';
import { useUser, useProgrammes } from '../hooks';
import { workoutsApi } from '../api';
import { WorkoutDay, WorkoutExercise, ExerciseSet, Difficulty, Intensity } from '../types';

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes['3xl']};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

const PageSubtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing['2xl']};
`;

const WorkoutCard = styled(Card)`
  margin-bottom: ${({ theme }) => theme.spacing.lg};
`;

const ExerciseCard = styled(Card)<{ $expanded: boolean }>`
  margin-bottom: ${({ theme }) => theme.spacing.md};
  border-color: ${({ $expanded, theme }) => 
    $expanded ? theme.colors.primary : theme.colors.border};
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
  opacity: ${({ $completed }) => $completed ? 0.6 : 1};
  
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

const CompleteButton = styled.button<{ $completed: boolean }>`
  width: 40px;
  height: 40px;
  border-radius: 50%;
  border: 2px solid ${({ $completed, theme }) => 
    $completed ? theme.colors.success : theme.colors.border};
  background: ${({ $completed, theme }) => 
    $completed ? theme.colors.success : 'transparent'};
  color: ${({ $completed }) => $completed ? 'white' : 'inherit'};
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  transition: all ${({ theme }) => theme.transitions.fast};
  
  &:hover {
    border-color: ${({ theme }) => theme.colors.success};
  }
`;

const EmptyState = styled.div`
  text-align: center;
  padding: ${({ theme }) => theme.spacing['3xl']};
  
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
  { value: '', label: 'Select difficulty' },
  { value: Difficulty.VeryEasy.toString(), label: 'Very Easy' },
  { value: Difficulty.Easy.toString(), label: 'Easy' },
  { value: Difficulty.Moderate.toString(), label: 'Moderate' },
  { value: Difficulty.Hard.toString(), label: 'Hard' },
  { value: Difficulty.VeryHard.toString(), label: 'Very Hard' },
  { value: Difficulty.Maximum.toString(), label: 'Maximum' },
];

const intensityOptions = [
  { value: '', label: 'Select intensity' },
  { value: Intensity.Low.toString(), label: 'Low' },
  { value: Intensity.Moderate.toString(), label: 'Moderate' },
  { value: Intensity.High.toString(), label: 'High' },
  { value: Intensity.VeryHigh.toString(), label: 'Very High' },
  { value: Intensity.Maximum.toString(), label: 'Maximum' },
];

interface LocalSet extends ExerciseSet {
  localReps?: number;
  localWeight?: number;
}

export const Workout: React.FC = () => {
  const { user } = useUser();
  const { activeProgramme } = useProgrammes(user?.id);
  const [todayWorkout, setTodayWorkout] = useState<WorkoutDay | null>(null);
  const [loading, setLoading] = useState(true);
  const [expandedExercise, setExpandedExercise] = useState<string | null>(null);
  const [localSets, setLocalSets] = useState<Record<string, LocalSet>>({});

  useEffect(() => {
    const findTodayWorkout = () => {
      if (!activeProgramme) {
        setLoading(false);
        return;
      }

      // Find today's workout based on day of week
      const today = new Date().getDay();
      
      // Look through all weeks to find an incomplete workout for today
      for (const week of activeProgramme.weeks || []) {
        const workout = week.workoutDays?.find(
          (day) => day.dayOfWeek === today && !day.isCompleted && !day.isRestDay
        );
        if (workout) {
          loadWorkoutDetail(workout.id);
          return;
        }
      }
      
      // If no workout for today, find the next incomplete workout
      for (const week of activeProgramme.weeks || []) {
        const workout = week.workoutDays?.find(
          (day) => !day.isCompleted && !day.isRestDay
        );
        if (workout) {
          loadWorkoutDetail(workout.id);
          return;
        }
      }
      
      setLoading(false);
    };

    findTodayWorkout();
  }, [activeProgramme]);

  const loadWorkoutDetail = async (workoutId: string) => {
    try {
      const response = await workoutsApi.getWorkoutDay(workoutId);
      setTodayWorkout(response.data);
      
      // Initialize local sets from workout data
      const setsMap: Record<string, LocalSet> = {};
      response.data.exercises.forEach((exercise) => {
        exercise.sets.forEach((set) => {
          setsMap[set.id] = {
            ...set,
            localReps: set.reps || undefined,
            localWeight: set.weight || undefined,
          };
        });
      });
      setLocalSets(setsMap);
    } catch (err) {
      console.error('Failed to load workout:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleAddSet = async (workoutExerciseId: string, exercise: WorkoutExercise) => {
    const setNumber = exercise.sets.length + 1;
    
    try {
      await workoutsApi.addSet(workoutExerciseId, {
        setNumber,
        reps: exercise.targetReps,
        weight: exercise.targetWeight,
        isWarmup: false,
      });
      
      // Reload workout
      if (todayWorkout) {
        await loadWorkoutDetail(todayWorkout.id);
      }
    } catch (err) {
      console.error('Failed to add set:', err);
    }
  };

  const handleCompleteSet = async (setId: string) => {
    const localSet = localSets[setId];
    
    try {
      await workoutsApi.completeSet(setId, {
        reps: localSet?.localReps,
        weight: localSet?.localWeight,
      });
      
      // Reload workout
      if (todayWorkout) {
        await loadWorkoutDetail(todayWorkout.id);
      }
    } catch (err) {
      console.error('Failed to complete set:', err);
    }
  };

  const handleCompleteWorkout = async () => {
    if (!todayWorkout) return;
    
    try {
      await workoutsApi.completeWorkout(todayWorkout.id, new Date().toISOString());
      setTodayWorkout({ ...todayWorkout, isCompleted: true });
    } catch (err) {
      console.error('Failed to complete workout:', err);
    }
  };

  const updateLocalSet = (setId: string, field: 'localReps' | 'localWeight', value: number) => {
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
            <div style={{ textAlign: 'center', padding: '4rem' }}>
              Loading workout...
            </div>
          </Container>
        </PageWrapper>
      </>
    );
  }

  if (!activeProgramme) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <Card>
              <EmptyState>
                <div className="icon">🏋️</div>
                <h3>No Active Programme</h3>
                <p>Create or select a programme to start working out</p>
                <Link to="/programmes">
                  <Button size="lg">Browse Programmes</Button>
                </Link>
              </EmptyState>
            </Card>
          </Container>
        </PageWrapper>
      </>
    );
  }

  if (!todayWorkout) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <Card>
              <EmptyState>
                <div className="icon">🎉</div>
                <h3>All Caught Up!</h3>
                <p>No scheduled workouts remaining. Great job!</p>
                <Link to="/programmes">
                  <Button size="lg">View Programme</Button>
                </Link>
              </EmptyState>
            </Card>
          </Container>
        </PageWrapper>
      </>
    );
  }

  const completedSets = todayWorkout.exercises.reduce(
    (acc, e) => acc + e.sets.filter((s) => s.isCompleted).length,
    0
  );
  const totalSets = todayWorkout.exercises.reduce(
    (acc, e) => acc + e.targetSets,
    0
  );

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <Flex justify="space-between" align="flex-start">
            <div>
              <PageTitle>{todayWorkout.name}</PageTitle>
              <PageSubtitle>
                {completedSets} / {totalSets} sets completed
              </PageSubtitle>
            </div>
            {todayWorkout.isCompleted ? (
              <Badge $variant="success">Completed!</Badge>
            ) : (
              <Button onClick={handleCompleteWorkout}>
                Complete Workout
              </Button>
            )}
          </Flex>

          {todayWorkout.exercises.length === 0 ? (
            <Card>
              <EmptyState>
                <div className="icon">📝</div>
                <h3>No Exercises</h3>
                <p>Add exercises to this workout in the programme builder</p>
                <Link to={`/programmes/${activeProgramme?.id}`}>
                  <Button>Edit Programme</Button>
                </Link>
              </EmptyState>
            </Card>
          ) : (
            todayWorkout.exercises
              .sort((a, b) => a.orderIndex - b.orderIndex)
              .map((exercise) => (
                <ExerciseCard 
                  key={exercise.id} 
                  $expanded={expandedExercise === exercise.id}
                >
                  <ExerciseHeader 
                    onClick={() => setExpandedExercise(
                      expandedExercise === exercise.id ? null : exercise.id
                    )}
                  >
                    <div>
                      <CardTitle style={{ fontSize: '1.125rem' }}>
                        {exercise.exerciseName}
                      </CardTitle>
                      <p style={{ color: '#64748B', fontSize: '0.875rem' }}>
                        {exercise.targetSets} sets × {exercise.targetReps} reps
                        {exercise.targetWeight && ` @ ${exercise.targetWeight}kg`}
                      </p>
                    </div>
                    <Flex gap="0.5rem" align="center">
                      <Badge $variant={
                        exercise.sets.filter(s => s.isCompleted).length === exercise.targetSets 
                          ? 'success' 
                          : 'default'
                      }>
                        {exercise.sets.filter(s => s.isCompleted).length}/{exercise.targetSets}
                      </Badge>
                      <span style={{ fontSize: '1.5rem' }}>
                        {expandedExercise === exercise.id ? '−' : '+'}
                      </span>
                    </Flex>
                  </ExerciseHeader>

                  {expandedExercise === exercise.id && (
                    <div style={{ paddingTop: '0.5rem' }}>
                      <div style={{ 
                        display: 'grid', 
                        gridTemplateColumns: '60px 1fr 1fr 1fr 1fr auto',
                        gap: '0.5rem',
                        fontSize: '0.75rem',
                        color: '#64748B',
                        textTransform: 'uppercase',
                        letterSpacing: '0.05em',
                        paddingBottom: '0.5rem'
                      }}>
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
                            <SetNumber>
                              {set.isWarmup ? 'W' : set.setNumber}
                            </SetNumber>
                            <SetInput
                              type="number"
                              value={localSets[set.id]?.localReps ?? set.reps ?? ''}
                              onChange={(e) => updateLocalSet(set.id, 'localReps', parseInt(e.target.value))}
                              placeholder={exercise.targetReps.toString()}
                              disabled={set.isCompleted}
                            />
                            <SetInput
                              type="number"
                              step="0.5"
                              value={localSets[set.id]?.localWeight ?? set.weight ?? ''}
                              onChange={(e) => updateLocalSet(set.id, 'localWeight', parseFloat(e.target.value))}
                              placeholder={exercise.targetWeight?.toString() || '0'}
                              disabled={set.isCompleted}
                            />
                            <select
                              style={{
                                padding: '0.5rem',
                                background: '#121822',
                                border: '1px solid #2D3748',
                                borderRadius: '0.5rem',
                                color: 'white',
                                fontSize: '0.75rem',
                              }}
                              disabled={set.isCompleted}
                              value={set.difficulty || ''}
                            >
                              {difficultyOptions.map((opt) => (
                                <option key={opt.value} value={opt.value}>{opt.label}</option>
                              ))}
                            </select>
                            <select
                              style={{
                                padding: '0.5rem',
                                background: '#121822',
                                border: '1px solid #2D3748',
                                borderRadius: '0.5rem',
                                color: 'white',
                                fontSize: '0.75rem',
                              }}
                              disabled={set.isCompleted}
                              value={set.intensity || ''}
                            >
                              {intensityOptions.map((opt) => (
                                <option key={opt.value} value={opt.value}>{opt.label}</option>
                              ))}
                            </select>
                            <CompleteButton
                              $completed={set.isCompleted}
                              onClick={() => !set.isCompleted && handleCompleteSet(set.id)}
                              disabled={set.isCompleted}
                            >
                              {set.isCompleted ? '✓' : ''}
                            </CompleteButton>
                          </SetRow>
                        ))}

                      <Button
                        variant="ghost"
                        size="sm"
                        fullWidth
                        style={{ marginTop: '1rem' }}
                        onClick={() => handleAddSet(exercise.id, exercise)}
                      >
                        + Add Set
                      </Button>
                    </div>
                  )}
                </ExerciseCard>
              ))
          )}
        </Container>
      </PageWrapper>
    </>
  );
};

