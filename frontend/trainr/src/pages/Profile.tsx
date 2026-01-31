/**
 * Profile Page
 * User profile with tabs for viewing details and editing settings
 */

import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import {
  Container,
  Card,
  Button,
  Input,
  Select,
  Stack,
  Flex,
} from '../components/styled';
import { EquipmentSelector } from '../components/forms/EquipmentSelector';
import { useProfile } from '../hooks/useProfile';
import { UpdateUserRequest, FitnessLevel, FitnessGoal } from '../types';
import { FITNESS_GOAL_OPTIONS, FITNESS_LEVEL_OPTIONS, WORKOUT_DAYS_OPTIONS } from '../utils';

const PageWrapper = styled.div`
  min-height: 100vh;
  padding: ${({ theme }) => theme.spacing['2xl']} 0;
  background: ${({ theme }) => theme.colors.background};
`;

const ProfileCard = styled(Card)`
  max-width: 800px;
  width: 100%;
  margin: 0 auto;
  padding: ${({ theme }) => theme.spacing['2xl']};

  @media (max-width: ${({ theme }) => theme.breakpoints.md}) {
    padding: ${({ theme }) => theme.spacing.xl};
  }
`;

const Title = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes['3xl']};
  margin-bottom: ${({ theme }) => theme.spacing.xl};
  
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

const TabsContainer = styled.div`
  display: flex;
  gap: ${({ theme }) => theme.spacing.md};
  margin-bottom: ${({ theme }) => theme.spacing.xl};
  border-bottom: 2px solid ${({ theme }) => theme.colors.border};
`;

const Tab = styled.button<{ $active: boolean }>`
  padding: ${({ theme }) => theme.spacing.md} ${({ theme }) => theme.spacing.lg};
  font-size: ${({ theme }) => theme.fontSizes.md};
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  color: ${({ theme, $active }) =>
    $active ? theme.colors.primary : theme.colors.textSecondary};
  background: transparent;
  border: none;
  border-bottom: 2px solid ${({ theme, $active }) =>
    $active ? theme.colors.primary : 'transparent'};
  margin-bottom: -2px;
  cursor: pointer;
  transition: all ${({ theme }) => theme.transitions.fast};

  &:hover {
    color: ${({ theme }) => theme.colors.primary};
  }
`;

const TabContent = styled.div`
  animation: fadeIn 0.3s ease-in;
  padding-top: ${({ theme }) => theme.spacing.lg};

  @keyframes fadeIn {
    from { opacity: 0; }
    to { opacity: 1; }
  }
`;

const InfoRow = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: ${({ theme }) => theme.spacing.lg};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border: 1px solid ${({ theme }) => theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.lg};
  transition: all ${({ theme }) => theme.transitions.fast};
  min-height: 64px;

  &:hover {
    background: ${({ theme }) => theme.colors.surface};
    border-color: ${({ theme }) => theme.colors.borderLight};
    transform: translateX(2px);
  }
`;

const InfoLabel = styled.span`
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  color: ${({ theme }) => theme.colors.textSecondary};
  text-transform: uppercase;
  letter-spacing: 0.5px;
`;

const InfoValue = styled.span`
  font-size: ${({ theme }) => theme.fontSizes.md};
  color: ${({ theme }) => theme.colors.text};
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  text-align: right;
  max-width: 60%;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const ErrorMessage = styled.div`
  background: ${({ theme }) => theme.colors.errorLight};
  color: ${({ theme }) => theme.colors.error};
  padding: ${({ theme }) => theme.spacing.md} ${({ theme }) => theme.spacing.lg};
  border-radius: ${({ theme }) => theme.radii.lg};
  border: 1px solid ${({ theme }) => theme.colors.error};
  margin-bottom: ${({ theme }) => theme.spacing.lg};
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  display: flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.sm};
`;

