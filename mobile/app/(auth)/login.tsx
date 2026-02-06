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
import { SafeAreaView } from "react-native-safe-area-context";
import { Link, router } from "expo-router";
import { colors, spacing, typography, borderRadius } from "../../theme";
import { useAuthStore } from "../../stores/authStore";

export default function LoginScreen() {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const { login, isLoading, error, clearError } = useAuthStore();

  const handleUsernameChange = (text: string) => {
    if (error) clearError();
    setUsername(text);
  };

  const handlePasswordChange = (text: string) => {
    if (error) clearError();
    setPassword(text);
  };

  const handleLogin = async () => {
    try {
      await login({ username, password, deviceInfo: Platform.OS });
      router.replace("/(tabs)");
    } catch {
      // Error state handled by store
    }
  };

  const isFormInvalid = !username.trim() || !password.trim();

  return (
    <SafeAreaView style={styles.safeArea} edges={["top", "left", "right"]}>
      <KeyboardAvoidingView
        style={styles.container}
        behavior={Platform.OS === "ios" ? "padding" : "height"}
      >
        <ScrollView contentContainerStyle={styles.content}>
          <View style={styles.header}>
            <Text style={styles.title}>trAInr</Text>
            <Text style={styles.subtitle}>Welcome back</Text>
          </View>

          <View style={styles.form}>
            {error && <Text style={styles.errorText}>{error}</Text>}
            <View style={styles.inputContainer}>
              <Text style={styles.label}>Username</Text>
              <TextInput
                style={styles.input}
                placeholder="Enter your username"
                placeholderTextColor={
                  isDark ? colors.dark.textTertiary : colors.textTertiary
                }
                value={username}
                onChangeText={handleUsernameChange}
                autoCapitalize="none"
                autoCorrect={false}
              />
            </View>

            <View style={styles.inputContainer}>
              <Text style={styles.label}>Password</Text>
              <TextInput
                style={styles.input}
                placeholder="Enter your password"
                placeholderTextColor={
                  isDark ? colors.dark.textTertiary : colors.textTertiary
                }
                value={password}
                onChangeText={handlePasswordChange}
                secureTextEntry
              />
            </View>

            <Pressable
              style={[
                styles.button,
                (isLoading || isFormInvalid) && styles.buttonDisabled,
              ]}
              onPress={handleLogin}
              disabled={isLoading || isFormInvalid}
            >
              <Text style={styles.buttonText}>
                {isLoading ? "Signing in..." : "Sign In"}
              </Text>
            </Pressable>

            <View style={styles.footer}>
              <Text style={styles.footerText}>Don't have an account? </Text>
              <Link href="/(auth)/register" asChild>
                <Pressable>
                  <Text style={styles.linkText}>Sign Up</Text>
                </Pressable>
              </Link>
            </View>
          </View>
        </ScrollView>
      </KeyboardAvoidingView>
    </SafeAreaView>
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
    content: {
      flexGrow: 1,
      justifyContent: "center",
      padding: spacing.lg,
    },
    header: {
      alignItems: "center",
      marginBottom: spacing.xl,
    },
    title: {
      fontSize: 40,
      fontWeight: "700",
      color: colors.primary,
      marginBottom: spacing.sm,
    },
    subtitle: {
      ...typography.h2,
      color: isDark ? colors.dark.text : colors.text,
    },
    form: {
      gap: spacing.md,
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
