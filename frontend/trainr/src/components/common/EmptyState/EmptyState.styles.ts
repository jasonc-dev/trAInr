/**
 * EmptyState Styled Components
 */

import styled from "styled-components";

export const EmptyStateWrapper = styled.div`
  text-align: center;
  padding: ${({ theme }) => theme.spacing["3xl"]};
`;

export const Icon = styled.div`
  font-size: 4rem;
  margin-bottom: ${({ theme }) => theme.spacing.lg};
`;

export const Title = styled.h3`
  font-size: ${({ theme }) => theme.fontSizes.xl};
  margin-bottom: ${({ theme }) => theme.spacing.sm};
`;

export const Description = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing.xl};
`;