const SuccessMessage = styled.div`
  background: ${({ theme }) => theme.colors.successLight};
  color: ${({ theme }) => theme.colors.success};
  padding: ${({ theme }) => theme.spacing.md} ${({ theme }) => theme.spacing.lg};
  border-radius: ${({ theme }) => theme.radii.lg};
  border: 1px solid ${({ theme }) => theme.colors.success};
  margin-bottom: ${({ theme }) => theme.spacing.lg};
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  display: flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.sm};
`;

const LoadingSpinner = styled.div`
  text-align: center;
  padding: ${({ theme }) => theme.spacing['2xl']};
  color: ${({ theme }) => theme.colors.textSecondary};
`;

const FormSection = styled.div`
  margin-top: ${({ theme }) => theme.spacing.xl};
`;

const NameFieldsWrapper = styled(Flex)`
  > * {
    flex: 1;
    min-width: 0;
  }

  @media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
    flex-direction: column;
    
    > * {
      flex: none;
      width: 100%;
    }
  }
`;

const ButtonGroup = styled(Flex)`
  margin-top: ${({ theme }) => theme.spacing['2xl']};
  padding-top: ${({ theme }) => theme.spacing.xl};
  border-top: 1px solid ${({ theme }) => theme.colors.border};
  gap: ${({ theme }) => theme.spacing.md};
  
  button {
    min-width: 140px;
  }
`;

type TabType = 'profile' | 'settings';

