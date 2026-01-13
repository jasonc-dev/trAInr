import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import styled, { keyframes } from "styled-components";
import { Container, Card, Button, Input, Stack } from "../components/styled";
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

const LoginCard = styled(Card)`
  max-width: 420px;
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

const ErrorMessage = styled.div`
  background: ${({ theme }) => theme.colors.errorLight};
  color: ${({ theme }) => theme.colors.error};
  padding: ${({ theme }) => theme.spacing.md};
  border-radius: ${({ theme }) => theme.radii.md};
  margin-bottom: ${({ theme }) => theme.spacing.lg};
  font-size: ${({ theme }) => theme.fontSizes.sm};
  text-align: center;
`;

const RegisterLink = styled.div`
  text-align: center;
  margin-top: ${({ theme }) => theme.spacing.xl};
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

const Logo = styled.div`
  text-align: center;
  margin-bottom: ${({ theme }) => theme.spacing.lg};
  font-size: 3rem;
`;

export const Login: React.FC = () => {
  const navigate = useNavigate();
  const { login, loading, error, clearError } = useAuth();
  const [formData, setFormData] = useState({
    username: "",
    password: "",
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.username || !formData.password) {
      return;
    }

    try {
      await login(formData);
      navigate("/dashboard");
    } catch {
      // Error is handled by the hook
    }
  };

  const handleInputChange =
    (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
      clearError();
      setFormData({ ...formData, [field]: e.target.value });
    };

  const isValid = formData.username.trim() && formData.password.trim();

  return (
    <PageWrapper>
      <Container>
        <LoginCard $padding="2.5rem">
          <Logo>💪</Logo>
          <Title>
            Welcome to <span>trAInr</span>
          </Title>
          <Subtitle>Sign in to continue your fitness journey</Subtitle>

          {error && <ErrorMessage>{error}</ErrorMessage>}

          <form onSubmit={handleSubmit}>
            <Stack $gap="1.25rem">
              <Input
                label="Username"
                placeholder="Enter your username"
                value={formData.username}
                onChange={handleInputChange("username")}
                autoComplete="username"
                autoFocus
              />
              <Input
                label="Password"
                type="password"
                placeholder="Enter your password"
                value={formData.password}
                onChange={handleInputChange("password")}
                autoComplete="current-password"
              />
              <Button
                type="submit"
                disabled={!isValid || loading}
                style={{ width: "100%", marginTop: "0.5rem" }}
              >
                {loading ? "Signing in..." : "Sign In"}
              </Button>
            </Stack>
          </form>

          <RegisterLink>
            Don't have an account?
            <Link to="/register">Create one</Link>
          </RegisterLink>
        </LoginCard>
      </Container>
    </PageWrapper>
  );
};
