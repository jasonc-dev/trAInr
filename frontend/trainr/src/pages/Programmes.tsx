import React, { useState } from "react";
import styled from "styled-components";
import { useNavigate } from "react-router-dom";
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
} from "../components/styled";
import { Navigation } from "../components/styled/Navigation";
import { useUser, useProgrammes } from "../hooks";
import {
  CreateProgrammeRequest,
  ProgrammeSummary,
  UpdateProgrammeRequest,
} from "../types";

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes["3xl"]};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

const PageSubtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing["2xl"]};
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
    $active ? theme.colors.primaryGhost : "transparent"};
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
  ${({ $isActive, theme }) =>
    $isActive &&
    `
    border-color: ${theme.colors.primary};
    box-shadow: ${theme.shadows.glow};
  `}
`;

const TemplateCard = styled(Card)`
  position: relative;
  overflow: hidden;

  &::before {
    content: "";
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 4px;
    background: linear-gradient(
      90deg,
      ${({ theme }) => theme.colors.primary} 0%,
      ${({ theme }) => theme.colors.accent} 100%
    );
  }
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
  font-size: ${({ theme }) => theme.fontSizes["2xl"]};
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

// const CheckboxLabel = styled.label`
//   display: block;
//   font-size: ${({ theme }) => theme.fontSizes.sm};
//   font-weight: ${({ theme }) => theme.fontWeights.medium};
//   color: ${({ theme }) => theme.colors.textSecondary};
//   cursor: pointer;
// `;

const durationOptions = [
  { value: "4", label: "4 weeks" },
  { value: "5", label: "5 weeks" },
  { value: "6", label: "6 weeks" },
  { value: "7", label: "7 weeks" },
  { value: "8", label: "8 weeks" },
  { value: "9", label: "9 weeks" },
  { value: "10", label: "10 weeks" },
];

