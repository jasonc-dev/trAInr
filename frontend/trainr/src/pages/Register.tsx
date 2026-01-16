import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
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
import { FitnessLevel, FitnessGoal, RegisterRequest } from "../types";
import { useAuth } from "../hooks/useAuth";

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

const RegisterCard = styled(Card)`
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

const ErrorMessage = styled.div`
  background: ${({ theme }) => theme.colors.errorLight};
  color: ${({ theme }) => theme.colors.error};
  padding: ${({ theme }) => theme.spacing.md};
  border-radius: ${({ theme }) => theme.radii.md};
  margin-bottom: ${({ theme }) => theme.spacing.lg};
  font-size: ${({ theme }) => theme.fontSizes.sm};
  text-align: center;
`;

const LoginLink = styled.div`
  text-align: center;
  margin-top: ${({ theme }) => theme.spacing.lg};
  color: ${({ theme }) => theme.colors.textSecondary};
  font-size: ${({ theme }) => theme.fontSizes.sm};

  a {
    color: ${({ theme }) => theme.colors.primary};
    text-decoration: none;
    font-weight: ${({ theme }) => theme.fontWeights.semibold};
    margin-left: ${({ theme }) => theme.spacing.xs};
    transition: color ${({ theme }) => theme.transitions.fast};

    &:hover {
      color: ${({ theme }) => theme.colors.primaryLight};
    }
  }
`;

const PasswordHint = styled.span`
  font-size: ${({ theme }) => theme.fontSizes.xs};
  color: ${({ theme }) => theme.colors.textMuted};
  margin-top: 4px;
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

export const Register: React.FC = () => {
  const navigate = useNavigate();
  const { register, loading, error, clearError } = useAuth();
  const [step, setStep] = useState(1);
  const [formData, setFormData] = useState<RegisterRequest>({
    username: "",
    password: "",
    email: "",
    firstName: "",
    lastName: "",
    dateOfBirth: "", // ISO date string (YYYY-MM-DD)
    fitnessLevel: FitnessLevel.Beginner,
    primaryGoal: FitnessGoal.GeneralFitness,
    workoutDaysPerWeek: 3,
  });
  const [confirmPassword, setConfirmPassword] = useState("");

  const handleNext = (e: React.FormEvent) => {
    e.preventDefault();
    if (step < 3) setStep(step + 1);
  };

  const handleBack = () => {
    if (step > 1) setStep(step - 1);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await register(formData);
      navigate("/dashboard");
    } catch {
      // Error is handled by the hook
    }
  };

  const handleInputChange =
    (field: keyof RegisterRequest) =>
    (e: React.ChangeEvent<HTMLInputElement>) => {
      clearError();
      // dateOfBirth is stored as ISO string (YYYY-MM-DD)
      setFormData({ ...formData, [field]: e.target.value });
    };

  const passwordsMatch = formData.password === confirmPassword;
  const passwordLongEnough = formData.password.length >= 6;

  const isStepValid = () => {
    switch (step) {
      case 1:
        return (
          formData.username.trim() &&
          formData.password.trim() &&
          confirmPassword.trim() &&
          passwordsMatch &&
          passwordLongEnough &&
          formData.email.trim() &&
          formData.firstName.trim() &&
          formData.lastName.trim() &&
          formData.dateOfBirth.trim() // Ensure date is selected
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
        <RegisterCard $padding="2rem">
          <Title>
            Join <span>trAInr</span>
          </Title>
          <Subtitle>
            Create your account and start your fitness journey
          </Subtitle>

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

          {error && <ErrorMessage>{error}</ErrorMessage>}

          {step === 1 && (
            <form id="step-1" onSubmit={handleNext}>
              <Stack $gap="1.25rem">
                <Input
                  label="Username"
                  placeholder="Choose a username"
                  value={formData.username}
                  onChange={handleInputChange("username")}
                  autoComplete="username"
                  autoFocus
                  required
                />
                <div>
                  <Input
                    label="Password"
                    type="password"
                    placeholder="Create a password"
                    value={formData.password}
                    onChange={handleInputChange("password")}
                    autoComplete="new-password"
                    error={
                      formData.password && !passwordLongEnough
                        ? "Password must be at least 6 characters"
                        : undefined
                    }
                    required
                  />
                  <PasswordHint>Minimum 6 characters</PasswordHint>
                </div>
                <Input
                  label="Confirm Password"
                  type="password"
                  placeholder="Confirm your password"
                  value={confirmPassword}
                  onChange={(e) => {
                    clearError();
                    setConfirmPassword(e.target.value);
                  }}
                  autoComplete="new-password"
                  error={
                    confirmPassword && !passwordsMatch
                      ? "Passwords do not match"
                      : undefined
                  }
                  required
                />
                <Input
                  label="Email"
                  type="email"
                  placeholder="john@example.com"
                  value={formData.email}
                  onChange={handleInputChange("email")}
                  autoComplete="email"
                  required
                />
                <Flex $gap="1rem">
                  <Input
                    label="First Name"
                    placeholder="John"
                    value={formData.firstName}
                    onChange={handleInputChange("firstName")}
                    autoComplete="given-name"
                    required
                  />
                  <Input
                    label="Last Name"
                    placeholder="Doe"
                    value={formData.lastName}
                    onChange={handleInputChange("lastName")}
                    autoComplete="family-name"
                    required
                  />
                </Flex>
                <Input
                  label="Date of Birth"
                  type="date"
                  value={formData.dateOfBirth}
                  onChange={handleInputChange("dateOfBirth")}
                  required
                />
              </Stack>
            </form>
          )}

          {step === 2 && (
            <form id="step-2" onSubmit={handleNext}>
              <Stack $gap="1.5rem">
                <div>
                  <h4 style={{ marginBottom: "1rem" }}>
                    What's your fitness level?
                  </h4>
                  <OptionGrid>
                    {fitnessLevelOptions.map((option) => (
                      <OptionCard
                        key={option.value}
                        type="button"
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
                        type="button"
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
            </form>
          )}

          {step === 3 && (
            <form id="step-3" onSubmit={handleSubmit}>
              <Stack $gap="1.5rem">
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
                    required
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
                    <strong>Username:</strong> {formData.username}
                    <br />
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
            </form>
          )}

          <Flex $justify="space-between" style={{ marginTop: "2rem" }}>
            <Button variant="ghost" onClick={handleBack} disabled={step === 1}>
              Back
            </Button>
            {step < 3 ? (
              <Button type="submit" form={`step-${step}`} disabled={!isStepValid()}>
                Continue
              </Button>
            ) : (
              <Button
                type="submit"
                form="step-3"
                disabled={!isStepValid() || loading}
              >
                {loading ? "Creating Account..." : "Create Account"}
              </Button>
            )}
          </Flex>

          <LoginLink>
            Already have an account?
            <Link to="/login">Sign in</Link>
          </LoginLink>
        </RegisterCard>
      </Container>
    </PageWrapper>
  );
};
