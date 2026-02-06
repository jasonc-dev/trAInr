/**
 * Tabs Component
 * Horizontal tab navigation with badges
 */

import {
  View,
  Text,
  StyleSheet,
  Pressable,
  ScrollView,
  useColorScheme,
} from "react-native";
import { colors, spacing, typography, borderRadius } from "../../theme";
import { Badge } from "./Badge";

export interface Tab {
  id: string;
  label: string;
  badge?: number;
}

interface TabsProps {
  tabs: Tab[];
  activeTabId: string;
  onTabChange: (tabId: string) => void;
}

export function Tabs({ tabs, activeTabId, onTabChange }: TabsProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  return (
    <View style={styles.container}>
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={styles.scrollContent}
      >
        {tabs.map((tab) => {
          const isActive = tab.id === activeTabId;
          return (
            <Pressable
              key={tab.id}
              style={({ pressed }) => [
                styles.tab,
                isActive && styles.tabActive,
                pressed && styles.tabPressed,
              ]}
              onPress={() => onTabChange(tab.id)}
            >
              <Text style={[styles.tabText, isActive && styles.tabTextActive]}>
                {tab.label}
              </Text>
              {tab.badge !== undefined && tab.badge > 0 && (
                <Badge variant="primary" style={styles.badge}>
                  {tab.badge}
                </Badge>
              )}
            </Pressable>
          );
        })}
      </ScrollView>
    </View>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      borderBottomWidth: 1,
      borderBottomColor: isDark ? colors.dark.border : colors.border,
    },
    scrollContent: {
      paddingHorizontal: spacing.md,
      paddingVertical: spacing.sm,
      gap: spacing.xs,
    },
    tab: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.xs,
      paddingHorizontal: spacing.lg,
      paddingVertical: spacing.sm,
      borderRadius: borderRadius.md,
      backgroundColor: "transparent",
    },
    tabActive: {
      backgroundColor: isDark
        ? "rgba(99, 102, 241, 0.2)"
        : "rgba(99, 102, 241, 0.1)",
    },
    tabPressed: {
      opacity: 0.7,
    },
    tabText: {
      ...typography.body,
      fontWeight: "500",
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    tabTextActive: {
      color: colors.primary,
      fontWeight: "600",
    },
    badge: {
      marginLeft: 0,
    },
  });
