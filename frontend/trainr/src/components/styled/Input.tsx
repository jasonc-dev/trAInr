import styled, { css } from 'styled-components';

interface InputWrapperProps {
  $hasError?: boolean;
}

export const InputWrapper = styled.div<InputWrapperProps>`
  position: relative;
  width: 100%;
`;

export const Label = styled.label`
  display: block;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
`;

const inputStyles = css<InputWrapperProps>`
  width: 100%;
  padding: 0.875rem 1rem;
  font-size: ${({ theme }) => theme.fontSizes.md};
  color: ${({ theme }) => theme.colors.text};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border: 1px solid ${({ theme, $hasError }) =>
    $hasError ? theme.colors.error : theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.lg};
  transition: all ${({ theme }) => theme.transitions.fast};
  
  &::placeholder {
    color: ${({ theme }) => theme.colors.textMuted};
  }
  
  &:focus {
    outline: none;
    border-color: ${({ theme, $hasError }) =>
    $hasError ? theme.colors.error : theme.colors.primary};
    box-shadow: 0 0 0 3px ${({ theme, $hasError }) =>
    $hasError ? theme.colors.errorLight : theme.colors.primaryGhost};
  }
  
  &:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }
`;

export const StyledInput = styled.input<InputWrapperProps>`
  ${inputStyles}
`;

export const StyledTextarea = styled.textarea<InputWrapperProps>`
  ${inputStyles}
  min-height: 120px;
  resize: vertical;
`;

export const StyledSelect = styled.select<InputWrapperProps & { $size: 'md' | 'lg' }>`
  ${inputStyles}
  height: ${({ $size }) => $size === 'md' ? '2rem' : '3rem'};
  padding: ${({ $size }) => $size === 'md' ? '0 0.5rem' : '0 1rem'};
  cursor: pointer;
  display: flex;
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='%23A0AEC0' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  background-size: 1rem;
  padding-right: 2.5rem;
`;

const FieldError = styled.span`
  display: block;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  color: ${({ theme }) => theme.colors.error};
  margin-top: ${({ theme }) => theme.spacing.xs};
`;

export const HelperText = styled.span`
  display: block;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  color: ${({ theme }) => theme.colors.textMuted};
  margin-top: ${({ theme }) => theme.spacing.xs};
`;

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
}

export const Input: React.FC<InputProps> = ({
  label,
  error,
  helperText,
  id,
  ...props
}) => {
  return (
    <InputWrapper $hasError={!!error}>
      {label && <Label htmlFor={id}>{label}</Label>}
      <StyledInput id={id} $hasError={!!error} {...props} />
      {error && <FieldError>{error}</FieldError>}
      {helperText && !error && <HelperText>{helperText}</HelperText>}
    </InputWrapper>
  );
};

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  error?: string;
  options: { value: string | number; label: string }[];
  $size?: 'md' | 'lg';
}

export const Select: React.FC<SelectProps> = ({
  id,
  label,
  error,
  options,
  $size = 'md',
  ...props
}) => {
  return (
    <InputWrapper $hasError={!!error}>
      {label && <Label htmlFor={id}>{label}</Label>}
      <StyledSelect id={id} $hasError={!!error} $size={$size} {...props}>
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </StyledSelect>
      {error && <FieldError>{error}</FieldError>}
    </InputWrapper>
  );
};

