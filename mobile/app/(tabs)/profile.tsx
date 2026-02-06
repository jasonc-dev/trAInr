import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  useColorScheme,
  Pressable,
} from "react-native";
import { router } from "expo-router";
import { colors, spacing, typography, borderRadius } from "../../theme";
import { useAuthStore } from "../../stores/authStore";

export default function ProfileScreen() {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const { user, logout } = useAuthStore();

  const styles = createStyles(isDark);
  const initials =
    user?.firstName && user?.lastName
      ? `${user.firstName[0]}${user.lastName[0]}`
      : "NA";

  const handleLogout = async () => {
    await logout();
    router.replace("/(auth)/login");
  };

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.content}
      contentInsetAdjustmentBehavior="automatic"
    >
      <View style={styles.header}>
        <View style={styles.avatar}>
          <Text style={styles.avatarText}>{initials.toUpperCase()}</Text>
        </View>
        <Text style={styles.name}>
          {user ? `${user.firstName} ${user.lastName}` : "Guest"}
        </Text>
        <Text style={styles.email}>{user?.email ?? "Not signed in"}</Text>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Settings</Text>

        <Pressable style={styles.menuItem}>
          <Text style={styles.menuItemText}>Edit Profile</Text>
        </Pressable>

        <Pressable style={styles.menuItem}>
          <Text style={styles.menuItemText}>Training Preferences</Text>
        </Pressable>

        <Pressable style={styles.menuItem}>
          <Text style={styles.menuItemText}>Equipment</Text>
        </Pressable>

        <Pressable style={styles.menuItem}>
          <Text style={styles.menuItemText}>Units (kg/lbs)</Text>
        </Pressable>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Sync</Text>

        <View style={styles.syncStatus}>
          <Text style={styles.syncStatusText}>Last synced: Just now</Text>
          <View style={styles.syncIndicator} />
        </View>
      </View>

      <Pressable style={styles.logoutButton} onPress={handleLogout}>
        <Text style={styles.logoutButtonText}>Sign Out</Text>
      </Pressable>
    </ScrollView>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      flex: 1,
    },
    content: {
      padding: spacing.md,
    },
    header: {
      alignItems: "center",
      marginBottom: spacing.xl,
      paddingTop: spacing.lg,
    },
    avatar: {
      width: 80,
      height: 80,
      borderRadius: 40,
      backgroundColor: colors.primary,
      alignItems: "center",
      justifyContent: "center",
      marginBottom: spacing.md,
    },
    avatarText: {
      ...typography.h2,
      color: "#FFFFFF",
    },
    name: {
      ...typography.h2,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    email: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    section: {
      marginBottom: spacing.lg,
    },
    sectionTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.sm,
    },
    menuItem: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.md,
      padding: spacing.md,
      marginBottom: spacing.xs,
    },
    menuItemText: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
    },
    syncStatus: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.md,
      padding: spacing.md,
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
    },
    syncStatusText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    syncIndicator: {
      width: 8,
      height: 8,
      borderRadius: 4,
      backgroundColor: colors.success,
    },
    logoutButton: {
      backgroundColor: colors.error,
      borderRadius: borderRadius.md,
      padding: spacing.md,
      alignItems: "center",
      marginTop: spacing.lg,
    },
    logoutButtonText: {
      ...typography.button,
      color: "#FFFFFF",
    },
  });
