/**
 * ErrorMessage Component
 * Displays error messages with consistent styling
 */

import React from "react";
import styled from "styled-components";

const StyledErrorMessage = styled.div`
  background: ${({ theme }) => theme.colors.errorLight};
  color: ${({ theme }) => theme.colors.error};
  padding: ${({ theme }) => theme.spacing.md};
  border-radius: ${({ theme }) => theme.radii.md};
  margin-bottom: ${({ theme }) => theme.spacing.lg};
  font-size: ${({ theme }) => theme.fontSizes.sm};
  text-align: center;
`;

interface ErrorMessageProps {
  message: string;
}

export const ErrorMessage: React.FC<ErrorMessageProps> = ({ message }) => {
  return <StyledErrorMessage>{message}</StyledErrorMessage>;
};
