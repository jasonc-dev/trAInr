import React, { useState } from 'react';
import styled from 'styled-components';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  PageWrapper,
  Grid,
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  CardFooter,
  Button,
  Input,
  Select,
  Badge,
  Flex,
  Stack,
  ProgressBar,
} from '../components/styled';
import { Navigation } from '../components/Navigation';
import { useUser, useProgrammes } from '../hooks';
import { CreateProgrammeRequest } from '../types';

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes['3xl']};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

const PageSubtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing['2xl']};
`;

const TabContainer = styled.div`
  display: flex;
  gap: ${({ theme }) => theme.spacing.sm};
  margin-bottom: ${({ theme }) => theme.spacing.xl};
  border-bottom: 1px solid ${({ theme }) => theme.colors.border};
  padding-bottom: ${({ theme }) => theme.spacing.sm};
`;

const Tab = styled.button<{ $active: boolean }>`
  padding: ${({ theme }) => theme.spacing.sm} ${({ theme }) => theme.spacing.lg};
  background: ${({ $active, theme }) => 
    $active ? theme.colors.primaryGhost : 'transparent'};
  color: ${({ $active, theme }) => 
    $active ? theme.colors.primary : theme.colors.textSecondary};
  border: none;
  border-radius: ${({ theme }) => theme.radii.md};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  cursor: pointer;
  transition: all ${({ theme }) => theme.transitions.fast};
  
  &:hover {
    color: ${({ theme }) => theme.colors.text};
    background: ${({ theme }) => theme.colors.surface};
  }
`;

const ProgrammeCard = styled(Card)<{ $isActive?: boolean }>`
  ${({ $isActive, theme }) => $isActive && `
    border-color: ${theme.colors.primary};
    box-shadow: ${theme.shadows.glow};
  `}
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
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
`;

const ModalTitle = styled.h2`
  font-size: ${({ theme }) => theme.fontSizes['2xl']};
  margin-bottom: ${({ theme }) => theme.spacing.lg};
