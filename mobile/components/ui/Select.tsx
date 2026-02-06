/**
 * Select Component
 * Dropdown/Picker component for mobile
 */

import {
  View,
  Text,
  StyleSheet,
  Pressable,
  useColorScheme,
  Modal as RNModal,
  ScrollView,
} from "react-native";
import { useState } from "react";
import { colors, spacing, typography, borderRadius } from "../../theme";

export interface SelectOption {
  label: string;
  value: string;
}

interface SelectProps {
  label?: string;
  value: string;
  options: SelectOption[];
  onChange: (value: string) => void;
  placeholder?: string;
  disabled?: boolean;
}

export function Select({
  label,
  value,
  options,
  onChange,
  placeholder = "Select...",
  disabled = false,
}: SelectProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);
  const [showModal, setShowModal] = useState(false);

  const selectedOption = options.find((opt) => opt.value === value);
  const displayValue = selectedOption?.label || placeholder;

  const handleSelect = (optionValue: string) => {
    onChange(optionValue);
    setShowModal(false);
  };

  return (
    <View style={styles.container}>
      {label && <Text style={styles.label}>{label}</Text>}
      <Pressable
        style={({ pressed }) => [
          styles.selectButton,
          pressed && !disabled && styles.selectButtonPressed,
          disabled && styles.selectButtonDisabled,
        ]}
        onPress={() => !disabled && setShowModal(true)}
        disabled={disabled}
      >
        <Text
          style={[styles.selectText, !selectedOption && styles.placeholderText]}
        >
          {displayValue}
        </Text>
        <Text style={styles.chevron}>▼</Text>
      </Pressable>

      <RNModal
        visible={showModal}
        transparent
        animationType="fade"
        onRequestClose={() => setShowModal(false)}
      >
        <Pressable
          style={styles.modalOverlay}
          onPress={() => setShowModal(false)}
        >
          <View style={styles.modalSafeArea}>
            <View
              style={styles.modalContent}
              onStartShouldSetResponder={() => true}
            >
              <View style={styles.modalHeader}>
                <Text style={styles.modalTitle}>{label}</Text>
                <Pressable
                  style={styles.closeButton}
                  onPress={() => setShowModal(false)}
                >
                  <Text style={styles.closeButtonText}>✕</Text>
                </Pressable>
              </View>
              <ScrollView
                style={styles.optionsList}
                contentContainerStyle={styles.optionsListContent}
                bounces={false}
              >
                {options.map((option) => (
                  <Pressable
                    key={option.value}
                    style={({ pressed }) => [
                      styles.option,
                      option.value === value && styles.optionSelected,
                      pressed && styles.optionPressed,
                    ]}
                    onPress={() => handleSelect(option.value)}
                  >
                    <Text
                      style={[
                        styles.optionText,
                        option.value === value && styles.optionTextSelected,
                      ]}
                    >
                      {option.label}
                    </Text>
                    {option.value === value && (
                      <Text style={styles.checkmark}>✓</Text>
                    )}
                  </Pressable>
                ))}
              </ScrollView>
            </View>
          </View>
        </Pressable>
      </RNModal>
    </View>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      flex: 1,
    },
    label: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.xs,
    },
    selectButton: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
      borderRadius: borderRadius.sm,
      paddingHorizontal: spacing.md,
      height: 32,
    },
    selectButtonPressed: {
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
    },
    selectButtonDisabled: {
      opacity: 0.5,
    },
    selectText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      flex: 1,
    },
    placeholderText: {
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    chevron: {
      fontSize: 12,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginLeft: spacing.sm,
    },
    modalOverlay: {
      flex: 1,
      backgroundColor: "rgba(0, 0, 0, 0.5)",
      justifyContent: "flex-end",
    },
    modalSafeArea: {
      width: "100%",
    },
    modalContent: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderTopLeftRadius: borderRadius.lg,
      borderTopRightRadius: borderRadius.lg,
      maxHeight: "70%",
      minHeight: 440,
      paddingBottom: spacing.lg,
    },
    modalHeader: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      padding: spacing.md,
      borderBottomWidth: 1,
      borderBottomColor: isDark ? colors.dark.border : colors.border,
    },
    modalTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
    },
    closeButton: {
      width: 32,
      height: 32,
      borderRadius: borderRadius.sm,
      alignItems: "center",
      justifyContent: "center",
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
    },
    closeButtonText: {
      fontSize: 20,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    optionsList: {
      flex: 1,
    },
    optionsListContent: {
      paddingBottom: spacing.lg,
    },
    option: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      padding: spacing.md,
      borderBottomWidth: 1,
      borderBottomColor: isDark ? colors.dark.border : colors.border,
    },
    optionSelected: {
      backgroundColor: isDark
        ? "rgba(99, 102, 241, 0.1)"
        : "rgba(99, 102, 241, 0.05)",
    },
    optionPressed: {
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
    },
    optionText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
    },
    optionTextSelected: {
      color: colors.primary,
      fontWeight: "600",
    },
    checkmark: {
      fontSize: 18,
      color: colors.primary,
      fontWeight: "bold",
    },
  });
