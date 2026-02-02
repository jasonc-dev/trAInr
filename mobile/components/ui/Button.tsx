/**
 * Button Component
 * Reusable button with variants
 */

import {
  Pressable,
  Text,
  StyleSheet,
  ActivityIndicator,
  useColorScheme,
  ViewStyle,
  TextStyle,
} from 'react-native';
import * as Haptics from 'expo-haptics';
import { colors, spacing, typography, borderRadius } from '../../theme';

type ButtonVariant = 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
type ButtonSize = 'sm' | 'md' | 'lg';

interface ButtonProps {
  title: string;
  onPress: () => void;
  variant?: ButtonVariant;
  size?: ButtonSize;
  disabled?: boolean;
  loading?: boolean;
  fullWidth?: boolean;
  haptic?: boolean;
}

export function Button({
  title,
  onPress,
  variant = 'primary',
  size = 'md',
  disabled = false,
  loading = false,
  fullWidth = false,
  haptic = true,
}: ButtonProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';

  const handlePress = () => {
    if (haptic) {
      Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
    }
    onPress();
  };

  const buttonStyles = getButtonStyles(variant, size, isDark, disabled, fullWidth);
  const textStyles = getTextStyles(variant, size, isDark, disabled);

  return (
    <Pressable
      style={({ pressed }) => [
        buttonStyles,
        pressed && !disabled && styles.pressed,
      ]}
      onPress={handlePress}
      disabled={disabled || loading}
    >
      {loading ? (
        <ActivityIndicator
          color={variant === 'primary' || variant === 'danger' ? '#FFFFFF' : colors.primary}
          size="small"
        />
      ) : (
        <Text style={textStyles}>{title}</Text>
      )}
    </Pressable>
  );
}

const styles = StyleSheet.create({
  pressed: {
    opacity: 0.8,
    transform: [{ scale: 0.98 }],
  },
});

function getButtonStyles(
  variant: ButtonVariant,
  size: ButtonSize,
  isDark: boolean,
  disabled: boolean,
  fullWidth: boolean
): ViewStyle {
  const base: ViewStyle = {
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: borderRadius.md,
    opacity: disabled ? 0.5 : 1,
    width: fullWidth ? '100%' : undefined,
  };

  // Size styles
  const sizeStyles: Record<ButtonSize, ViewStyle> = {
    sm: { paddingVertical: spacing.xs, paddingHorizontal: spacing.md },
    md: { paddingVertical: spacing.sm + 2, paddingHorizontal: spacing.lg },
    lg: { paddingVertical: spacing.md, paddingHorizontal: spacing.xl },
  };

  // Variant styles
  const variantStyles: Record<ButtonVariant, ViewStyle> = {
    primary: { backgroundColor: colors.primary },
    secondary: { backgroundColor: isDark ? colors.dark.surface : colors.surfaceSecondary },
    outline: {
      backgroundColor: 'transparent',
      borderWidth: 1,
      borderColor: colors.primary,
    },
    ghost: { backgroundColor: 'transparent' },
    danger: { backgroundColor: colors.error },
  };

  return { ...base, ...sizeStyles[size], ...variantStyles[variant] };
}

function getTextStyles(
  variant: ButtonVariant,
  size: ButtonSize,
  isDark: boolean,
  disabled: boolean
): TextStyle {
  const base: TextStyle = {
    ...typography.button,
    opacity: disabled ? 0.7 : 1,
  };

  // Size styles
  const sizeStyles: Record<ButtonSize, TextStyle> = {
    sm: { fontSize: 14 },
    md: { fontSize: 16 },
    lg: { fontSize: 18 },
  };

  // Variant styles
  const variantStyles: Record<ButtonVariant, TextStyle> = {
    primary: { color: '#FFFFFF' },
    secondary: { color: isDark ? colors.dark.text : colors.text },
    outline: { color: colors.primary },
    ghost: { color: colors.primary },
    danger: { color: '#FFFFFF' },
  };

  return { ...base, ...sizeStyles[size], ...variantStyles[variant] };
}
