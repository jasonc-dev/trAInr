/**
 * Card Component
 * Container with consistent styling
 */

import {
  View,
  StyleSheet,
  useColorScheme,
  ViewStyle,
  Pressable,
} from 'react-native';
import { colors, spacing, borderRadius, shadows } from '../../theme';

interface CardProps {
  children: React.ReactNode;
  style?: ViewStyle;
  onPress?: () => void;
  padding?: 'none' | 'sm' | 'md' | 'lg';
}

export function Card({
  children,
  style,
  onPress,
  padding = 'md',
}: CardProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark, padding);

  if (onPress) {
    return (
      <Pressable
        style={({ pressed }) => [
          styles.card,
          pressed && styles.pressed,
          style,
        ]}
        onPress={onPress}
      >
        {children}
      </Pressable>
    );
  }

  return <View style={[styles.card, style]}>{children}</View>;
}

const createStyles = (isDark: boolean, padding: 'none' | 'sm' | 'md' | 'lg') => {
  const paddingValues = {
    none: 0,
    sm: spacing.sm,
    md: spacing.md,
    lg: spacing.lg,
  };

  return StyleSheet.create({
    card: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: paddingValues[padding],
      ...shadows.sm,
    },
    pressed: {
      opacity: 0.9,
      transform: [{ scale: 0.99 }],
    },
  });
};
