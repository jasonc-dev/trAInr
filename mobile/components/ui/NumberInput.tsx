/**
 * NumberInput Component
 * Reusable number input with increment/decrement buttons
 */

import { View, TextInput, StyleSheet, useColorScheme } from "react-native";
import { colors, spacing, typography, borderRadius } from "../../theme";

export type NumberInputType = "reps" | "weight" | "rpe";

interface NumberInputProps {
  value: number | null;
  onChange: (value: number | null) => void;
  type: NumberInputType;
  disabled?: boolean;
}

export function NumberInput({
  value,
  onChange,
  type,
  disabled = false,
}: NumberInputProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  const getConfig = () => {
    switch (type) {
      case "reps":
        return { min: 1, max: 50, step: 1, placeholder: "0" };
      case "weight":
        return { min: 0, max: 600, step: 0.5, placeholder: "0" };
      case "rpe":
        return { min: 1, max: 10, step: 1, placeholder: "0" };
      default:
        return { min: 0, max: 100, step: 1, placeholder: "0" };
    }
  };

  const config = getConfig();

  const handleTextChange = (text: string) => {
    if (disabled) return;
    if (text === "") {
      onChange(null);
      return;
    }
    const numValue = parseFloat(text);
    if (!isNaN(numValue)) {
      const clampedValue = Math.max(config.min, Math.min(numValue, config.max));
      onChange(clampedValue);
    }
  };

  return (
    <View style={[styles.container, disabled && styles.containerDisabled]}>
      <TextInput
        style={[styles.input, disabled && styles.inputDisabled]}
        value={value !== null ? value.toString() : ""}
        onChangeText={handleTextChange}
        keyboardType="numeric"
        placeholder={config.placeholder}
        placeholderTextColor={
          isDark ? colors.dark.textTertiary : colors.textTertiary
        }
        editable={!disabled}
      />
    </View>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.xs,
    },
    containerDisabled: {
      opacity: 0.6,
    },
    button: {
      width: 44,
      height: 44,
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
    buttonText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      fontSize: 20,
      fontWeight: "600",
    },
    buttonTextDisabled: {
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    input: {
      flex: 1,
      height: 32,
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.sm,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
      paddingHorizontal: spacing.sm,
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      textAlign: "center",
    },
    inputDisabled: {
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
  });
