import React, { useState, useEffect, useMemo } from "react";
import styled from "styled-components";
import { Link, useNavigate } from "react-router-dom";
import {
  Container,
  PageWrapper,
  Card,
  Button,
  Badge,
} from "../components/styled";
import { Navigation } from "../components/styled/Navigation";
import { useUser, useProgrammes, useWorkouts } from "../hooks";
import { WorkoutDay } from "../types";

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes["3xl"]};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

const PageSubtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing["2xl"]};
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

const TimelineContainer = styled.div`
  max-height: calc(100vh - 250px);
  overflow-y: auto;
  padding-right: ${({ theme }) => theme.spacing.sm};

  &::-webkit-scrollbar {
    width: 8px;
  }

  &::-webkit-scrollbar-track {
    background: ${({ theme }) => theme.colors.backgroundSecondary};
    border-radius: ${({ theme }) => theme.radii.md};
  }

  &::-webkit-scrollbar-thumb {
    background: ${({ theme }) => theme.colors.border};
    border-radius: ${({ theme }) => theme.radii.md};

    &:hover {
      background: ${({ theme }) => theme.colors.textSecondary};
    }
  }
`;

const TimelineSection = styled.div`
  margin-bottom: ${({ theme }) => theme.spacing["2xl"]};
`;

const SectionHeader = styled.h2`
  font-size: ${({ theme }) => theme.fontSizes.xl};
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  margin-bottom: ${({ theme }) => theme.spacing.lg};
  color: ${({ theme }) => theme.colors.text};
  position: sticky;
  top: 0;
  background: ${({ theme }) => theme.colors.background};
  padding: ${({ theme }) => theme.spacing.sm} 0;
  z-index: 10;
`;

const DateGroup = styled.div`
  margin-bottom: ${({ theme }) => theme.spacing.lg};
`;

const DateHeader = styled.div<{ $isPast?: boolean; $isToday?: boolean }>`
  display: flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.md};
  margin-bottom: ${({ theme }) => theme.spacing.md};
  padding-bottom: ${({ theme }) => theme.spacing.sm};
  border-bottom: 2px solid
    ${({ $isToday, $isPast, theme }) =>
      $isToday
        ? theme.colors.primary
        : $isPast
        ? theme.colors.border
        : theme.colors.border};

  .date-text {
    font-size: ${({ theme }) => theme.fontSizes.lg};
    font-weight: ${({ theme }) => theme.fontWeights.semibold};
    color: ${({ $isToday, theme }) =>
      $isToday ? theme.colors.primary : theme.colors.text};
  }

  .day-name {
    font-size: ${({ theme }) => theme.fontSizes.sm};
    color: ${({ theme }) => theme.colors.textSecondary};
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }
`;

const WorkoutCard = styled(Card)<{ $isCompleted?: boolean; $isPast?: boolean }>`
  margin-bottom: ${({ theme }) => theme.spacing.md};
  opacity: ${({ $isCompleted, $isPast }) =>
    $isCompleted && $isPast ? 0.8 : 1};
  border-left: 4px solid
    ${({ $isCompleted, theme }) =>
      $isCompleted ? theme.colors.success : theme.colors.border};
  cursor: pointer;
  transition: all ${({ theme }) => theme.transitions.normal};

  &:hover {
    border-left-color: ${({ $isCompleted, theme }) =>
      $isCompleted ? theme.colors.success : theme.colors.primary};
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }
`;

const WorkoutHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: ${({ theme }) => theme.spacing.sm};
`;

const WorkoutInfo = styled.div`
  flex: 1;
`;

const WorkoutTitle = styled.h3`
  font-size: ${({ theme }) => theme.fontSizes.lg};
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
  color: ${({ theme }) => theme.colors.text};
`;

const WorkoutMeta = styled.div`
  display: flex;
  gap: ${({ theme }) => theme.spacing.md};
  flex-wrap: wrap;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  color: ${({ theme }) => theme.colors.textSecondary};
`;

const WorkoutActions = styled.div`
  display: flex;
  gap: ${({ theme }) => theme.spacing.sm};
  align-items: center;
`;

const RestDayCard = styled(Card)`
  margin-bottom: ${({ theme }) => theme.spacing.md};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border-left: 4px solid ${({ theme }) => theme.colors.textSecondary};
  opacity: 0.8;
  text-align: center;
  padding: ${({ theme }) => theme.spacing.lg};

  .icon {
    font-size: 2rem;
    margin-bottom: ${({ theme }) => theme.spacing.sm};
  }

  .title {
    font-weight: ${({ theme }) => theme.fontWeights.semibold};
    color: ${({ theme }) => theme.colors.textSecondary};
  }
