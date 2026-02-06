import { useState } from "react";
import {
  View,
  Text,
  StyleSheet,
  TextInput,
  Pressable,
  useColorScheme,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
} from "react-native";
import { Link, router } from "expo-router";
import { colors, spacing, typography, borderRadius } from "../../theme";
import { useAuthStore } from "../../stores/authStore";

type RegisterFormData = {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
};

export default function RegisterScreen() {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  const [formData, setFormData] = useState<RegisterFormData>({
    username: "",
    email: "",
    password: "",
    firstName: "",
    lastName: "",
  });
  const { register, isLoading, error, clearError } = useAuthStore();

  const handleChange = (key: keyof RegisterFormData, value: string) => {
    if (error) clearError();
    setFormData((previous: RegisterFormData) => ({
      ...previous,
      [key]: value,
    }));
  };

  const handleRegister = async () => {
    try {
      await register({
        username: formData.username.trim(),
        email: formData.email.trim(),
        password: formData.password,
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        dateOfBirth: "1990-01-01",
        fitnessLevel: 0,
        primaryGoal: 0,
        workoutDaysPerWeek: 3,
        deviceInfo: Platform.OS,
      });
      router.replace("/(tabs)");
    } catch {
      // Error state handled by store
    }
  };

  const isFormInvalid =
    !formData.firstName.trim() ||
    !formData.lastName.trim() ||
    !formData.username.trim() ||
    !formData.email.trim() ||
    !formData.password.trim();

  return (
    <KeyboardAvoidingView
      style={styles.container}
      behavior={Platform.OS === "ios" ? "padding" : "height"}
    >
      <ScrollView
        style={styles.scrollView}
        contentContainerStyle={styles.content}
        contentInsetAdjustmentBehavior="automatic"
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.header}>
          <Text style={styles.title}>Create Account</Text>
          <Text style={styles.subtitle}>Start your training journey</Text>
        </View>

        <View style={styles.form}>
          {error && <Text style={styles.errorText}>{error}</Text>}
          <View style={styles.row}>
            <View style={[styles.inputContainer, styles.halfWidth]}>
              <Text style={styles.label}>First Name</Text>
              <TextInput
                style={styles.input}
                placeholder="John"
                placeholderTextColor={
                  isDark ? colors.dark.textTertiary : colors.textTertiary
                }
                value={formData.firstName}
                onChangeText={(text: string) => handleChange("firstName", text)}
              />
            </View>
            <View style={[styles.inputContainer, styles.halfWidth]}>
              <Text style={styles.label}>Last Name</Text>
              <TextInput
                style={styles.input}
                placeholder="Doe"
                placeholderTextColor={
                  isDark ? colors.dark.textTertiary : colors.textTertiary
                }
                value={formData.lastName}
                onChangeText={(text: string) => handleChange("lastName", text)}
              />
            </View>
          </View>

          <View style={styles.inputContainer}>
            <Text style={styles.label}>Username</Text>
            <TextInput
              style={styles.input}
              placeholder="Choose a username"
              placeholderTextColor={
                isDark ? colors.dark.textTertiary : colors.textTertiary
              }
              value={formData.username}
              onChangeText={(text: string) => handleChange("username", text)}
              autoCapitalize="none"
              autoCorrect={false}
            />
          </View>

          <View style={styles.inputContainer}>
            <Text style={styles.label}>Email</Text>
            <TextInput
              style={styles.input}
              placeholder="your@email.com"
              placeholderTextColor={
                isDark ? colors.dark.textTertiary : colors.textTertiary
              }
              value={formData.email}
              onChangeText={(text: string) => handleChange("email", text)}
              keyboardType="email-address"
              autoCapitalize="none"
              autoCorrect={false}
            />
          </View>

          <View style={styles.inputContainer}>
            <Text style={styles.label}>Password</Text>
            <TextInput
              style={styles.input}
              placeholder="Create a password"
              placeholderTextColor={
                isDark ? colors.dark.textTertiary : colors.textTertiary
              }
              value={formData.password}
              onChangeText={(text: string) => handleChange("password", text)}
              secureTextEntry
            />
          </View>

          <Pressable
            style={[
              styles.button,
              (isLoading || isFormInvalid) && styles.buttonDisabled,
            ]}
            onPress={handleRegister}
            disabled={isLoading || isFormInvalid}
          >
            <Text style={styles.buttonText}>
              {isLoading ? "Creating account..." : "Create Account"}
            </Text>
          </Pressable>

          <View style={styles.footer}>
            <Text style={styles.footerText}>Already have an account? </Text>
            <Link href="/(auth)/login" asChild>
              <Pressable>
                <Text style={styles.linkText}>Sign In</Text>
              </Pressable>
            </Link>
          </View>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      flex: 1,
      backgroundColor: isDark ? colors.dark.background : colors.background,
    },
    scrollView: {
      flex: 1,
    },
    content: {
      flexGrow: 1,
      justifyContent: "center",
      padding: spacing.lg,
      paddingBottom: spacing.xl,
    },
    header: {
      alignItems: "center",
      marginBottom: spacing.xl,
    },
    title: {
      ...typography.h1,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    subtitle: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    form: {
      gap: spacing.md,
    },
    row: {
      flexDirection: "row",
      gap: spacing.sm,
    },
    halfWidth: {
      flex: 1,
    },
    inputContainer: {
      gap: spacing.xs,
    },
    label: {
      ...typography.bodySmall,
      fontWeight: "500",
      color: isDark ? colors.dark.text : colors.text,
    },
    input: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.md,
      padding: spacing.md,
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
    },
    button: {
      backgroundColor: colors.primary,
      borderRadius: borderRadius.md,
      padding: spacing.md,
      alignItems: "center",
      marginTop: spacing.md,
    },
    buttonDisabled: {
      opacity: 0.6,
    },
    buttonText: {
      ...typography.button,
      color: "#FFFFFF",
    },
    footer: {
      flexDirection: "row",
      justifyContent: "center",
      marginTop: spacing.lg,
    },
    footerText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    linkText: {
      ...typography.body,
      color: colors.primary,
      fontWeight: "600",
    },
    errorText: {
      ...typography.bodySmall,
      color: colors.error,
      textAlign: "center",
    },
  });
