import styled, { css } from 'styled-components';

interface CardProps {
  $interactive?: boolean;
  $glow?: boolean;
  $padding?: string;
}

export const Card = styled.div<CardProps>`
  background: ${({ theme }) => theme.colors.surface};
  border-radius: ${({ theme }) => theme.radii.xl};
  border: 1px solid ${({ theme }) => theme.colors.border};
  padding: ${({ $padding, theme }) => $padding || theme.spacing.lg};
  transition: all ${({ theme }) => theme.transitions.normal};
  
  ${({ $interactive }) =>
    $interactive &&
    css`
      cursor: pointer;
      
      &:hover {
        border-color: ${({ theme }) => theme.colors.borderLight};
        transform: translateY(-2px);
        box-shadow: ${({ theme }) => theme.shadows.lg};
      }
    `}
  
  ${({ $glow }) =>
    $glow &&
    css`
      border-color: ${({ theme }) => theme.colors.primary};
      box-shadow: ${({ theme }) => theme.shadows.glow};
    `}
`;

export const CardHeader = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: ${({ theme }) => theme.spacing.md};
`;

export const CardTitle = styled.h3`
  font-size: ${({ theme }) => theme.fontSizes.xl};
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  color: ${({ theme }) => theme.colors.text};
`;

export const CardSubtitle = styled.p`
  font-size: ${({ theme }) => theme.fontSizes.sm};
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-top: ${({ theme }) => theme.spacing.xs};
`;

export const CardContent = styled.div`
  color: ${({ theme }) => theme.colors.textSecondary};
`;

export const CardFooter = styled.div`
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: ${({ theme }) => theme.spacing.sm};
  margin-top: ${({ theme }) => theme.spacing.lg};
  padding-top: ${({ theme }) => theme.spacing.md};
  border-top: 1px solid ${({ theme }) => theme.colors.border};
`;

export const StatCard = styled(Card)`
  text-align: center;
  
  .stat-value {
    font-family: ${({ theme }) => theme.fonts.heading};
    font-size: ${({ theme }) => theme.fontSizes['4xl']};
    font-weight: ${({ theme }) => theme.fontWeights.bold};
    color: ${({ theme }) => theme.colors.primary};
    line-height: 1;
    margin-bottom: ${({ theme }) => theme.spacing.xs};
  }
  
  .stat-label {
    font-size: ${({ theme }) => theme.fontSizes.sm};
    color: ${({ theme }) => theme.colors.textSecondary};
    text-transform: uppercase;
    letter-spacing: 0.05em;
  }
  
  .stat-change {
    font-size: ${({ theme }) => theme.fontSizes.sm};
    margin-top: ${({ theme }) => theme.spacing.sm};
    
    &.positive { color: ${({ theme }) => theme.colors.success}; }
    &.negative { color: ${({ theme }) => theme.colors.error}; }
  }
`;

