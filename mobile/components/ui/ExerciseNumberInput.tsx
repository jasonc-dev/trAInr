/**
 * ExerciseNumberInput Component
 * Number input with increment/decrement buttons and label for exercise configuration
 */

import {
  View,
  Text,
  TextInput,
  Pressable,
  StyleSheet,
  useColorScheme,
} from "react-native";
import { colors, spacing, typography, borderRadius } from "../../theme";

interface ExerciseNumberInputProps {
  label: string;
  value: number | null;
  onChange: (value: number | null) => void;
  min?: number;
  max?: number;
  suffix?: string;
  disabled?: boolean;
}

export function ExerciseNumberInput({
  label,
  value,
  onChange,
  min = 0,
  max = 100,
  suffix,
  disabled = false,
}: ExerciseNumberInputProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  const handleTextChange = (text: string) => {
    if (disabled) return;
    if (text === "") {
      onChange(null);
      return;
    }
    const numValue = parseFloat(text);
    if (!isNaN(numValue)) {
      const clampedValue = Math.max(min, Math.min(numValue, max));
      onChange(clampedValue);
    }
  };

  return (
    <View style={[styles.wrapper, disabled && styles.wrapperDisabled]}>
      <Text style={styles.label}>{label}</Text>
      <View style={styles.container}>
        <View style={styles.inputWrapper}>
          <TextInput
            style={[styles.input, disabled && styles.inputDisabled]}
            value={value !== null ? value.toString() : ""}
            onChangeText={handleTextChange}
            keyboardType="numeric"
            placeholder="0"
            placeholderTextColor={
              isDark ? colors.dark.textTertiary : colors.textTertiary
            }
            editable={!disabled}
          />
          {suffix && <Text style={styles.suffix}>{suffix}</Text>}
        </View>
      </View>
    </View>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    wrapper: {
      flex: 1,
      gap: spacing.xs,
    },
    wrapperDisabled: {
      opacity: 0.6,
    },
    label: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      fontWeight: "600",
    },
    container: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.xs,
    },
    button: {
      width: 36,
      height: 36,
      borderRadius: borderRadius.sm,
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      alignItems: "center",
      justifyContent: "center",
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
    },
    buttonDisabled: {
      opacity: 0.5,
    },
    buttonInactive: {
      opacity: 0.3,
    },
    buttonText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      fontSize: 20,
      fontWeight: "600",
    },
    buttonTextDisabled: {
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    buttonTextInactive: {
      color: isDark ? colors.dark.textTertiary : colors.textTertiary,
    },
    inputWrapper: {
      flex: 1,
      flexDirection: "row",
      alignItems: "center",
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.sm,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
      height: 36,
      paddingHorizontal: spacing.sm,
    },
    input: {
      flex: 1,
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      textAlign: "center",
      padding: 0,
    },
    inputDisabled: {
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    suffix: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginLeft: spacing.xs,
    },
  });
