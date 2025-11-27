import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import {
  Container,
  PageWrapper,
  Grid,
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
import { useUser, useExercises } from '../hooks';
import { programmesApi, workoutsApi } from '../api';
import { Programme, WorkoutDay, ExerciseSummary, ExerciseType, MuscleGroup } from '../types';

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes['3xl']};
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
    $active ? theme.colors.primary : 
    $completed ? theme.colors.successLight : theme.colors.surface};
  color: ${({ $active, theme }) => 
    $active ? theme.colors.background : theme.colors.text};
  border: 1px solid ${({ $active, $completed, theme }) => 
    $active ? theme.colors.primary : 
    $completed ? theme.colors.success : theme.colors.border};
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
    $isRestDay ? theme.colors.backgroundSecondary :
    $isCompleted ? 'rgba(0, 214, 143, 0.1)' : theme.colors.surface};
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
  gap: ${({ theme }) => theme.spacing.sm};
`;

const ExerciseItem = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: ${({ theme }) => theme.spacing.sm} ${({ theme }) => theme.spacing.md};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border-radius: ${({ theme }) => theme.radii.md};
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

const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export const ProgrammeDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user } = useUser();
  const { exercises, searchExercises } = useExercises();
  
  const [programme, setProgramme] = useState<Programme | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedWeek, setSelectedWeek] = useState(0);
  
  // Modal states
  const [showAddDayModal, setShowAddDayModal] = useState(false);
  const [showAddExerciseModal, setShowAddExerciseModal] = useState(false);
  const [selectedWorkoutDayId, setSelectedWorkoutDayId] = useState<string | null>(null);
  
  // Form states
  const [newDay, setNewDay] = useState({
    dayOfWeek: 1,
    name: '',
    description: '',
    isRestDay: false,
  });
  
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<ExerciseSummary[]>([]);
  const [newExercise, setNewExercise] = useState({
    exerciseId: '',
    targetSets: 3,
    targetReps: 10,
    targetWeight: 0,
  });

  useEffect(() => {
    const loadProgramme = async () => {
      if (!id) return;
      try {
        const response = await programmesApi.getById(id);
        setProgramme(response.data);
      } catch (err) {
        console.error('Failed to load programme:', err);
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

  const handleAddDay = async () => {
    if (!programme || !programme.weeks[selectedWeek]) return;
    
    try {
      const weekId = programme.weeks[selectedWeek].id;
      const existingDays = programme.weeks[selectedWeek].workoutDays.length;
      
      await workoutsApi.createWorkoutDay(weekId, {
        dayOfWeek: newDay.dayOfWeek,
        name: newDay.name || `Day ${existingDays + 1}`,
        description: newDay.description,
        isRestDay: newDay.isRestDay,
      });
      
      // Reload programme
      const response = await programmesApi.getById(programme.id);
      setProgramme(response.data);
      setShowAddDayModal(false);
      setNewDay({ dayOfWeek: 1, name: '', description: '', isRestDay: false });
    } catch (err) {
      console.error('Failed to add day:', err);
    }
  };

  const handleAddExercise = async (exerciseId: string) => {
    if (!selectedWorkoutDayId) return;
    
    try {
      const workoutDay = programme?.weeks[selectedWeek]?.workoutDays.find(d => d.id === selectedWorkoutDayId);
      const orderIndex = workoutDay?.exercises.length || 0;
      
      await workoutsApi.addExercise(selectedWorkoutDayId, {
        exerciseId,
        orderIndex,
        targetSets: newExercise.targetSets,
        targetReps: newExercise.targetReps,
        targetWeight: newExercise.targetWeight || undefined,
      });
      
      // Reload programme
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
      setShowAddExerciseModal(false);
      setSearchQuery('');
      setSearchResults([]);
    } catch (err) {
      console.error('Failed to add exercise:', err);
    }
  };

  const handleRemoveExercise = async (workoutExerciseId: string) => {
    try {
      await workoutsApi.removeExercise(workoutExerciseId);
      const response = await programmesApi.getById(programme!.id);
      setProgramme(response.data);
    } catch (err) {
      console.error('Failed to remove exercise:', err);
    }
  };

  if (loading) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <div style={{ textAlign: 'center', padding: '4rem' }}>Loading programme...</div>
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
            <div style={{ textAlign: 'center', padding: '4rem' }}>Programme not found</div>
          </Container>
        </PageWrapper>
      </>
    );
  }

  const currentWeek = programme.weeks[selectedWeek];

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <BackButton onClick={() => navigate('/programmes')}>
            ← Back to Programmes
          </BackButton>
          
          <Flex justify="space-between" align="flex-start" style={{ marginBottom: '2rem' }}>
            <div>
              <PageTitle>{programme.name}</PageTitle>
              <p style={{ color: '#A0AEC0', marginTop: '0.5rem' }}>
                {programme.description || 'No description'}
              </p>
            </div>
            {programme.isActive && (
              <Badge $variant="primary">Active</Badge>
            )}
          </Flex>

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
              <Flex justify="space-between" align="center" style={{ marginBottom: '1.5rem' }}>
                <h3>Week {currentWeek.weekNumber} Workouts</h3>
                <Button size="sm" onClick={() => setShowAddDayModal(true)}>
                  + Add Day
                </Button>
              </Flex>

              {currentWeek.workoutDays.length === 0 ? (
                <Card>
                  <div style={{ textAlign: 'center', padding: '3rem' }}>
                    <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📅</div>
                    <h4 style={{ marginBottom: '0.5rem' }}>No Workout Days</h4>
                    <p style={{ color: '#A0AEC0', marginBottom: '1rem' }}>
                      Add workout days to plan your week
                    </p>
                    <Button onClick={() => setShowAddDayModal(true)}>
                      Add First Day
                    </Button>
                  </div>
                </Card>
              ) : (
                <Grid columns={2} gap="1.5rem">
                  {currentWeek.workoutDays
                    .sort((a, b) => a.dayOfWeek - b.dayOfWeek)
                    .map((day) => (
                      <DayCard key={day.id} $isRestDay={day.isRestDay} $isCompleted={day.isCompleted}>
                        <DayHeader>
                          <div>
                            <DayName>{day.name}</DayName>
                            <span style={{ fontSize: '0.875rem', color: '#64748B' }}>
                              {dayNames[day.dayOfWeek]}
                            </span>
                          </div>
                          <Flex gap="0.5rem">
                            {day.isRestDay && <Badge>Rest Day</Badge>}
                            {day.isCompleted && <Badge $variant="success">Done</Badge>}
                          </Flex>
                        </DayHeader>

                        {!day.isRestDay && (
                          <>
                            <ExerciseList>
                              {day.exercises.length === 0 ? (
                                <p style={{ color: '#64748B', fontSize: '0.875rem', textAlign: 'center', padding: '1rem' }}>
                                  No exercises added
                                </p>
                              ) : (
                                day.exercises
                                  .sort((a, b) => a.orderIndex - b.orderIndex)
                                  .map((exercise) => (
                                    <ExerciseItem key={exercise.id}>
                                      <div>
                                        <div style={{ fontWeight: 500 }}>{exercise.exerciseName}</div>
                                        <div style={{ fontSize: '0.75rem', color: '#64748B' }}>
                                          {exercise.targetSets} sets × {exercise.targetReps} reps
                                          {exercise.targetWeight && ` @ ${exercise.targetWeight}kg`}
                                        </div>
                                      </div>
                                      <Button 
                                        variant="ghost" 
                                        size="sm"
                                        onClick={() => handleRemoveExercise(exercise.id)}
                                      >
                                        ×
                                      </Button>
                                    </ExerciseItem>
                                  ))
                              )}
                            </ExerciseList>
                            <Button
                              variant="ghost"
                              size="sm"
                              fullWidth
                              style={{ marginTop: '1rem' }}
                              onClick={() => {
                                setSelectedWorkoutDayId(day.id);
                                setShowAddExerciseModal(true);
                              }}
                            >
                              + Add Exercise
                            </Button>
                          </>
                        )}
                      </DayCard>
                    ))}
                </Grid>
              )}
            </>
          )}
        </Container>

        {/* Add Day Modal */}
        {showAddDayModal && (
          <Modal onClick={() => setShowAddDayModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: '1.5rem' }}>Add Workout Day</h2>
              <Stack gap="1rem">
                <Input
                  label="Day Name"
                  placeholder="e.g., Push Day, Leg Day"
                  value={newDay.name}
                  onChange={(e) => setNewDay({ ...newDay, name: e.target.value })}
                />
                <Select
                  label="Day of Week"
                  options={dayNames.map((name, index) => ({ value: index.toString(), label: name }))}
                  value={newDay.dayOfWeek.toString()}
                  onChange={(e) => setNewDay({ ...newDay, dayOfWeek: parseInt(e.target.value) })}
                />
                <Input
                  label="Description (optional)"
                  placeholder="Workout focus or notes"
                  value={newDay.description}
                  onChange={(e) => setNewDay({ ...newDay, description: e.target.value })}
                />
                <Flex gap="1rem">
                  <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                    <input
                      type="checkbox"
                      checked={newDay.isRestDay}
                      onChange={(e) => setNewDay({ ...newDay, isRestDay: e.target.checked })}
                    />
                    Rest Day
                  </label>
                </Flex>
                <Flex justify="flex-end" gap="1rem" style={{ marginTop: '1rem' }}>
                  <Button variant="ghost" onClick={() => setShowAddDayModal(false)}>
                    Cancel
                  </Button>
                  <Button onClick={handleAddDay}>
                    Add Day
                  </Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}

        {/* Add Exercise Modal */}
        {showAddExerciseModal && (
          <Modal onClick={() => setShowAddExerciseModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <h2 style={{ marginBottom: '1.5rem' }}>Add Exercise</h2>
              <Input
                placeholder="Search exercises..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
              
              <Flex gap="1rem" style={{ marginTop: '1rem' }}>
                <Input
                  label="Sets"
                  type="number"
                  value={newExercise.targetSets}
                  onChange={(e) => setNewExercise({ ...newExercise, targetSets: parseInt(e.target.value) || 3 })}
                />
                <Input
                  label="Reps"
                  type="number"
                  value={newExercise.targetReps}
                  onChange={(e) => setNewExercise({ ...newExercise, targetReps: parseInt(e.target.value) || 10 })}
                />
                <Input
                  label="Weight (kg)"
                  type="number"
                  value={newExercise.targetWeight}
                  onChange={(e) => setNewExercise({ ...newExercise, targetWeight: parseFloat(e.target.value) || 0 })}
                />
              </Flex>

              <ExerciseSearchList>
                {searchResults.length > 0 ? (
                  searchResults.map((exercise) => (
                    <ExerciseSearchItem
                      key={exercise.id}
                      onClick={() => handleAddExercise(exercise.id)}
                    >
                      <div>
                        <div style={{ fontWeight: 500 }}>{exercise.name}</div>
                        <div style={{ fontSize: '0.75rem', color: '#64748B' }}>
                          {ExerciseType[exercise.type]} • {MuscleGroup[exercise.primaryMuscleGroup]}
                        </div>
                      </div>
                      <Button variant="primary" size="sm">Add</Button>
                    </ExerciseSearchItem>
                  ))
                ) : searchQuery.length >= 2 ? (
                  <div style={{ textAlign: 'center', padding: '2rem', color: '#64748B' }}>
                    No exercises found
                  </div>
                ) : (
                  <div style={{ textAlign: 'center', padding: '2rem', color: '#64748B' }}>
                    Type to search exercises
                  </div>
                )}
              </ExerciseSearchList>
              
              <Flex justify="flex-end" style={{ marginTop: '1rem' }}>
                <Button variant="ghost" onClick={() => setShowAddExerciseModal(false)}>
                  Close
                </Button>
              </Flex>
            </ModalContent>
          </Modal>
        )}
      </PageWrapper>
    </>
  );
};

