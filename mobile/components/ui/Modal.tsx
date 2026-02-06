/**
 * Modal Component
 * Full-screen modal wrapper with header and scrollable content
 */

import {
  Modal as RNModal,
  View,
  Text,
  StyleSheet,
  Pressable,
  ScrollView,
  useColorScheme,
  KeyboardAvoidingView,
  Platform,
} from "react-native";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import { colors, spacing, typography, borderRadius } from "../../theme";

interface ModalProps {
  visible: boolean;
  onClose: () => void;
  title: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
  maxHeight?: boolean;
}

export function Modal({
  visible,
  onClose,
  title,
  children,
  footer,
  maxHeight = false,
}: ModalProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  return (
    <RNModal
      visible={visible}
      animationType="none"
      presentationStyle="fullScreen"
      onRequestClose={onClose}
    >
      <SafeAreaProvider>
        <SafeAreaView style={styles.safeArea} edges={["top", "left", "right"]}>
          <KeyboardAvoidingView
            behavior={Platform.OS === "ios" ? "padding" : "height"}
            style={styles.container}
          >
            {/* Header */}
            <View style={styles.header}>
              <Text style={styles.title}>{title}</Text>
              <Pressable
                style={({ pressed }) => [
                  styles.closeButton,
                  pressed && styles.closeButtonPressed,
                ]}
                onPress={onClose}
              >
                <Text style={styles.closeButtonText}>✕</Text>
              </Pressable>
            </View>

            {/* Content */}
            <ScrollView
              style={styles.content}
              contentContainerStyle={[
                styles.contentContainer,
                maxHeight && styles.contentContainerMaxHeight,
              ]}
              showsVerticalScrollIndicator={true}
              scrollEnabled={true}
              nestedScrollEnabled={true}
            >
              {children}
            </ScrollView>

            {/* Footer */}
            {footer && <View style={styles.footer}>{footer}</View>}
          </KeyboardAvoidingView>
        </SafeAreaView>
      </SafeAreaProvider>
    </RNModal>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    safeArea: {
      flex: 1,
      backgroundColor: isDark ? colors.dark.background : colors.background,
    },
    container: {
      flex: 1,
    },
    header: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      paddingHorizontal: spacing.md,
      paddingTop: spacing.lg,
      paddingBottom: spacing.md,
      borderBottomWidth: 1,
      borderBottomColor: isDark ? colors.dark.border : colors.border,
    },
    title: {
      ...typography.h2,
      color: isDark ? colors.dark.text : colors.text,
      flex: 1,
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
    closeButtonPressed: {
      opacity: 0.7,
    },
    closeButtonText: {
      fontSize: 20,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    content: {
      flex: 1,
    },
    contentContainer: {
      padding: spacing.md,
    },
    contentContainerMaxHeight: {
      flexGrow: 1,
    },
    footer: {
      padding: spacing.md,
      borderTopWidth: 1,
      borderTopColor: isDark ? colors.dark.border : colors.border,
    },
  });