`;

const durationOptions = [
  { value: '4', label: '4 weeks' },
  { value: '5', label: '5 weeks' },
  { value: '6', label: '6 weeks' },
  { value: '7', label: '7 weeks' },
  { value: '8', label: '8 weeks' },
  { value: '9', label: '9 weeks' },
  { value: '10', label: '10 weeks' },
];

export const Programmes: React.FC = () => {
  const navigate = useNavigate();
  const { user } = useUser();
  const { programmes, activeProgramme, createProgramme, deleteProgramme, loading } = useProgrammes(user?.id);
  
  const [activeTab, setActiveTab] = useState<'my' | 'create'>('my');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [formData, setFormData] = useState<CreateProgrammeRequest>({
    name: '',
    description: '',
    durationWeeks: 6,
    startDate: new Date().toISOString().split('T')[0],
  });
  const [creating, setCreating] = useState(false);

  const handleCreateProgramme = async () => {
    try {
      setCreating(true);
      const programme = await createProgramme(formData);
      setShowCreateModal(false);
      navigate(`/programmes/${programme.id}`);
    } catch (err) {
      console.error('Failed to create programme:', err);
    } finally {
      setCreating(false);
    }
  };

  const handleDeleteProgramme = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this programme?')) {
      await deleteProgramme(id);
    }
  };

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <Flex justify="space-between" align="center" style={{ marginBottom: '2rem' }}>
            <div>
              <PageTitle>Programmes</PageTitle>
              <PageSubtitle>Create and manage your workout programmes</PageSubtitle>
            </div>
            <Button onClick={() => setShowCreateModal(true)}>
              + New Programme
            </Button>
          </Flex>

          <TabContainer>
            <Tab $active={activeTab === 'my'} onClick={() => setActiveTab('my')}>
              My Programmes
            </Tab>
            <Tab $active={activeTab === 'create'} onClick={() => setActiveTab('create')}>
              Templates
            </Tab>
          </TabContainer>

          {activeTab === 'my' && (
            <>
              {programmes.length === 0 ? (
                <Card>
                  <div style={{ textAlign: 'center', padding: '4rem' }}>
                    <div style={{ fontSize: '4rem', marginBottom: '1rem' }}>📋</div>
                    <h3 style={{ marginBottom: '0.5rem' }}>No Programmes Yet</h3>
                    <p style={{ color: '#A0AEC0', marginBottom: '1.5rem' }}>
                      Create your first programme to start tracking your workouts
                    </p>
                    <Button onClick={() => setShowCreateModal(true)}>
                      Create Programme
                    </Button>
                  </div>
                </Card>
              ) : (
                <Grid columns={3} gap="1.5rem">
                  {programmes.map((programme) => (
                    <ProgrammeCard 
                      key={programme.id} 
                      $isActive={programme.isActive}
                      $interactive
                      onClick={() => navigate(`/programmes/${programme.id}`)}
                    >
                      <CardHeader>
                        <div>
                          <CardTitle>{programme.name}</CardTitle>
                          {programme.isActive && (
                            <Badge $variant="primary" style={{ marginTop: '0.5rem' }}>
                              Active
                            </Badge>
                          )}
                        </div>
                      </CardHeader>
                      <CardContent>
                        <p style={{ 
                          color: '#A0AEC0', 
                          fontSize: '0.875rem',
                          marginBottom: '1rem',
                          minHeight: '2.5rem'
                        }}>
                          {programme.description || 'No description'}
                        </p>
                        <Stack gap="0.5rem">
                          <Flex justify="space-between" style={{ fontSize: '0.875rem' }}>
                            <span style={{ color: '#64748B' }}>Duration</span>
                            <span>{programme.durationWeeks} weeks</span>
                          </Flex>
                          <Flex justify="space-between" style={{ fontSize: '0.875rem' }}>
                            <span style={{ color: '#64748B' }}>Progress</span>
                            <span>{programme.completedWeeks} / {programme.durationWeeks}</span>
                          </Flex>
                          <ProgressBar 
                            value={programme.progressPercentage} 
                            variant={programme.isActive ? 'primary' : 'default' as any}
                          />
                        </Stack>
                      </CardContent>
                      <CardFooter>
                        <Button 
                          variant="ghost" 
                          size="sm"
                          onClick={(e) => {
                            e.stopPropagation();
                            handleDeleteProgramme(programme.id);
                          }}
                        >
                          Delete
                        </Button>
                        <Button 
                          variant="primary" 
                          size="sm"
                          onClick={(e) => {
                            e.stopPropagation();
                            navigate(`/programmes/${programme.id}`);
                          }}
                        >
                          View
                        </Button>
                      </CardFooter>
                    </ProgrammeCard>
                  ))}
                </Grid>
              )}
            </>
          )}

          {activeTab === 'create' && (
            <Card>
              <div style={{ textAlign: 'center', padding: '4rem' }}>
                <div style={{ fontSize: '4rem', marginBottom: '1rem' }}>🏗️</div>
                <h3 style={{ marginBottom: '0.5rem' }}>Coming Soon</h3>
                <p style={{ color: '#A0AEC0' }}>
                  Pre-made programme templates will be available soon!
                </p>
              </div>
            </Card>
          )}
        </Container>

        {/* Create Programme Modal */}
        {showCreateModal && (
          <Modal onClick={() => setShowCreateModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <ModalTitle>Create New Programme</ModalTitle>
              <Stack gap="1.25rem">
                <Input
                  label="Programme Name"
                  placeholder="e.g., Strength Building Phase"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
                <Input
                  label="Description"
                  placeholder="What are the goals of this programme?"
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                />
                <Select
                  label="Duration"
                  options={durationOptions}
                  value={formData.durationWeeks.toString()}
                  onChange={(e) => setFormData({ ...formData, durationWeeks: parseInt(e.target.value) })}
                />
                <Input
                  label="Start Date"
                  type="date"
                  value={formData.startDate}
                  onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                />
                <Flex justify="flex-end" gap="1rem" style={{ marginTop: '1rem' }}>
                  <Button variant="ghost" onClick={() => setShowCreateModal(false)}>
                    Cancel
                  </Button>
                  <Button 
                    onClick={handleCreateProgramme} 
                    disabled={!formData.name || creating}
                  >
                    {creating ? 'Creating...' : 'Create Programme'}
                  </Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}
      </PageWrapper>
    </>
  );
};

