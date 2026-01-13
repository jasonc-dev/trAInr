import React, { useState, useEffect } from "react";
import styled from "styled-components";
import {
  Container,
  PageWrapper,
  Grid,
  Card,
  CardTitle,
  CardContent,
  Button,
  Input,
  Badge,
  Chip,
  Flex,
} from "../components/styled";
import { Navigation } from "../components/styled/Navigation";
import { useExercises } from "../hooks";
import { Exercise, ExerciseType, MuscleGroup } from "../types";
import { EXERCISE_TYPES, getExerciseIcon } from "../utils";
import { ExerciseIcon } from "../components/styled";

const PageTitle = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes["3xl"]};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

const PageSubtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing["2xl"]};
`;

const FilterSection = styled.div`
  margin-bottom: ${({ theme }) => theme.spacing.xl};
`;

const FilterLabel = styled.span`
  display: block;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing.sm};
`;

const ChipGroup = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: ${({ theme }) => theme.spacing.sm};
`;

const ExerciseCard = styled(Card)`
  cursor: pointer;

  &:hover {
    border-color: ${({ theme }) => theme.colors.primary};
    transform: translateY(-2px);
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
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
`;

const getExerciseTypeLabel = (type: ExerciseType) => {
  return ExerciseType[type] || "Unknown";
};

const getMuscleGroupLabel = (group: MuscleGroup) => {
  return MuscleGroup[group] || "Unknown";
};

const muscleGroups = [
  { value: null, label: "All Muscles" },
  { value: MuscleGroup.Chest, label: "Chest" },
  { value: MuscleGroup.Back, label: "Back" },
  { value: MuscleGroup.Shoulders, label: "Shoulders" },
  { value: MuscleGroup.Biceps, label: "Biceps" },
  { value: MuscleGroup.Triceps, label: "Triceps" },
  { value: MuscleGroup.Core, label: "Core" },
  { value: MuscleGroup.Quadriceps, label: "Quadriceps" },
  { value: MuscleGroup.Hamstrings, label: "Hamstrings" },
  { value: MuscleGroup.Glutes, label: "Glutes" },
  { value: MuscleGroup.Calves, label: "Calves" },
  { value: MuscleGroup.FullBody, label: "Full Body" },
];

