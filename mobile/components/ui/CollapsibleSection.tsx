/**
 * CollapsibleSection Component
 * Reusable collapsible section with header and expandable content
 */

import {
  View,
  Text,
  Pressable,
  StyleSheet,
  useColorScheme,
} from "react-native";
import { colors, spacing, typography, borderRadius } from "../../theme";
import { Badge } from "./Badge";

interface CollapsibleSectionProps {
  title: string;
  count?: number;
  isExpanded: boolean;
  onToggle: () => void;
  children: React.ReactNode;
  defaultOpen?: boolean;
  disableToggle?: boolean;
}

export function CollapsibleSection({
  title,
  count,
  isExpanded,
  onToggle,
  children,
  disableToggle = false,
}: CollapsibleSectionProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);

  const headerContent = (
    <>
      <View style={styles.headerContent}>
        <Text style={styles.title}>{title}</Text>
        {count !== undefined && count > 0 && (
          <Badge variant="default">{count}</Badge>
        )}
      </View>
      {!disableToggle && (
        <Text style={styles.expandIcon}>{isExpanded ? "−" : "+"}</Text>
      )}
    </>
  );

  return (
    <View style={styles.container}>
      {disableToggle ? (
        <View style={styles.header}>{headerContent}</View>
      ) : (
        <Pressable
          style={({ pressed }) => [
            styles.header,
            pressed && styles.headerPressed,
          ]}
          onPress={onToggle}
        >
          {headerContent}
        </Pressable>
      )}

      {isExpanded && <View style={styles.content}>{children}</View>}
    </View>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      marginBottom: spacing.md,
    },
    header: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: spacing.md,
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
    },
    headerPressed: {
      opacity: 0.8,
    },
    headerContent: {
      flexDirection: "row",
      alignItems: "center",
      gap: spacing.sm,
      flex: 1,
    },
    title: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
    },
    expandIcon: {
      fontSize: 24,
      fontWeight: "600",
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginLeft: spacing.sm,
    },
    content: {
      marginTop: spacing.sm,
    },
  });
