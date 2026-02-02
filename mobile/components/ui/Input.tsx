/**
 * Input Component
 * Reusable text input with label and error state
 */

import {
  View,
  Text,
  TextInput,
  StyleSheet,
  useColorScheme,
  TextInputProps,
} from 'react-native';
import { colors, spacing, typography, borderRadius } from '../../theme';

interface InputProps extends TextInputProps {
  label?: string;
  error?: string;
  helper?: string;
}

export function Input({
  label,
  error,
  helper,
  style,
  ...props
}: InputProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark, !!error);

  return (
    <View style={styles.container}>
      {label && <Text style={styles.label}>{label}</Text>}
      <TextInput
        style={[styles.input, style]}
        placeholderTextColor={isDark ? colors.dark.textTertiary : colors.textTertiary}
        {...props}
      />
      {error && <Text style={styles.error}>{error}</Text>}
      {helper && !error && <Text style={styles.helper}>{helper}</Text>}
    </View>
  );
}

const createStyles = (isDark: boolean, hasError: boolean) =>
  StyleSheet.create({
    container: {
      gap: spacing.xs,
    },
    label: {
      ...typography.bodySmall,
      fontWeight: '500',
      color: isDark ? colors.dark.text : colors.text,
    },
    input: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.md,
      padding: spacing.md,
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      borderWidth: 1,
      borderColor: hasError
        ? colors.error
        : isDark
          ? colors.dark.border
          : colors.border,
    },
    error: {
      ...typography.caption,
      color: colors.error,
    },
    helper: {
      ...typography.caption,
      color: isDark ? colors.dark.textTertiary : colors.textTertiary,
    },
  });
