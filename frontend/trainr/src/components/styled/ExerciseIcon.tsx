import styled from "styled-components";
import { ExerciseType } from "../../types";

interface ExerciseIconProps {
  $type: ExerciseType;
}

export const ExerciseIcon = styled.div<ExerciseIconProps>`
  font-size: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: ${({ theme }) => theme.spacing.sm};

  ${({ $type, theme }) => {
    switch ($type) {
      case ExerciseType.WeightTraining:
        return `color: ${theme.colors.primary};`;
      case ExerciseType.Bodyweight:
        return `color: ${theme.colors.success};`;
      case ExerciseType.Cardio:
        return `color: ${theme.colors.warning};`;
      case ExerciseType.Flexibility:
        return `color: ${theme.colors.info};`;
      default:
        return `color: ${theme.colors.text};`;
    }
  }}
`;
