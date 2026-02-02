import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  useColorScheme,
  Pressable,
  ActivityIndicator,
  RefreshControl,
} from 'react-native';
import { useQuery } from '@tanstack/react-query';
import { useAuthStore } from '../../stores/authStore';
import { programmesApi, ProgrammeSummary } from '../../lib/api/programmes';
import { colors, spacing, typography, borderRadius } from '../../theme';

function ProgrammeCard({
  programme,
  isActive,
  onPress,
  isDark,
}: {
  programme: ProgrammeSummary;
  isActive: boolean;
  onPress?: () => void;
  isDark: boolean;
}) {
  const styles = createStyles(isDark);
  const progress = Math.round(programme.progressPercentage);

  return (
    <Pressable
      style={({ pressed }) => [
        styles.card,
        isActive && styles.activeCard,
        pressed && styles.pressed,
      ]}
      onPress={onPress}
    >
      <View style={styles.cardHeader}>
        <Text style={styles.cardTitle} numberOfLines={1}>
          {programme.name}
        </Text>
        {isActive && (
          <View style={styles.activeBadge}>
            <Text style={styles.activeBadgeText}>Active</Text>
          </View>
        )}
      </View>
      {programme.description ? (
        <Text style={styles.cardSubtitle} numberOfLines={2}>
          {programme.description}
        </Text>
      ) : null}
      <View style={styles.cardMeta}>
        <Text style={styles.cardMetaText}>
          {programme.durationWeeks} weeks · {programme.completedWeeks} completed
        </Text>
        <View style={styles.progressBar}>
          <View
            style={[styles.progressFill, { width: `${progress}%` }]}
          />
        </View>
      </View>
    </Pressable>
  );
}

export default function ProgramsScreen() {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark);
  const { user } = useAuthStore();

  const {
    data: programmes = [],
    isLoading,
    isError,
    error,
    refetch,
    isRefetching,
  } = useQuery({
    queryKey: ['programmes', user?.id],
    queryFn: () => programmesApi.getAssignedProgrammes(user!.id),
    enabled: !!user?.id,
  });

  const activeProgramme = programmes.find((p) => p.isActive);

  if (!user?.id) {
    return (
      <View style={styles.container}>
        <View style={styles.placeholder}>
          <Text style={styles.placeholderText}>Sign in to view your programmes</Text>
        </View>
      </View>
    );
  }

  return (
    <ScrollView
      style={styles.container}
      contentContainerStyle={styles.content}
      refreshControl={
        <RefreshControl
          refreshing={isRefetching && !isLoading}
          onRefresh={refetch}
          tintColor={colors.primary}
        />
      }
    >
      <View style={styles.header}>
        <Text style={styles.title}>Programs</Text>
        <Text style={styles.subtitle}>Manage your training programs</Text>
      </View>

      {isLoading ? (
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={colors.primary} />
          <Text style={styles.loadingText}>Loading programmes...</Text>
        </View>
      ) : isError ? (
        <View style={styles.placeholder}>
          <Text style={styles.errorText}>
            {error instanceof Error ? error.message : 'Failed to load programmes'}
          </Text>
        </View>
      ) : (
        <>
          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Active Program</Text>
            {activeProgramme ? (
              <ProgrammeCard
                programme={activeProgramme}
                isActive
                isDark={isDark}
              />
            ) : (
              <View style={styles.placeholder}>
                <Text style={styles.placeholderText}>No active program</Text>
              </View>
            )}
          </View>

          {programmes.filter((p) => !p.isActive).length > 0 && (
            <View style={styles.section}>
              <Text style={styles.sectionTitle}>Your Programmes</Text>
              {programmes
                .filter((p) => !p.isActive)
                .map((programme) => (
                  <ProgrammeCard
                    key={programme.id}
                    programme={programme}
                    isActive={false}
                    isDark={isDark}
                  />
                ))}
            </View>
          )}

          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Get More Programmes</Text>
            <Pressable style={styles.card}>
              <Text style={styles.cardTitle}>Browse Pre-made Programs</Text>
              <Text style={styles.cardSubtitle}>
                Start with a professionally designed program
              </Text>
            </Pressable>
            <Pressable style={styles.card}>
              <Text style={styles.cardTitle}>Generate with AI</Text>
              <Text style={styles.cardSubtitle}>
                Create a personalized program with AI
              </Text>
            </Pressable>
          </View>
        </>
      )}
    </ScrollView>
  );
}

const createStyles = (isDark: boolean) =>
  StyleSheet.create({
    container: {
      flex: 1,
      backgroundColor: isDark ? colors.dark.background : colors.background,
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
    section: {
      marginBottom: spacing.lg,
    },
    sectionTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.sm,
    },
    loadingContainer: {
      padding: spacing.xl,
      alignItems: 'center',
      gap: spacing.md,
    },
    loadingText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    placeholder: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: spacing.lg,
      alignItems: 'center',
    },
    placeholderText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    errorText: {
      ...typography.body,
      color: colors.error,
      textAlign: 'center',
    },
    card: {
      backgroundColor: isDark ? colors.dark.surface : colors.surface,
      borderRadius: borderRadius.lg,
      padding: spacing.md,
      marginBottom: spacing.sm,
    },
    activeCard: {
      borderWidth: 2,
      borderColor: colors.primary,
    },
    pressed: {
      opacity: 0.9,
    },
    cardHeader: {
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'space-between',
      gap: spacing.sm,
      marginBottom: spacing.xs,
    },
    cardTitle: {
      ...typography.body,
      fontWeight: '600',
      color: isDark ? colors.dark.text : colors.text,
      flex: 1,
    },
    activeBadge: {
      backgroundColor: colors.primary,
      paddingHorizontal: spacing.sm,
      paddingVertical: spacing.xs,
      borderRadius: borderRadius.sm,
    },
    activeBadgeText: {
      ...typography.caption,
      color: '#FFFFFF',
      fontWeight: '600',
    },
    cardSubtitle: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.sm,
    },
    cardMeta: {
      gap: spacing.xs,
    },
    cardMetaText: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
    progressBar: {
      height: 4,
      backgroundColor: isDark ? colors.dark.surfaceSecondary : colors.surfaceSecondary,
      borderRadius: 2,
      overflow: 'hidden',
    },
    progressFill: {
      height: '100%',
      backgroundColor: colors.primary,
      borderRadius: 2,
    },
  });
