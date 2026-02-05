/**
 * Badge Component
 * Small colored badge for status indicators
 */

import {
  View,
  Text,
  StyleSheet,
  useColorScheme,
  ViewStyle,
  TextStyle,
} from "react-native";
import { colors, spacing, typography, borderRadius } from "../../theme";

type BadgeVariant = "primary" | "success" | "warning" | "info" | "default";

interface BadgeProps {
  children: React.ReactNode;
  variant?: BadgeVariant;
  style?: ViewStyle;
}

export function Badge({ children, variant = "default", style }: BadgeProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";

  const badgeStyle = getBadgeStyle(variant, isDark);
  const textStyle = getTextStyle(variant, isDark);

  return (
    <View style={[badgeStyle, style]}>
      <Text style={textStyle}>{children}</Text>
    </View>
  );
}

function getBadgeStyle(variant: BadgeVariant, isDark: boolean): ViewStyle {
  const base: ViewStyle = {
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    borderRadius: borderRadius.sm,
    alignSelf: "flex-start",
  };

  const variantStyles: Record<BadgeVariant, ViewStyle> = {
    primary: {
      backgroundColor: colors.primary,
    },
    success: {
      backgroundColor: isDark
        ? "rgba(16, 185, 129, 0.2)"
        : "rgba(16, 185, 129, 0.1)",
      borderWidth: 1,
      borderColor: colors.success,
    },
    warning: {
      backgroundColor: isDark
        ? "rgba(245, 158, 11, 0.2)"
        : "rgba(245, 158, 11, 0.1)",
      borderWidth: 1,
      borderColor: colors.warning,
    },
    info: {
      backgroundColor: isDark
        ? "rgba(99, 102, 241, 0.2)"
        : "rgba(99, 102, 241, 0.1)",
      borderWidth: 1,
      borderColor: colors.primary,
    },
    default: {
      backgroundColor: isDark
        ? colors.dark.surfaceSecondary
        : colors.surfaceSecondary,
      borderWidth: 1,
      borderColor: isDark ? colors.dark.border : colors.border,
    },
  };

  return { ...base, ...variantStyles[variant] };
}

function getTextStyle(variant: BadgeVariant, isDark: boolean): TextStyle {
  const base: TextStyle = {
    ...typography.caption,
    fontWeight: "600",
  };

  const variantStyles: Record<BadgeVariant, TextStyle> = {
    primary: {
      color: "#FFFFFF",
    },
    success: {
      color: colors.success,
    },
    warning: {
      color: colors.warning,
    },
    info: {
      color: colors.primary,
    },
    default: {
      color: isDark ? colors.dark.text : colors.text,
    },
  };

  return { ...base, ...variantStyles[variant] };
}