export const Profile: React.FC = () => {
  const navigate = useNavigate();
  const { user, loading, error, fetchProfile, updateProfile, clearError } = useProfile();
  const [activeTab, setActiveTab] = useState<TabType>('profile');
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [formData, setFormData] = useState<UpdateUserRequest>({
    firstName: '',
    lastName: '',
    fitnessLevel: FitnessLevel.Beginner,
    primaryGoal: FitnessGoal.GeneralFitness,
    workoutDaysPerWeek: 3,
    equipmentPreferences: [],
  });

  useEffect(() => {
    fetchProfile();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (user) {
      setFormData({
        firstName: user.firstName,
        lastName: user.lastName,
        fitnessLevel: user.fitnessLevel,
        primaryGoal: user.primaryGoal,
        workoutDaysPerWeek: user.workoutDaysPerWeek,
        equipmentPreferences: user.equipmentPreferences || [],
      });
    }
  }, [user]);

  const handleInputChange = (field: keyof UpdateUserRequest, value: any) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
    clearError();
    setSuccessMessage(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await updateProfile(formData);
      setSuccessMessage('Profile updated successfully!');
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      // Error is handled by the hook
    }
  };

  const getFitnessLevelLabel = (level: FitnessLevel): string => {
    return FITNESS_LEVEL_OPTIONS.find((opt) => opt.value === level)?.label || 'Unknown';
  };

  const getFitnessGoalLabel = (goal: FitnessGoal): string => {
    return FITNESS_GOAL_OPTIONS.find((opt) => opt.value === goal)?.label || 'Unknown';
  };

  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  if (loading && !user) {
    return (
      <PageWrapper>
        <Container>
          <ProfileCard>
            <LoadingSpinner>Loading profile...</LoadingSpinner>
          </ProfileCard>
        </Container>
      </PageWrapper>
    );
  }

  if (!user) {
    return (
      <PageWrapper>
        <Container>
          <ProfileCard>
            <ErrorMessage>
              Failed to load profile. Please try refreshing the page.
            </ErrorMessage>
          </ProfileCard>
        </Container>
      </PageWrapper>
    );
  }

  return (
    <PageWrapper>
      <Container>
        <ProfileCard>
          <Title>
            <span>Profile</span>
          </Title>

          <TabsContainer>
            <Tab
              $active={activeTab === 'profile'}
              onClick={() => setActiveTab('profile')}
            >
              Profile
            </Tab>
            <Tab
              $active={activeTab === 'settings'}
              onClick={() => setActiveTab('settings')}
            >
              Settings
            </Tab>
          </TabsContainer>

          {error && <ErrorMessage>{error}</ErrorMessage>}
          {successMessage && <SuccessMessage>{successMessage}</SuccessMessage>}

          {activeTab === 'profile' && (
            <TabContent>
              <Stack $gap="lg">
                <InfoRow>
                  <InfoLabel>Username</InfoLabel>
                  <InfoValue>{user.username}</InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Email</InfoLabel>
                  <InfoValue>{user.email}</InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Full Name</InfoLabel>
                  <InfoValue>{`${user.firstName} ${user.lastName}`}</InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Date of Birth</InfoLabel>
                  <InfoValue>{formatDate(user.dateOfBirth)}</InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Fitness Level</InfoLabel>
                  <InfoValue>{getFitnessLevelLabel(user.fitnessLevel)}</InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Primary Goal</InfoLabel>
                  <InfoValue>{getFitnessGoalLabel(user.primaryGoal)}</InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Workout Days Per Week</InfoLabel>
                  <InfoValue>{user.workoutDaysPerWeek}</InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Equipment</InfoLabel>
                  <InfoValue>
                    {user.equipmentPreferences?.length > 0
                      ? user.equipmentPreferences.join(', ')
                      : 'None selected'}
                  </InfoValue>
                </InfoRow>
                <InfoRow>
                  <InfoLabel>Member Since</InfoLabel>
                  <InfoValue>{formatDate(user.createdAt)}</InfoValue>
                </InfoRow>
              </Stack>
            </TabContent>
          )}

          {activeTab === 'settings' && (
            <TabContent>
              <form onSubmit={handleSubmit}>
                <Stack $gap="lg">
                  <NameFieldsWrapper $gap="md">
                    <Input
                      label="First Name"
                      value={formData.firstName}
                      onChange={(e) => handleInputChange('firstName', e.target.value)}
                      required
                    />
                    <Input
                      label="Last Name"
                      value={formData.lastName}
                      onChange={(e) => handleInputChange('lastName', e.target.value)}
                      required
                    />
                  </NameFieldsWrapper>

                  <Select
                    label="Fitness Level"
                    value={formData.fitnessLevel.toString()}
                    onChange={(e) =>
                      handleInputChange('fitnessLevel', parseInt(e.target.value))
                    }
                    options={FITNESS_LEVEL_OPTIONS.map((opt) => ({
                      value: opt.value.toString(),
                      label: opt.label,
                    }))}
                  />

                  <Select
                    label="Primary Goal"
                    value={formData.primaryGoal.toString()}
                    onChange={(e) =>
                      handleInputChange('primaryGoal', parseInt(e.target.value))
                    }
                    options={FITNESS_GOAL_OPTIONS.map((opt) => ({
                      value: opt.value.toString(),
                      label: opt.label,
                    }))}
                  />

                  <Select
                    label="Workout Days Per Week"
                    value={formData.workoutDaysPerWeek.toString()}
                    onChange={(e) =>
                      handleInputChange('workoutDaysPerWeek', parseInt(e.target.value))
                    }
                    options={WORKOUT_DAYS_OPTIONS.map((opt) => ({
                      value: opt.value,
                      label: opt.label,
                    }))}
                  />

                  <FormSection>
                    <EquipmentSelector
                      selected={formData.equipmentPreferences || []}
                      onChange={(selected) =>
                        handleInputChange('equipmentPreferences', selected)
                      }
                    />
                  </FormSection>

                  <ButtonGroup $gap="md" $justify="flex-start">
                    <Button type="submit" variant="primary" size="lg" disabled={loading}>
                      {loading ? 'Saving...' : 'Save Changes'}
                    </Button>
                    <Button
                      type="button"
                      variant="ghost"
                      size="lg"
                      onClick={() => navigate(-1)}
                    >
                      Cancel
                    </Button>
                  </ButtonGroup>
                </Stack>
              </form>
            </TabContent>
          )}
        </ProfileCard>
      </Container>
    </PageWrapper>
  );
};