export const Programmes: React.FC = () => {
  const navigate = useNavigate();
  const { user } = useUser();
  const {
    programmes,
    preMadeProgrammes,
    createProgramme,
    updateProgramme,
    deleteProgramme,
    cloneProgramme,
  } = useProgrammes(user?.id);

  const [activeTab, setActiveTab] = useState<"my" | "templates">("my");
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showCloneModal, setShowCloneModal] = useState(false);
  const [selectedTemplate, setSelectedTemplate] =
    useState<ProgrammeSummary | null>(null);
  const [selectedProgramme, setSelectedProgramme] =
    useState<ProgrammeSummary | null>(null);
  const [formData, setFormData] = useState<CreateProgrammeRequest>({
    name: "",
    description: "",
    durationWeeks: 6,
    startDate: "",
  });
  const [editFormData, setEditFormData] = useState<UpdateProgrammeRequest>({
    name: "",
    description: "",
    isActive: false,
  });
  const [creating, setCreating] = useState(false);
  const [updating, setUpdating] = useState(false);
  const [cloning, setCloning] = useState(false);

  const handleCreateProgramme = async () => {
    try {
      setCreating(true);
      const programme = await createProgramme(formData);
      setShowCreateModal(false);
      setFormData({
        name: "",
        description: "",
        durationWeeks: 6,
        startDate: "",
      });
      // Navigate to programme detail with flag to open the exercise builder
      navigate(`/programmes/${programme.id}`, { state: { openBuilder: true } });
    } catch (err) {
      console.error("Failed to create programme:", err);
    } finally {
      setCreating(false);
    }
  };

  const handleDeleteProgramme = async (id: string) => {
    if (window.confirm("Are you sure you want to delete this programme?")) {
      await deleteProgramme(id);
    }
  };

  const handleEditProgramme = (programme: ProgrammeSummary) => {
    setSelectedProgramme(programme);
    setEditFormData({
      name: programme.name,
      description: programme.description || "",
      isActive: programme.isActive,
    });
    setShowEditModal(true);
  };

  const handleUpdateProgramme = async () => {
    if (!selectedProgramme) return;

    try {
      setUpdating(true);
      await updateProgramme(selectedProgramme.id, editFormData);
      setShowEditModal(false);
      setSelectedProgramme(null);
      setEditFormData({ name: "", description: "", isActive: false });
    } catch (err) {
      console.error("Failed to update programme:", err);
    } finally {
      setUpdating(false);
    }
  };

  const handleCloneTemplate = (template: ProgrammeSummary) => {
    setSelectedTemplate(template);
    setShowCloneModal(true);
  };

  const handleConfirmClone = async () => {
    if (!selectedTemplate) return;

    try {
      setCloning(true);
      const programme = await cloneProgramme(selectedTemplate.id);
      setShowCloneModal(false);
      setSelectedTemplate(null);
      navigate(`/programmes/${programme.id}`);
    } catch (err) {
      console.error("Failed to clone programme:", err);
    } finally {
      setCloning(false);
    }
  };

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <Flex
            $justify="space-between"
            $align="center"
            style={{ marginBottom: "2rem" }}
          >
            <div>
              <PageTitle>Programmes</PageTitle>
              <PageSubtitle>
                Create and manage your workout programmes
              </PageSubtitle>
            </div>
            <Button onClick={() => setShowCreateModal(true)}>
              + New Programme
            </Button>
          </Flex>

          <TabContainer>
            <Tab
              $active={activeTab === "my"}
              onClick={() => setActiveTab("my")}
            >
              My Programmes
            </Tab>
            <Tab
              $active={activeTab === "templates"}
              onClick={() => setActiveTab("templates")}
            >
              Templates
              {preMadeProgrammes.length > 0 && (
                <Badge $variant="primary" style={{ marginLeft: "0.5rem" }}>
                  {preMadeProgrammes.length}
                </Badge>
              )}
            </Tab>
          </TabContainer>

          {/* My Programmes Tab */}
          {activeTab === "my" && (
            <>
              {programmes.length === 0 ? (
                <Card>
                  <EmptyState>
                    <div className="icon">📋</div>
                    <h3>No Programmes Yet</h3>
                    <p>
                      Create your first programme to start tracking your
                      workouts
                    </p>
                    <Flex $gap="1rem" $justify="center">
                      <Button onClick={() => setShowCreateModal(true)}>
                        Create Programme
                      </Button>
                      {preMadeProgrammes.length > 0 && (
                        <Button
                          variant="secondary"
                          onClick={() => setActiveTab("templates")}
                        >
                          Browse Templates
                        </Button>
                      )}
                    </Flex>
                  </EmptyState>
                </Card>
              ) : (
                <Grid $columns={3} $gap="1.5rem">
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
                            <Badge
                              $variant="primary"
                              style={{ marginTop: "0.5rem" }}
                            >
                              Active
                            </Badge>
                          )}
                        </div>
                      </CardHeader>
                      <CardContent>
                        <p
                          style={{
                            color: "#A0AEC0",
                            fontSize: "0.875rem",
                            marginBottom: "1rem",
                            minHeight: "2.5rem",
                          }}
                        >
                          {programme.description || "No description"}
                        </p>
                        <Stack $gap="0.5rem">
                          <Flex
                            $justify="space-between"
                            style={{ fontSize: "0.875rem" }}
                          >
                            <span style={{ color: "#64748B" }}>Duration</span>
                            <span>{programme.durationWeeks} weeks</span>
                          </Flex>
                          <Flex
                            $justify="space-between"
                            style={{ fontSize: "0.875rem" }}
                          >
                            <span style={{ color: "#64748B" }}>Progress</span>
                            <span>
                              {programme.completedWeeks} /{" "}
                              {programme.durationWeeks}
                            </span>
                          </Flex>
                          <ProgressBar
                            value={programme.progressPercentage}
                            variant={
                              programme.isActive
                                ? "primary"
                                : ("default" as any)
                            }
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
                          variant="secondary"
                          size="sm"
                          onClick={(e) => {
                            e.stopPropagation();
                            handleEditProgramme(programme);
                          }}
                        >
                          Edit
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

          {/* Templates Tab */}
          {activeTab === "templates" && (
            <>
              {preMadeProgrammes.length === 0 ? (
                <Card>
                  <EmptyState>
                    <div className="icon">📚</div>
                    <h3>No Templates Available</h3>
                    <p>
                      Pre-made programme templates will appear here when
                      available
                    </p>
                  </EmptyState>
                </Card>
              ) : (
                <Grid $columns={3} $gap="1.5rem">
                  {preMadeProgrammes.map((template) => (
                    <TemplateCard key={template.id} $interactive>
                      <CardHeader>
                        <div>
                          <CardTitle>{template.name}</CardTitle>
                          <Badge
                            $variant="info"
                            style={{ marginTop: "0.5rem" }}
                          >
                            Template
                          </Badge>
                        </div>
                      </CardHeader>
                      <CardContent>
                        <p
                          style={{
                            color: "#A0AEC0",
                            fontSize: "0.875rem",
                            marginBottom: "1rem",
                            minHeight: "2.5rem",
                          }}
                        >
                          {template.description || "No description"}
                        </p>
                        <Stack $gap="0.5rem">
                          <Flex
                            $justify="space-between"
                            style={{ fontSize: "0.875rem" }}
                          >
                            <span style={{ color: "#64748B" }}>Duration</span>
                            <span>{template.durationWeeks} weeks</span>
                          </Flex>
                        </Stack>
                      </CardContent>
                      <CardFooter>
                        <Button
                          variant="primary"
                          fullWidth
                          onClick={() => handleCloneTemplate(template)}
                        >
                          Use This Template
                        </Button>
                      </CardFooter>
                    </TemplateCard>
                  ))}
                </Grid>
              )}
            </>
          )}
        </Container>

        {/* Create Programme Modal */}
        {showCreateModal && (
          <Modal onClick={() => setShowCreateModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <ModalTitle>Create New Programme</ModalTitle>
              <Stack $gap="1.25rem">
                <Input
                  label="Programme Name"
                  placeholder="e.g., Strength Building Phase"
                  value={formData.name}
                  onChange={(e) =>
                    setFormData({ ...formData, name: e.target.value })
                  }
                />
                <Input
                  label="Description"
                  placeholder="What are the goals of this programme?"
                  value={formData.description}
                  onChange={(e) =>
                    setFormData({ ...formData, description: e.target.value })
                  }
                />
                <Select
                  label="Duration"
                  options={durationOptions}
                  value={formData.durationWeeks.toString()}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      durationWeeks: parseInt(e.target.value),
                    })
                  }
                />
                <Input
                  label="Start Date"
                  type="date"
                  value={formData.startDate}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      startDate: e.target.value,
                    })
                  }
                />
                <Flex
                  $justify="flex-end"
                  $gap="1rem"
                  style={{ marginTop: "1rem" }}
                >
                  <Button
                    variant="ghost"
                    onClick={() => setShowCreateModal(false)}
                  >
                    Cancel
                  </Button>
                  <Button
                    onClick={handleCreateProgramme}
                    disabled={!formData.name || creating}
                  >
                    {creating ? "Creating..." : "Create Programme"}
                  </Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}

        {/* Edit Programme Modal */}
        {showEditModal && selectedProgramme && (
          <Modal onClick={() => setShowEditModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <ModalTitle>Edit Programme</ModalTitle>
              <Stack $gap="1.25rem">
                <Input
                  label="Programme Name"
                  placeholder="e.g., Strength Building Phase"
                  value={editFormData.name}
                  onChange={(e) =>
                    setEditFormData({ ...editFormData, name: e.target.value })
                  }
                />
                <Input
                  label="Description"
                  placeholder="What are the goals of this programme?"
                  value={editFormData.description}
                  onChange={(e) =>
                    setEditFormData({
                      ...editFormData,
                      description: e.target.value,
                    })
                  }
                />
                <Flex $gap="1rem" $align="center">
                  <label
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "0.5rem",
                      cursor: "pointer",
                    }}
                  >
                    <input
                      type="checkbox"
                      checked={editFormData.isActive}
                      onChange={(e) =>
                        setEditFormData({
                          ...editFormData,
                          isActive: e.target.checked,
                        })
                      }
                    />
                    Set as Active Programme
                  </label>
                </Flex>
                <Flex
                  $justify="flex-end"
                  $gap="1rem"
                  style={{ marginTop: "1rem" }}
                >
                  <Button
                    variant="ghost"
                    onClick={() => {
                      setShowEditModal(false);
                      setSelectedProgramme(null);
                    }}
                  >
                    Cancel
                  </Button>
                  <Button
                    onClick={handleUpdateProgramme}
                    disabled={!editFormData.name || updating}
                  >
                    {updating ? "Saving..." : "Save Changes"}
                  </Button>
                </Flex>
              </Stack>
            </ModalContent>
          </Modal>
        )}

        {/* Clone Template Modal */}
        {showCloneModal && selectedTemplate && (
          <Modal onClick={() => setShowCloneModal(false)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <ModalTitle>Start Programme</ModalTitle>
              <Stack $gap="1.25rem">
                <Card style={{ background: "rgba(0, 207, 193, 0.1)" }}>
                  <CardContent>
                    <h4 style={{ marginBottom: "0.5rem" }}>
                      {selectedTemplate.name}
                    </h4>
                    <p style={{ color: "#A0AEC0", fontSize: "0.875rem" }}>
                      {selectedTemplate.description}
                    </p>
                    <Flex
                      $justify="space-between"
                      style={{ marginTop: "1rem", fontSize: "0.875rem" }}
                    >
                      <span style={{ color: "#64748B" }}>Duration</span>
                      <span>{selectedTemplate.durationWeeks} weeks</span>
                    </Flex>
                  </CardContent>
                </Card>

                <p style={{ color: "#A0AEC0", fontSize: "0.875rem" }}>
                  This will create a copy of the template programme for you to
                  customize and track your progress.
                </p>

                <Flex
                  $justify="flex-end"
                  $gap="1rem"
                  style={{ marginTop: "1rem" }}
                >
                  <Button
                    variant="ghost"
                    onClick={() => {
                      setShowCloneModal(false);
                      setSelectedTemplate(null);
                    }}
                  >
                    Cancel
                  </Button>
                  <Button onClick={handleConfirmClone} disabled={cloning}>
                    {cloning ? "Starting..." : "Start Programme"}
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
