import React from "react";
import styled from "styled-components";
import { Link } from "react-router-dom";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  BarChart,
  Bar,
} from "recharts";
import {
  Container,
  PageWrapper,
  Grid,
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  StatCard,
  Button,
  ProgressBar,
  Flex,
  Stack,
  Badge,
} from "../components/styled";
import { Navigation } from "../components/styled/Navigation";
import { useUser, useDashboard } from "../hooks";
import { ExerciseName } from "../components/styled/ExerciseName";

const WelcomeSection = styled.div`
  margin-bottom: ${({ theme }) => theme.spacing["2xl"]};
`;

const WelcomeTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes["3xl"]};
  margin-bottom: ${({ theme }) => theme.spacing.xs};

  span {
    color: ${({ theme }) => theme.colors.primary};
  }
`;

const WelcomeSubtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
`;

const ActiveProgrammeCard = styled(Card)`
  background: linear-gradient(
    135deg,
    ${({ theme }) => theme.colors.surface} 0%,
    rgba(0, 207, 193, 0.1) 100%
  );
  border-color: rgba(0, 207, 193, 0.3);
`;

const ChartContainer = styled.div`
  height: 250px;
  margin-top: ${({ theme }) => theme.spacing.md};
`;

const ExerciseItem = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: ${({ theme }) => theme.spacing.md};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border-radius: ${({ theme }) => theme.radii.md};
  margin-bottom: ${({ theme }) => theme.spacing.sm};

  &:last-child {
    margin-bottom: 0;
  }
`;

const ExerciseVolume = styled.span`
  color: ${({ theme }) => theme.colors.primary};
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
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

// const formatDuration = (duration: string): string => {
//   // Parse ISO duration or time string
//   const hours = Math.floor(parseInt(duration) / 3600) || 0;
//   const minutes = Math.floor((parseInt(duration) % 3600) / 60) || 0;
//   if (hours > 0) return `${hours}h ${minutes}m`;
//   return `${minutes}m`;
// };

