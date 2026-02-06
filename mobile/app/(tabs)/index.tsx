import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  useColorScheme,
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
import { colors, spacing, typography } from "../../theme";

export default function TodayScreen() {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";

  const styles = createStyles(isDark);

  return (
    <SafeAreaView style={styles.safeArea} edges={["left", "right"]}>
      <ScrollView
        style={styles.container}
        contentContainerStyle={styles.content}
      >
        <View style={styles.header}>
          <Text style={styles.title}>Today's Workout</Text>
          <Text style={styles.subtitle}>No workout scheduled for today</Text>
        </View>

        <View style={styles.placeholder}>
          <Text style={styles.placeholderText}>
            Start by selecting a program from the Programs tab
          </Text>
        </View>
      </ScrollView>
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
      padding: spacing.md,
    },
    header: {
      marginBottom: spacing.lg,
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
    placeholder: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: 16,
      padding: spacing.xl,
      alignItems: "center",
      justifyContent: "center",
      minHeight: 200,
    },
    placeholderText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      textAlign: "center",
    },
  });
