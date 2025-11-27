import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import styled, { keyframes } from "styled-components";
import {
  Container,
  Card,
  Button,
  Input,
  Select,
  Stack,
  Flex,
} from "../components/styled";
import { FitnessLevel, FitnessGoal, CreateUserRequest } from "../types";
import { useUser } from "../hooks";

const fadeIn = keyframes`
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
`;

const PageWrapper = styled.div`
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: ${({ theme }) => theme.spacing["2xl"]} 0;
  background: radial-gradient(
      ellipse at 30% 20%,
      rgba(0, 207, 193, 0.12) 0%,
      transparent 50%
    ),
    radial-gradient(
      ellipse at 70% 80%,
      rgba(124, 77, 255, 0.08) 0%,
      transparent 50%
    ),
    ${({ theme }) => theme.colors.background};
`;

const OnboardingCard = styled(Card)`
  max-width: 600px;
  width: 100%;
  animation: ${fadeIn} 0.5s ease-out;
`;

const Title = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes["4xl"]};
  text-align: center;
  margin-bottom: ${({ theme }) => theme.spacing.xs};

  span {
    background: linear-gradient(
      135deg,
      ${({ theme }) => theme.colors.primary} 0%,
      ${({ theme }) => theme.colors.secondary} 100%
    );
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }
`;

const Subtitle = styled.p`
  text-align: center;
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing["2xl"]};
`;

const StepIndicator = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  gap: ${({ theme }) => theme.spacing.sm};
  margin-bottom: ${({ theme }) => theme.spacing.xl};
`;

const Step = styled.div<{ $active: boolean; $completed: boolean }>`
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  background: ${({ theme, $active, $completed }) =>
    $active || $completed ? theme.colors.primary : theme.colors.surface};
  color: ${({ theme, $active, $completed }) =>
    $active || $completed ? theme.colors.background : theme.colors.textMuted};
  transition: all ${({ theme }) => theme.transitions.normal};
`;

const StepLine = styled.div<{ $completed: boolean }>`
  width: 60px;
  height: 2px;
  background: ${({ theme, $completed }) =>
    $completed ? theme.colors.primary : theme.colors.border};
  transition: all ${({ theme }) => theme.transitions.normal};
`;

const OptionGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: ${({ theme }) => theme.spacing.md};

  @media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
    grid-template-columns: 1fr;
  }
`;

const OptionCard = styled.button<{ $selected: boolean }>`
  padding: ${({ theme }) => theme.spacing.lg};
  background: ${({ theme, $selected }) =>
    $selected ? theme.colors.primaryGhost : theme.colors.surface};
  border: 2px solid
    ${({ theme, $selected }) =>
      $selected ? theme.colors.primary : theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.lg};
  text-align: left;
  cursor: pointer;
  transition: all ${({ theme }) => theme.transitions.fast};

  &:hover {
    border-color: ${({ theme }) => theme.colors.primary};
  }

  .icon {
    font-size: 2rem;
    margin-bottom: ${({ theme }) => theme.spacing.sm};
  }

  .title {
    font-size: ${({ theme }) => theme.fontSizes.lg};
    font-weight: ${({ theme }) => theme.fontWeights.semibold};
    color: ${({ theme }) => theme.colors.text};
    margin-bottom: ${({ theme }) => theme.spacing.xs};
  }

  .description {
    font-size: ${({ theme }) => theme.fontSizes.sm};
    color: ${({ theme }) => theme.colors.textSecondary};
  }
