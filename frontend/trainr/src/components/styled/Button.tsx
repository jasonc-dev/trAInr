import styled, { css } from "styled-components";

type ButtonVariant = "primary" | "secondary" | "accent" | "ghost" | "danger";
type ButtonSize = "sm" | "md" | "lg";

interface StyledButtonProps {
  $variant?: ButtonVariant;
  $size?: ButtonSize;
  $fullWidth?: boolean;
}

const sizeStyles = {
  sm: css`
    padding: 0.5rem 1rem;
    font-size: ${({ theme }) => theme.fontSizes.sm};
    border-radius: ${({ theme }) => theme.radii.md};
  `,
  md: css`
    padding: 0.75rem 1.5rem;
    font-size: ${({ theme }) => theme.fontSizes.md};
    border-radius: ${({ theme }) => theme.radii.lg};
  `,
  lg: css`
    padding: 1rem 2rem;
    font-size: ${({ theme }) => theme.fontSizes.lg};
    border-radius: ${({ theme }) => theme.radii.lg};
  `,
};

const variantStyles = {
  primary: css`
    background: linear-gradient(
      135deg,
      ${({ theme }) => theme.colors.primary} 0%,
      ${({ theme }) => theme.colors.primaryDark} 100%
    );
    color: ${({ theme }) => theme.colors.background};
    box-shadow: ${({ theme }) => theme.shadows.glow};

    &:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 0 30px rgba(0, 207, 193, 0.4);
    }
  `,
  secondary: css`
    background: ${({ theme }) => theme.colors.secondary};
    color: white;

    &:hover:not(:disabled) {
      background: ${({ theme }) => theme.colors.secondaryDark};
    }
  `,
  accent: css`
    background: linear-gradient(
      135deg,
      ${({ theme }) => theme.colors.accent} 0%,
      ${({ theme }) => theme.colors.accentDark} 100%
    );
    color: white;
    box-shadow: ${({ theme }) => theme.shadows.glowAccent};

    &:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 0 30px rgba(255, 107, 74, 0.4);
    }
  `,
  ghost: css`
    background: transparent;
    color: ${({ theme }) => theme.colors.text};
    border: 1px solid ${({ theme }) => theme.colors.border};

    &:hover:not(:disabled) {
      background: ${({ theme }) => theme.colors.surface};
      border-color: ${({ theme }) => theme.colors.borderLight};
    }
  `,
  danger: css`
    background: ${({ theme }) => theme.colors.error};
    color: white;

    &:hover:not(:disabled) {
      background: #e53e4e;
    }
  `,
};

export const StyledButton = styled.button<StyledButtonProps>`
  display: inline-flex;
  align-items: center;
  justify-content: flex-start;
  gap: 0.5rem;
  font-weight: ${({ theme }) => theme.fontWeights.semibold};
  transition: all ${({ theme }) => theme.transitions.normal};
  white-space: nowrap;
  width: ${({ $fullWidth }) => ($fullWidth ? "100%" : "auto")};

  ${({ $size = "md" }) => sizeStyles[$size]}
  ${({ $variant = "primary" }) => variantStyles[$variant]}

  &:active:not(:disabled) {
    transform: scale(0.98);
  }
`;

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
  size?: ButtonSize;
  fullWidth?: boolean;
  children: React.ReactNode;
}

export const Button: React.FC<ButtonProps> = ({
  variant = "primary",
  size = "md",
  fullWidth = false,
  children,
  ...props
}) => {
  return (
    <StyledButton
      $variant={variant}
      $size={size}
      $fullWidth={fullWidth}
      {...props}
    >
      {children}
    </StyledButton>
  );
};
