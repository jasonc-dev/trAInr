import styled from 'styled-components';

type BadgeVariant = 'default' | 'primary' | 'success' | 'warning' | 'error' | 'info';

interface StyledBadgeProps {
  $variant?: BadgeVariant;
}

const variantColors = {
  default: {
    bg: 'rgba(160, 174, 192, 0.15)',
    text: '#A0AEC0',
  },
  primary: {
    bg: 'rgba(0, 207, 193, 0.15)',
    text: '#00CFC1',
  },
  success: {
    bg: 'rgba(0, 214, 143, 0.15)',
    text: '#00D68F',
  },
  warning: {
    bg: 'rgba(255, 181, 71, 0.15)',
    text: '#FFB547',
  },
  error: {
    bg: 'rgba(255, 71, 87, 0.15)',
    text: '#FF4757',
  },
  info: {
    bg: 'rgba(0, 194, 255, 0.15)',
    text: '#00C2FF',
  },
};

export const Badge = styled.span<StyledBadgeProps>`
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.25rem 0.75rem;
  font-size: ${({ theme }) => theme.fontSizes.xs};
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  text-transform: uppercase;
  letter-spacing: 0.05em;
  border-radius: ${({ theme }) => theme.radii.full};
  background: ${({ $variant = 'default' }) => variantColors[$variant].bg};
  color: ${({ $variant = 'default' }) => variantColors[$variant].text};
`;

export const Tag = styled.span`
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.375rem 0.75rem;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  background: ${({ theme }) => theme.colors.surface};
  border: 1px solid ${({ theme }) => theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.md};
  color: ${({ theme }) => theme.colors.textSecondary};
  
  &:hover {
    background: ${({ theme }) => theme.colors.surfaceHover};
    border-color: ${({ theme }) => theme.colors.borderLight};
  }
`;

export const Chip = styled.button<{ $active?: boolean }>`
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  background: ${({ theme, $active }) => 
    $active ? theme.colors.primaryGhost : theme.colors.surface};
  border: 1px solid ${({ theme, $active }) => 
    $active ? theme.colors.primary : theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.full};
  color: ${({ theme, $active }) => 
    $active ? theme.colors.primary : theme.colors.textSecondary};
  cursor: pointer;
  transition: all ${({ theme }) => theme.transitions.fast};
  
  &:hover {
    border-color: ${({ theme }) => theme.colors.primary};
    color: ${({ theme }) => theme.colors.primary};
  }
`;