export const Dashboard: React.FC = () => {
  const { user } = useUser();
  const { dashboard, loading } = useDashboard(user?.id);
  // const { activeProgramme } = useProgrammes(user?.id);

  if (loading) {
    return (
      <>
        <Navigation />
        <PageWrapper>
          <Container>
            <div style={{ textAlign: "center", padding: "4rem" }}>
              Loading your dashboard...
            </div>
          </Container>
        </PageWrapper>
      </>
    );
  }

  const hasActiveProgramme = dashboard?.activeProgramme;
  const weeklyProgress = dashboard?.weeklyProgress || [];

  const volumeChartData = weeklyProgress.map((w) => ({
    name: `Week ${w.weekNumber}`,
    volume: w.totalVolume,
    reps: w.totalReps,
  }));

  const intensityChartData = weeklyProgress.map((w) => ({
    name: `Week ${w.weekNumber}`,
    intensity: w.averageIntensity,
  }));

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <WelcomeSection>
            <WelcomeTitle>
              Welcome back, <span>{user?.firstName || "Athlete"}</span>
            </WelcomeTitle>
            <WelcomeSubtitle>
              {hasActiveProgramme
                ? "Let's keep crushing your goals!"
                : "Ready to start your fitness journey?"}
            </WelcomeSubtitle>
          </WelcomeSection>

          {!hasActiveProgramme ? (
            <Card>
              <EmptyState>
                <div className="icon">🏋️</div>
                <h3>No Active Programme</h3>
                <p>
                  Create or select a programme to start tracking your workouts
                </p>
                <Link to="/programmes">
                  <Button size="lg">Browse Programmes</Button>
                </Link>
              </EmptyState>
            </Card>
          ) : (
            <>
              {/* Active Programme Summary */}
              <ActiveProgrammeCard
                $padding="1.5rem"
                style={{ marginBottom: "2rem" }}
              >
                <CardHeader>
                  <div>
                    <CardTitle>{dashboard?.activeProgramme?.name}</CardTitle>
                    <p style={{ color: "#A0AEC0", fontSize: "0.875rem" }}>
                      {dashboard?.activeProgramme?.description}
                    </p>
                  </div>
                  <Badge $variant="primary">Active</Badge>
                </CardHeader>
                <Flex $align="center" $gap="2rem" style={{ marginTop: "1rem" }}>
                  <div style={{ flex: 1 }}>
                    <ProgressBar
                      value={
                        dashboard?.activeProgramme?.progressPercentage || 0
                      }
                      variant="primary"
                      showLabel
                    />
                  </div>
                  <Link to="/workout">
                    <Button>Start Workout</Button>
                  </Link>
                </Flex>
              </ActiveProgrammeCard>

              {/* Stats Overview */}
              <Grid $columns={4} $gap="1rem" style={{ marginBottom: "2rem" }}>
                <StatCard>
                  <div className="stat-value">
                    {dashboard?.overallStats.totalWorkoutsCompleted || 0}
                  </div>
                  <div className="stat-label">Workouts</div>
                </StatCard>
                <StatCard>
                  <div className="stat-value">
                    {dashboard?.overallStats.totalSetsCompleted || 0}
                  </div>
                  <div className="stat-label">Total Sets</div>
                </StatCard>
                <StatCard>
                  <div className="stat-value">
                    {Math.round(
                      dashboard?.overallStats.totalVolumeLifted || 0
                    ).toLocaleString()}
                  </div>
                  <div className="stat-label">Volume (kg)</div>
                </StatCard>
                <StatCard>
                  <div className="stat-value">
                    {dashboard?.overallStats.currentStreak || 0}
                  </div>
                  <div className="stat-label">Day Streak</div>
                </StatCard>
              </Grid>

              {/* Charts */}
              <Grid $columns={2} $gap="1.5rem" style={{ marginBottom: "2rem" }}>
                <Card>
                  <CardHeader>
                    <CardTitle>Weekly Volume</CardTitle>
                  </CardHeader>
                  <ChartContainer>
                    {volumeChartData.length > 0 ? (
                      <ResponsiveContainer width="100%" height="100%">
                        <BarChart data={volumeChartData}>
                          <CartesianGrid
                            strokeDasharray="3 3"
                            stroke="#2D3748"
                          />
                          <XAxis
                            dataKey="name"
                            stroke="#A0AEC0"
                            fontSize={12}
                          />
                          <YAxis stroke="#A0AEC0" fontSize={12} />
                          <Tooltip
                            contentStyle={{
                              background: "#1E2740",
                              border: "1px solid #2D3748",
                              borderRadius: "8px",
                            }}
                          />
                          <Bar
                            dataKey="volume"
                            fill="#00CFC1"
                            radius={[4, 4, 0, 0]}
                          />
                        </BarChart>
                      </ResponsiveContainer>
                    ) : (
                      <div
                        style={{
                          textAlign: "center",
                          color: "#64748B",
                          paddingTop: "5rem",
                        }}
                      >
                        Complete workouts to see your volume progress
                      </div>
                    )}
                  </ChartContainer>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle>Intensity Trend</CardTitle>
                  </CardHeader>
                  <ChartContainer>
                    {intensityChartData.length > 0 ? (
                      <ResponsiveContainer width="100%" height="100%">
                        <LineChart data={intensityChartData}>
                          <CartesianGrid
                            strokeDasharray="3 3"
                            stroke="#2D3748"
                          />
                          <XAxis
                            dataKey="name"
                            stroke="#A0AEC0"
                            fontSize={12}
                          />
                          <YAxis
                            stroke="#A0AEC0"
                            fontSize={12}
                            domain={[0, 5]}
                          />
                          <Tooltip
                            contentStyle={{
                              background: "#1E2740",
                              border: "1px solid #2D3748",
                              borderRadius: "8px",
                            }}
                          />
                          <Line
                            type="monotone"
                            dataKey="intensity"
                            stroke="#FF6B4A"
                            strokeWidth={3}
                            dot={{ fill: "#FF6B4A", strokeWidth: 2 }}
                          />
                        </LineChart>
                      </ResponsiveContainer>
                    ) : (
                      <div
                        style={{
                          textAlign: "center",
                          color: "#64748B",
                          paddingTop: "5rem",
                        }}
                      >
                        Log workout intensity to see trends
                      </div>
                    )}
                  </ChartContainer>
                </Card>
              </Grid>

              {/* Current Week & Top Exercises */}
              <Grid $columns={2} $gap="1.5rem">
                <Card>
                  <CardHeader>
                    <CardTitle>This Week</CardTitle>
                  </CardHeader>
                  <CardContent>
                    {dashboard?.currentWeekMetrics ? (
                      <Stack $gap="1rem">
                        <Flex $justify="space-between">
                          <span style={{ color: "#A0AEC0" }}>
                            Workouts Completed
                          </span>
                          <span>
                            {dashboard.currentWeekMetrics.workoutsCompleted} /{" "}
                            {dashboard.currentWeekMetrics.workoutsPlanned}
                          </span>
                        </Flex>
                        <Flex $justify="space-between">
                          <span style={{ color: "#A0AEC0" }}>
                            Sets Completed
                          </span>
                          <span>
                            {dashboard.currentWeekMetrics.totalSetsCompleted}
                          </span>
                        </Flex>
                        <Flex $justify="space-between">
                          <span style={{ color: "#A0AEC0" }}>Total Volume</span>
                          <span>
                            {Math.round(
                              dashboard.currentWeekMetrics.totalVolume
                            ).toLocaleString()}{" "}
                            kg
                          </span>
                        </Flex>
                        <Flex $justify="space-between">
                          <span style={{ color: "#A0AEC0" }}>Total Reps</span>
                          <span>{dashboard.currentWeekMetrics.totalReps}</span>
                        </Flex>
                      </Stack>
                    ) : (
                      <div
                        style={{
                          textAlign: "center",
                          color: "#64748B",
                          padding: "2rem",
                        }}
                      >
                        Start a workout to see your weekly stats
                      </div>
                    )}
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle>Top Exercises</CardTitle>
                  </CardHeader>
                  <CardContent>
                    {dashboard?.topExercises &&
                    dashboard.topExercises.length > 0 ? (
                      <div>
                        {dashboard.topExercises.slice(0, 5).map((exercise) => (
                          <ExerciseItem key={exercise.exerciseId}>
                            <ExerciseName>{exercise.exerciseName}</ExerciseName>
                            <ExerciseVolume>
                              {Math.round(
                                exercise.totalVolume
                              ).toLocaleString()}{" "}
                              kg
                            </ExerciseVolume>
                          </ExerciseItem>
                        ))}
                      </div>
                    ) : (
                      <div
                        style={{
                          textAlign: "center",
                          color: "#64748B",
                          padding: "2rem",
                        }}
                      >
                        Complete exercises to see your top performers
                      </div>
                    )}
                  </CardContent>
                </Card>
              </Grid>
            </>
          )}
        </Container>
      </PageWrapper>
    </>
  );
};
