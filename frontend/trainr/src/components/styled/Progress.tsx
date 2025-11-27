import styled, { keyframes } from 'styled-components';

const shimmer = keyframes`
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
`;

interface ProgressBarProps {
  $value: number;
  $variant?: 'primary' | 'success' | 'warning' | 'error';
}

const variantColors = {
  primary: '#00CFC1',
  success: '#00D68F',
  warning: '#FFB547',
  error: '#FF4757',
};

export const ProgressBarContainer = styled.div`
  width: 100%;
  height: 8px;
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border-radius: ${({ theme }) => theme.radii.full};
  overflow: hidden;
`;

export const ProgressBarFill = styled.div<ProgressBarProps>`
  height: 100%;
  width: ${({ $value }) => Math.min(100, Math.max(0, $value))}%;
  background: linear-gradient(
    90deg,
    ${({ $variant = 'primary' }) => variantColors[$variant]} 0%,
    ${({ $variant = 'primary' }) => variantColors[$variant]}dd 100%
  );
  border-radius: ${({ theme }) => theme.radii.full};
  transition: width ${({ theme }) => theme.transitions.normal};
  position: relative;
  
  &::after {
    content: '';
    position: absolute;
    inset: 0;
    background: linear-gradient(
      90deg,
      transparent,
      rgba(255, 255, 255, 0.2),
      transparent
    );
    background-size: 200% 100%;
    animation: ${shimmer} 2s infinite;
  }
`;

interface ProgressBarComponentProps {
  value: number;
  variant?: 'primary' | 'success' | 'warning' | 'error';
  showLabel?: boolean;
}

export const ProgressBar: React.FC<ProgressBarComponentProps> = ({
  value,
  variant = 'primary',
  showLabel = false,
}) => {
  return (
    <div style={{ width: '100%' }}>
      <ProgressBarContainer>
        <ProgressBarFill $value={value} $variant={variant} />
      </ProgressBarContainer>
      {showLabel && (
        <ProgressLabel>{Math.round(value)}%</ProgressLabel>
      )}
    </div>
  );
};

const ProgressLabel = styled.span`
  display: block;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  color: ${({ theme }) => theme.colors.textSecondary};
  text-align: right;
  margin-top: ${({ theme }) => theme.spacing.xs};
`;

export const CircularProgress = styled.div<{ $size?: number; $value: number }>`
  width: ${({ $size }) => $size || 120}px;
  height: ${({ $size }) => $size || 120}px;
  border-radius: 50%;
  background: conic-gradient(
    ${({ theme }) => theme.colors.primary} ${({ $value }) => $value * 3.6}deg,
    ${({ theme }) => theme.colors.backgroundSecondary} 0deg
  );
  display: flex;
  align-items: center;
  justify-content: center;
  
  &::before {
    content: '';
    width: ${({ $size }) => ($size || 120) - 16}px;
    height: ${({ $size }) => ($size || 120) - 16}px;
    background: ${({ theme }) => theme.colors.surface};
    border-radius: 50%;
  }
`;

export const CircularProgressValue = styled.div`
  position: absolute;
  font-family: ${({ theme }) => theme.fonts.heading};
  font-size: ${({ theme }) => theme.fontSizes['2xl']};
  font-weight: ${({ theme }) => theme.fontWeights.bold};
  color: ${({ theme }) => theme.colors.text};
`;

