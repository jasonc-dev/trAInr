/**
 * NumberInput Component
 * Number input with increment/decrement buttons
 */

import {
  View,
  Text,
  TextInput,
  StyleSheet,
  Pressable,
  useColorScheme,
} from "react-native";
import * as Haptics from "expo-haptics";
import { colors, spacing, typography, borderRadius } from "../../theme";

interface NumberInputProps {
  label: string;
  value: number | null;
  onChange: (value: number) => void;
  min?: number;
  max?: number;
  step?: number;
  suffix?: string;
  disabled?: boolean;
}

export function NumberInput({
  label,
  value,
  onChange,
  min = 0,
  max = 999,
  step = 1,
  suffix,
  disabled = false,
}: NumberInputProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  const handleIncrement = () => {
    if (disabled) return;
    const currentValue = value ?? min;
    const newValue = Math.min(currentValue + step, max);
    if (newValue !== currentValue) {
      Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
      onChange(newValue);
    }
  };

  const handleDecrement = () => {
    if (disabled) return;
    const currentValue = value ?? min;
    const newValue = Math.max(currentValue - step, min);
    if (newValue !== currentValue) {
      Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
      onChange(newValue);
    }
  };

  const handleTextChange = (text: string) => {
    if (disabled) return;
    const numValue = parseInt(text, 10);
    if (!isNaN(numValue)) {
      onChange(Math.max(min, Math.min(numValue, max)));
    } else if (text === "") {
      onChange(min);
    }
  };

  const displayValue = value !== null ? value.toString() : min.toString();

  return (
    <View style={styles.container}>
      <Text style={styles.label}>{label}</Text>
      <View style={styles.inputContainer}>
        <Pressable
          style={({ pressed }) => [
            styles.button,
            pressed && !disabled && styles.buttonPressed,
            disabled && styles.buttonDisabled,
          ]}
          onPress={handleDecrement}
          disabled={disabled || (value ?? min) <= min}
        >
          <Text style={styles.buttonText}>−</Text>
        </Pressable>

        <View style={styles.inputWrapper}>
          <TextInput
            style={styles.input}
            value={displayValue}
            onChangeText={handleTextChange}
            keyboardType="number-pad"
            selectTextOnFocus
            editable={!disabled}
          />
          {suffix && <Text style={styles.suffix}>{suffix}</Text>}
        </View>

        <Pressable
          style={({ pressed }) => [
            styles.button,
            pressed && !disabled && styles.buttonPressed,
            disabled && styles.buttonDisabled,
          ]}
          onPress={handleIncrement}
          disabled={disabled || (value ?? min) >= max}
        >
          <Text style={styles.buttonText}>+</Text>
        </Pressable>
      </View>
    </View>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      flex: 1,
      minWidth: 100,
    },
    label: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.xs,
    },
    inputContainer: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.xs,
    },
    button: {
      width: 36,
      height: 36,
      borderRadius: borderRadius.sm,
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
      alignItems: "center",
      justifyContent: "center",
    },
    buttonPressed: {
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
    },
    buttonDisabled: {
      opacity: 0.5,
    },
    buttonText: {
      fontSize: 20,
      fontWeight: "600",
      color: isDark ? colors.dark.text : colors.text,
    },
    inputWrapper: {
      flex: 1,
      flexDirection: "row",
      alignItems: "center",
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
      borderRadius: borderRadius.sm,
      paddingHorizontal: spacing.sm,
      height: 36,
    },
    input: {
      flex: 1,
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      textAlign: "center",
      padding: 0,
    },
    suffix: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginLeft: spacing.xs,
    },
  });