`;

interface WorkoutWithDate extends WorkoutDay {
  scheduledDate: Date;
  weekNumber: number;
}

interface GroupedWorkouts {
  past: Map<string, WorkoutWithDate[]>;
  today: WorkoutWithDate[];
  future: Map<string, WorkoutWithDate[]>;
}

// Helper functions for date calculations
const getDayName = (dayOfWeek: number): string => {
  const days = [
    "Sunday",
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday",
  ];
  return days[dayOfWeek];
};

const formatDate = (date: Date): string => {
  return date.toLocaleDateString("en-GB", {
    weekday: "short",
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};

const calculateScheduledDate = (
  programmeStartDate: string,
  weekNumber: number,
  dayOfWeek: number
): Date => {
  const startDate = new Date(programmeStartDate);
  const daysFromStart = (weekNumber - 1) * 7;
  const scheduledDate = new Date(startDate);
  scheduledDate.setDate(startDate.getDate() + daysFromStart);

  // Adjust to the correct day of week
  const currentDay = scheduledDate.getDay();
  const daysToAdd = dayOfWeek - currentDay;
  scheduledDate.setDate(scheduledDate.getDate() + daysToAdd);

  return scheduledDate;
};

const isSameDay = (date1: Date, date2: Date): boolean => {
  return (
    date1.getFullYear() === date2.getFullYear() &&
    date1.getMonth() === date2.getMonth() &&
    date1.getDate() === date2.getDate()
  );
};

const isBeforeDay = (date1: Date, date2: Date): boolean => {
  const d1 = new Date(date1.getFullYear(), date1.getMonth(), date1.getDate());
  const d2 = new Date(date2.getFullYear(), date2.getMonth(), date2.getDate());
  return d1 < d2;
};

export const Workout: React.FC = () => {
  const navigate = useNavigate();
  const { user } = useUser();
  const { activeProgramme } = useProgrammes(user?.id);

  const {
    completeWorkout,
    loadWorkoutDays,
    loading: workoutLoading,
  } = useWorkouts();

  const [allWorkoutDays, setAllWorkoutDays] = useState<WorkoutWithDate[]>([]);
  const [loading, setLoading] = useState(true);
  const [completingWorkout, setCompletingWorkout] = useState<string | null>(
    null
  );

  // Load all workout days from all weeks with their scheduled dates
  useEffect(() => {
    const loadAllWorkouts = async () => {
      if (!activeProgramme || !activeProgramme.startDate) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        const allWorkouts: WorkoutWithDate[] = [];

        // Load workouts from all weeks
        for (const week of activeProgramme.weeks) {
          const workoutDays = await loadWorkoutDays(week.id);

          // Calculate scheduled dates for each workout
          const workoutsWithDates = workoutDays.map((day) => ({
            ...day,
            scheduledDate: calculateScheduledDate(
              activeProgramme.startDate!,
              week.weekNumber,
              day.dayOfWeek
            ),
            weekNumber: week.weekNumber,
          }));

          allWorkouts.push(...workoutsWithDates);
        }

        // Sort by scheduled date
        allWorkouts.sort(
          (a, b) => a.scheduledDate.getTime() - b.scheduledDate.getTime()
        );

        setAllWorkoutDays(allWorkouts);
      } catch (err) {
        console.error("Failed to load workouts:", err);
      } finally {
        setLoading(false);
      }
    };

    loadAllWorkouts();
  }, [activeProgramme, loadWorkoutDays]);

  // Group workouts by past, today, and future
  const groupedWorkouts = useMemo((): GroupedWorkouts => {
    const today = new Date();
    const past = new Map<string, WorkoutWithDate[]>();
    const todayWorkouts: WorkoutWithDate[] = [];
    const future = new Map<string, WorkoutWithDate[]>();

    allWorkoutDays.forEach((workout) => {
      const dateKey = formatDate(workout.scheduledDate);

      if (isSameDay(workout.scheduledDate, today)) {
        todayWorkouts.push(workout);
      } else if (isBeforeDay(workout.scheduledDate, today)) {
        if (!past.has(dateKey)) {
          past.set(dateKey, []);
        }
        past.get(dateKey)!.push(workout);
      } else {
        if (!future.has(dateKey)) {
          future.set(dateKey, []);
        }
        future.get(dateKey)!.push(workout);
      }
    });

    return { past, today: todayWorkouts, future };
  }, [allWorkoutDays]);

  const handleCompleteWorkout = async (workoutId: string) => {
    try {
      setCompletingWorkout(workoutId);
      await completeWorkout(workoutId, new Date());

      // Refresh the workouts list
      setAllWorkoutDays((prev) =>
        prev.map((w) =>
          w.id === workoutId
            ? { ...w, isCompleted: true, completedAt: new Date().toISOString() }
            : w
        )
      );
    } catch (err) {
      console.error("Failed to complete workout:", err);
    } finally {
      setCompletingWorkout(null);
    }
  };

  const handleViewWorkout = (workoutId: string) => {
    navigate(`/workout/${workoutId}`);
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

  if (!activeProgramme.startDate) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <Card>
              <EmptyState>
                <div className="icon">📅</div>
                <h3>Programme Not Started</h3>
                <p>Set a start date for your programme to view workouts</p>
                <Link to={`/programmes/${activeProgramme.id}`}>
                  <Button size="lg">Set Start Date</Button>
                </Link>
              </EmptyState>
            </Card>
          </Container>
        </PageWrapper>
      </>
    );
  }

  if (allWorkoutDays.length === 0 && !loading) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <Card>
              <EmptyState>
                <div className="icon">🎉</div>
                <h3>No Workouts Scheduled</h3>
                <p>Add workout days to your programme to get started</p>
                <Link to={`/programmes/${activeProgramme.id}`}>
                  <Button size="lg">Edit Programme</Button>
                </Link>
              </EmptyState>
            </Card>
          </Container>
        </PageWrapper>
      </>
    );
  }

  const renderWorkoutCard = (workout: WorkoutWithDate, isPast: boolean) => {
    if (workout.isRestDay) {
      return (
        <RestDayCard key={workout.id}>
          <div className="icon">😴</div>
          <div className="title">{workout.name}</div>
        </RestDayCard>
      );
    }

    const exerciseCount = workout.exercises.length;
    const completedSets = workout.exercises.reduce(
      (acc, e) => acc + e.sets.filter((s) => s.isCompleted).length,
      0
    );
    const totalSets = workout.exercises.reduce(
      (acc, e) => acc + e.targetSets,
      0
    );

    return (
      <WorkoutCard
        key={workout.id}
        $isCompleted={workout.isCompleted}
        $isPast={isPast}
        onClick={() => handleViewWorkout(workout.id)}
      >
        <WorkoutHeader>
          <WorkoutInfo>
            <WorkoutTitle>{workout.name}</WorkoutTitle>
            <WorkoutMeta>
              <span>Week {workout.weekNumber}</span>
              <span>•</span>
              <span>{exerciseCount} exercises</span>
              <span>•</span>
              <span>
                {completedSets}/{totalSets} sets
              </span>
              {workout.completedAt && (
                <>
                  <span>•</span>
                  <span>
                    Completed:{" "}
                    {new Date(workout.completedAt).toLocaleDateString()}
                  </span>
                </>
              )}
            </WorkoutMeta>
          </WorkoutInfo>
          <WorkoutActions>
            {workout.isCompleted ? (
              <Badge $variant="success">✓ Completed</Badge>
            ) : (
              <Button
                size="sm"
                onClick={(e) => {
                  e.stopPropagation();
                  handleCompleteWorkout(workout.id);
                }}
                disabled={completingWorkout === workout.id || workoutLoading}
              >
                {completingWorkout === workout.id
                  ? "Completing..."
                  : "Mark Complete"}
              </Button>
            )}
          </WorkoutActions>
        </WorkoutHeader>
      </WorkoutCard>
    );
  };

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <div style={{ marginBottom: "2rem" }}>
            <PageTitle>Workout Timeline</PageTitle>
            <PageSubtitle>
              {activeProgramme.name} - Track your progress over time
            </PageSubtitle>
          </div>

          <TimelineContainer>
            {/* Today's Workouts */}
            {groupedWorkouts.today.length > 0 && (
              <TimelineSection>
                <SectionHeader>Today</SectionHeader>
                <DateGroup>
                  <DateHeader $isToday={true}>
                    <div className="date-text">{formatDate(new Date())}</div>
                    <div className="day-name">
                      {getDayName(new Date().getDay())}
                    </div>
                  </DateHeader>
                  {groupedWorkouts.today.map((workout) =>
                    renderWorkoutCard(workout, false)
                  )}
                </DateGroup>
              </TimelineSection>
            )}

            {/* Future Workouts */}
            {groupedWorkouts.future.size > 0 && (
              <TimelineSection>
                <SectionHeader>Upcoming</SectionHeader>
                {Array.from(groupedWorkouts.future.entries()).map(
                  ([dateKey, workouts]) => (
                    <DateGroup key={dateKey}>
                      <DateHeader>
                        <div className="date-text">{dateKey}</div>
                        <div className="day-name">
                          {getDayName(workouts[0].scheduledDate.getDay())}
                        </div>
                      </DateHeader>
                      {workouts.map((workout) =>
                        renderWorkoutCard(workout, false)
                      )}
                    </DateGroup>
                  )
                )}
              </TimelineSection>
            )}

            {/* Past Workouts */}
            {groupedWorkouts.past.size > 0 && (
              <TimelineSection>
                <SectionHeader>Past Workouts</SectionHeader>
                {Array.from(groupedWorkouts.past.entries())
                  .reverse()
                  .map(([dateKey, workouts]) => (
                    <DateGroup key={dateKey}>
                      <DateHeader $isPast={true}>
                        <div className="date-text">{dateKey}</div>
                        <div className="day-name">
                          {getDayName(workouts[0].scheduledDate.getDay())}
                        </div>
                      </DateHeader>
                      {workouts.map((workout) =>
                        renderWorkoutCard(workout, true)
                      )}
                    </DateGroup>
                  ))}
              </TimelineSection>
            )}
          </TimelineContainer>
        </Container>
      </PageWrapper>
    </>
  );
};
