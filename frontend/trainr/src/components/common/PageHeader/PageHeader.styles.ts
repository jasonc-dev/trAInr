/**
 * PageHeader Styled Components
 */

import styled from 'styled-components';

export const PageHeaderWrapper = styled.div`
  margin-bottom: ${({ theme }) => theme.spacing['2xl']};
`;

export const Title = styled.h1`
  font-size: ${({ theme }) => theme.fontSizes['3xl']};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

export const Subtitle = styled.p`
  color: ${({ theme }) => theme.colors.textSecondary};
  font-size: ${({ theme }) => theme.fontSizes.md};
`;