export const Exercises: React.FC = () => {
  const { exercises, loading, searchExercises } = useExercises();
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedType, setSelectedType] = useState<ExerciseType | null>(null);
  const [selectedMuscle, setSelectedMuscle] = useState<MuscleGroup | null>(
    null
  );
  const [filteredExercises, setFilteredExercises] = useState<Exercise[]>([]);
  const [selectedExercise, setSelectedExercise] = useState<Exercise | null>(
    null
  );

  useEffect(() => {
    const filterExercises = async () => {
      if (searchQuery || selectedType !== null || selectedMuscle !== null) {
        const results = await searchExercises(
          searchQuery || undefined,
          selectedType || undefined,
          selectedMuscle || undefined
        );
        // Get full exercise details for filtered results
        const filteredIds = new Set(results.map((r) => r.id));
        setFilteredExercises(exercises.filter((e) => filteredIds.has(e.id)));
      } else {
        setFilteredExercises(exercises);
      }
    };

    const debounce = setTimeout(filterExercises, 300);
    return () => clearTimeout(debounce);
  }, [searchQuery, selectedType, selectedMuscle, exercises, searchExercises]);

  return (
    <>
      <Navigation />
      <PageWrapper>
        <Container>
          <PageTitle>Exercise Library</PageTitle>
          <PageSubtitle>
            Browse and learn about different exercises
          </PageSubtitle>

          <FilterSection>
            <Input
              placeholder="Search exercises..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              style={{ marginBottom: "1rem" }}
            />

            <FilterLabel>Exercise Type</FilterLabel>
            <ChipGroup style={{ marginBottom: "1rem" }}>
              {EXERCISE_TYPES.map((type) => (
                <Chip
                  key={type.label}
                  $active={selectedType === type.value}
                  onClick={() => setSelectedType(type.value)}
                >
                  {type.label}
                </Chip>
              ))}
            </ChipGroup>

            <FilterLabel>Muscle Group</FilterLabel>
            <ChipGroup>
              {muscleGroups.map((group) => (
                <Chip
                  key={group.label}
                  $active={selectedMuscle === group.value}
                  onClick={() => setSelectedMuscle(group.value)}
                >
                  {group.label}
                </Chip>
              ))}
            </ChipGroup>
          </FilterSection>

          {loading ? (
            <div style={{ textAlign: "center", padding: "4rem" }}>
              Loading exercises...
            </div>
          ) : filteredExercises.length === 0 ? (
            <Card>
              <div style={{ textAlign: "center", padding: "4rem" }}>
                <div style={{ fontSize: "4rem", marginBottom: "1rem" }}>🔍</div>
                <h3 style={{ marginBottom: "0.5rem" }}>No Exercises Found</h3>
                <p style={{ color: "#A0AEC0" }}>
                  Try adjusting your search or filters
                </p>
              </div>
            </Card>
          ) : (
            <Grid $columns={3} $gap="1.5rem">
              {filteredExercises.map((exercise) => (
                <ExerciseCard
                  key={exercise.id}
                  $interactive
                  onClick={() => setSelectedExercise(exercise)}
                >
                  <ExerciseIcon $type={exercise.type}>
                    {getExerciseIcon(exercise.type)}
                  </ExerciseIcon>
                  <CardTitle
                    style={{ fontSize: "1rem", marginBottom: "0.5rem" }}
                  >
                    {exercise.name}
                  </CardTitle>
                  <Flex $gap="0.5rem" $wrap>
                    <Badge $variant="primary">
                      {getExerciseTypeLabel(exercise.type)}
                    </Badge>
                    <Badge>
                      {getMuscleGroupLabel(exercise.primaryMuscleGroup)}
                    </Badge>
                  </Flex>
                  <CardContent style={{ marginTop: "0.75rem" }}>
                    <p
                      style={{
                        fontSize: "0.875rem",
                        color: "#A0AEC0",
                        overflow: "hidden",
                        textOverflow: "ellipsis",
                        display: "-webkit-box",
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: "vertical",
                      }}
                    >
                      {exercise.description}
                    </p>
                  </CardContent>
                </ExerciseCard>
              ))}
            </Grid>
          )}
        </Container>

        {/* Exercise Detail Modal */}
        {selectedExercise && (
          <Modal onClick={() => setSelectedExercise(null)}>
            <ModalContent onClick={(e) => e.stopPropagation()}>
              <Flex
                $gap="1rem"
                $align="flex-start"
                style={{ marginBottom: "1.5rem" }}
              >
                <ExerciseIcon
                  $type={selectedExercise.type}
                  style={{ marginBottom: 0 }}
                >
                  {getExerciseIcon(selectedExercise.type)}
                </ExerciseIcon>
                <div>
                  <h2 style={{ marginBottom: "0.5rem" }}>
                    {selectedExercise.name}
                  </h2>
                  <Flex $gap="0.5rem" $wrap>
                    <Badge $variant="primary">
                      {getExerciseTypeLabel(selectedExercise.type)}
                    </Badge>
                    <Badge>
                      {getMuscleGroupLabel(selectedExercise.primaryMuscleGroup)}
                    </Badge>
                    {selectedExercise.secondaryMuscleGroup && (
                      <Badge $variant="info">
                        {getMuscleGroupLabel(
                          selectedExercise.secondaryMuscleGroup
                        )}
                      </Badge>
                    )}
                  </Flex>
                </div>
              </Flex>

              <div style={{ marginBottom: "1.5rem" }}>
                <h4 style={{ marginBottom: "0.5rem", color: "#A0AEC0" }}>
                  Description
                </h4>
                <p>{selectedExercise.description}</p>
              </div>

              {selectedExercise.instructions && (
                <div style={{ marginBottom: "1.5rem" }}>
                  <h4 style={{ marginBottom: "0.5rem", color: "#A0AEC0" }}>
                    Instructions
                  </h4>
                  <p style={{ whiteSpace: "pre-wrap" }}>
                    {selectedExercise.instructions}
                  </p>
                </div>
              )}

              <Flex $justify="flex-end">
                <Button
                  variant="ghost"
                  onClick={() => setSelectedExercise(null)}
                >
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