`;

const fitnessLevelOptions = [
  {
    value: FitnessLevel.Beginner,
    label: "Beginner",
    icon: "🌱",
    desc: "New to fitness or returning after a break",
  },
  {
    value: FitnessLevel.Intermediate,
    label: "Intermediate",
    icon: "💪",
    desc: "1-3 years of consistent training",
  },
  {
    value: FitnessLevel.Advanced,
    label: "Advanced",
    icon: "🔥",
    desc: "3+ years with solid foundation",
  },
  {
    value: FitnessLevel.Elite,
    label: "Elite",
    icon: "⚡",
    desc: "Competitive or professional athlete",
  },
];

const fitnessGoalOptions = [
  {
    value: FitnessGoal.BuildMuscle,
    label: "Build Muscle",
    icon: "🏋️",
    desc: "Increase size and strength",
  },
  {
    value: FitnessGoal.LoseWeight,
    label: "Lose Weight",
    icon: "🎯",
    desc: "Reduce body fat percentage",
  },
  {
    value: FitnessGoal.ImproveEndurance,
    label: "Improve Endurance",
    icon: "🏃",
    desc: "Better stamina and cardio",
  },
  {
    value: FitnessGoal.IncreaseStrength,
    label: "Increase Strength",
    icon: "💎",
    desc: "Lift heavier weights",
  },
  {
    value: FitnessGoal.GeneralFitness,
    label: "General Fitness",
    icon: "✨",
    desc: "Overall health and wellness",
  },
];

const daysOptions = [
  { value: "2", label: "2 days per week" },
  { value: "3", label: "3 days per week" },
  { value: "4", label: "4 days per week" },
  { value: "5", label: "5 days per week" },
  { value: "6", label: "6 days per week" },
];

export const Onboarding: React.FC = () => {
  const navigate = useNavigate();
  const { createUser } = useUser();
  const [step, setStep] = useState(1);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<CreateUserRequest>({
    email: "",
    firstName: "",
    lastName: "",
    dateOfBirth: new Date(),
    fitnessLevel: FitnessLevel.Beginner,
    primaryGoal: FitnessGoal.GeneralFitness,
    workoutDaysPerWeek: 3,
  });

  const handleNext = () => {
    if (step < 3) setStep(step + 1);
  };

  const handleBack = () => {
    if (step > 1) setStep(step - 1);
  };

  const handleSubmit = async () => {
    try {
      setLoading(true);
      await createUser(formData);
      navigate("/dashboard");
    } catch (err) {
      console.error("Failed to create user:", err);
    } finally {
      setLoading(false);
    }
  };

  const isStepValid = () => {
    switch (step) {
      case 1:
        return (
          formData.email &&
          formData.firstName &&
          formData.lastName &&
          formData.dateOfBirth
        );
      case 2:
        return formData.fitnessLevel && formData.primaryGoal;
      case 3:
        return (
          formData.workoutDaysPerWeek >= 2 && formData.workoutDaysPerWeek <= 6
        );
      default:
        return false;
    }
  };

  return (
    <PageWrapper>
      <Container>
        <OnboardingCard $padding="2rem">
          <Title>
            Welcome to <span>trAInr</span>
          </Title>
          <Subtitle>Let's set up your personalized fitness journey</Subtitle>

          <StepIndicator>
            <Step $active={step === 1} $completed={step > 1}>
              1
            </Step>
            <StepLine $completed={step > 1} />
            <Step $active={step === 2} $completed={step > 2}>
              2
            </Step>
            <StepLine $completed={step > 2} />
            <Step $active={step === 3} $completed={step > 3}>
              3
            </Step>
          </StepIndicator>

          {step === 1 && (
            <Stack gap="1.25rem">
              <Flex gap="1rem">
                <Input
                  label="First Name"
                  placeholder="John"
                  value={formData.firstName}
                  onChange={(e) =>
                    setFormData({ ...formData, firstName: e.target.value })
                  }
                />
                <Input
                  label="Last Name"
                  placeholder="Doe"
                  value={formData.lastName}
                  onChange={(e) =>
                    setFormData({ ...formData, lastName: e.target.value })
                  }
                />
              </Flex>
              <Input
                label="Email"
                type="email"
                placeholder="john@example.com"
                value={formData.email}
                onChange={(e) =>
                  setFormData({ ...formData, email: e.target.value })
                }
              />
              <Input
                label="Date of Birth"
                type="date"
                value={formData.dateOfBirth.toISOString().split("T")[0]}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    dateOfBirth: new Date(e.target.value),
                  })
                }
              />
            </Stack>
          )}

          {step === 2 && (
            <Stack gap="1.5rem">
              <div>
                <h4 style={{ marginBottom: "1rem" }}>
                  What's your fitness level?
                </h4>
                <OptionGrid>
                  {fitnessLevelOptions.map((option) => (
                    <OptionCard
                      key={option.value}
                      $selected={formData.fitnessLevel === option.value}
                      onClick={() =>
                        setFormData({ ...formData, fitnessLevel: option.value })
                      }
                    >
                      <div className="icon">{option.icon}</div>
                      <div className="title">{option.label}</div>
                      <div className="description">{option.desc}</div>
                    </OptionCard>
                  ))}
                </OptionGrid>
              </div>
              <div>
                <h4 style={{ marginBottom: "1rem" }}>
                  What's your primary goal?
                </h4>
                <OptionGrid>
                  {fitnessGoalOptions.map((option) => (
                    <OptionCard
                      key={option.value}
                      $selected={formData.primaryGoal === option.value}
                      onClick={() =>
                        setFormData({ ...formData, primaryGoal: option.value })
                      }
                    >
                      <div className="icon">{option.icon}</div>
                      <div className="title">{option.label}</div>
                      <div className="description">{option.desc}</div>
                    </OptionCard>
                  ))}
                </OptionGrid>
              </div>
            </Stack>
          )}

          {step === 3 && (
            <Stack gap="1.5rem">
              <div>
                <h4 style={{ marginBottom: "1rem" }}>
                  How many days can you train?
                </h4>
                <Select
                  options={daysOptions}
                  value={formData.workoutDaysPerWeek.toString()}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      workoutDaysPerWeek: parseInt(e.target.value),
                    })
                  }
                />
              </div>
              <Card
                $padding="1.5rem"
                style={{
                  background: "rgba(0, 207, 193, 0.1)",
                  border: "1px solid rgba(0, 207, 193, 0.3)",
                }}
              >
                <h4 style={{ color: "#00CFC1", marginBottom: "0.5rem" }}>
                  Your Profile Summary
                </h4>
                <p style={{ color: "#A0AEC0", fontSize: "0.875rem" }}>
                  <strong>Name:</strong> {formData.firstName}{" "}
                  {formData.lastName}
                  <br />
                  <strong>Level:</strong>{" "}
                  {
                    fitnessLevelOptions.find(
                      (o) => o.value === formData.fitnessLevel
                    )?.label
                  }
                  <br />
                  <strong>Goal:</strong>{" "}
                  {
                    fitnessGoalOptions.find(
                      (o) => o.value === formData.primaryGoal
                    )?.label
                  }
                  <br />
                  <strong>Training Days:</strong> {formData.workoutDaysPerWeek}{" "}
                  per week
                </p>
              </Card>
            </Stack>
          )}

          <Flex justify="space-between" style={{ marginTop: "2rem" }}>
            <Button variant="ghost" onClick={handleBack} disabled={step === 1}>
              Back
            </Button>
            {step < 3 ? (
              <Button onClick={handleNext} disabled={!isStepValid()}>
                Continue
              </Button>
            ) : (
              <Button
                onClick={handleSubmit}
                disabled={!isStepValid() || loading}
              >
                {loading ? "Creating..." : "Start Training"}
              </Button>
            )}
          </Flex>
        </OnboardingCard>
      </Container>
    </PageWrapper>
  );
};
