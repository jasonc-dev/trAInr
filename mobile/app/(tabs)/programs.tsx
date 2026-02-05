import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  useColorScheme,
  Pressable,
  ActivityIndicator,
  RefreshControl,
  Alert,
  TextInput,
} from 'react-native';
import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuthStore } from '../../stores/authStore';
import { programmesApi } from '../../lib/api/programmes';
import type { ProgrammeSummary } from '../../lib/types/programme';
import { colors, spacing, typography, borderRadius } from '../../theme';
import { Tabs, Tab, Badge, Modal, Button, Select, SelectOption } from '../../components/ui';
import { Input } from '../../components/ui';

import ProgrammeDetailModal from '../../components/ProgrammeDetailModal';

function ProgrammeCard({
  programme,
  isActive,
  isTemplate,
  onPress,
  onEdit,
  onDelete,
  onClone,
  isDark,
}: {
  programme: ProgrammeSummary;
  isActive: boolean;
  isTemplate: boolean;
  onPress?: () => void;
  onEdit?: () => void;
  onDelete?: () => void;
  onClone?: () => void;
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
        <View style={styles.cardHeaderBadges}>
          {isActive && (
            <Badge variant="primary">Active</Badge>
          )}
          {isTemplate && (
            <Badge variant="info">Template</Badge>
          )}
        </View>
      </View>
      {programme.description ? (
        <Text style={styles.cardSubtitle} numberOfLines={2}>
          {programme.description}
        </Text>
      ) : null}
      <View style={styles.cardMeta}>
        <Text style={styles.cardMetaText}>
          {programme.durationWeeks} weeks
          {!isTemplate && ` · ${programme.completedWeeks} completed`}
        </Text>
        {!isTemplate && (
          <View style={styles.progressBar}>
            <View
              style={[styles.progressFill, { width: `${progress}%` }]}
            />
          </View>
        )}
      </View>
      <View style={styles.cardActions}>
        {isTemplate ? (
          <Button
            title="Use Template"
            onPress={() => onClone?.()}
            variant="primary"
            size="sm"
            fullWidth
          />
        ) : (
          <>
            <Button
              title="View"
              onPress={() => onPress?.()}
              variant="primary"
              size="sm"
            />
            <Button
              title="Edit"
              onPress={() => onEdit?.()}
              variant="secondary"
              size="sm"
            />
            <Button
              title="Delete"
              onPress={() => onDelete?.()}
              variant="ghost"
              size="sm"
            />
          </>
        )}
      </View>
    </Pressable>
  );
}

export default function ProgramsScreen() {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === 'dark';
  const styles = createStyles(isDark);
  const { user } = useAuthStore();
  const queryClient = useQueryClient();

  // Tab state
  const [activeTab, setActiveTab] = useState<'my' | 'created' | 'premade'>('my');

  // Modal states
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showCloneModal, setShowCloneModal] = useState(false);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [selectedProgramme, setSelectedProgramme] = useState<ProgrammeSummary | null>(null);
  const [selectedProgrammeId, setSelectedProgrammeId] = useState<string | null>(null);

  // Form states
  const [createForm, setCreateForm] = useState({
    name: '',
    description: '',
    durationWeeks: 6,
    startDate: new Date().toISOString().split('T')[0],
  });

  const [editForm, setEditForm] = useState({
    name: '',
    description: '',
    isActive: false,
  });

  const [cloneForm, setCloneForm] = useState({
    startDate: new Date().toISOString().split('T')[0],
  });

  // Queries
  const {
    data: myProgrammes = [],
    isLoading: loadingMy,
    refetch: refetchMy,
    isRefetching: isRefetchingMy,
  } = useQuery({
    queryKey: ['programmes', user?.id],
    queryFn: () => programmesApi.getAssignedProgrammes(user!.id),
    enabled: !!user?.id && activeTab === 'my',
  });

  const {
    data: createdProgrammes = [],
    isLoading: loadingCreated,
    refetch: refetchCreated,
    isRefetching: isRefetchingCreated,
  } = useQuery({
    queryKey: ['programmes', 'created', user?.id],
    queryFn: () => programmesApi.getCreatedProgrammes(user!.id),
    enabled: !!user?.id && activeTab === 'created',
  });

  const {
    data: premadeProgrammes = [],
    isLoading: loadingPremade,
    refetch: refetchPremade,
    isRefetching: isRefetchingPremade,
  } = useQuery({
    queryKey: ['programmes', 'premade'],
    queryFn: () => programmesApi.getPreMadeProgrammes(),
    enabled: activeTab === 'premade',
  });

  // Mutations
  const createMutation = useMutation({
    mutationFn: (data: typeof createForm) =>
      programmesApi.createProgramme(user!.id, data),
    onSuccess: (programme) => {
      queryClient.invalidateQueries({ queryKey: ['programmes'] });
      setShowCreateModal(false);
      setCreateForm({
        name: '',
        description: '',
        durationWeeks: 6,
        startDate: new Date().toISOString().split('T')[0],
      });
      // Open detail modal for new programme
      setSelectedProgrammeId(programme.id);
      setShowDetailModal(true);
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to create programme');
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: typeof editForm }) =>
      programmesApi.updateProgramme(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programmes'] });
      setShowEditModal(false);
      setSelectedProgramme(null);
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to update programme');
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => programmesApi.deleteProgramme(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['programmes'] });
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to delete programme');
    },
  });

  const cloneMutation = useMutation({
    mutationFn: ({ programmeId, data }: { programmeId: string; data: typeof cloneForm }) =>
      programmesApi.cloneProgramme(programmeId, {
        athleteId: user!.id,
        startDate: data.startDate,
      }),
    onSuccess: (programme) => {
      queryClient.invalidateQueries({ queryKey: ['programmes'] });
      setShowCloneModal(false);
      setSelectedProgramme(null);
      // Open detail modal for cloned programme
      setSelectedProgrammeId(programme.id);
      setShowDetailModal(true);
    },
    onError: (error: Error) => {
      Alert.alert('Error', error.message || 'Failed to clone programme');
    },
  });

  // Handlers
  const handleEdit = (programme: ProgrammeSummary) => {
    setSelectedProgramme(programme);
    setEditForm({
      name: programme.name,
      description: programme.description || '',
      isActive: programme.isActive,
    });
    setShowEditModal(true);
  };

  const handleDelete = (programme: ProgrammeSummary) => {
    Alert.alert(
      'Delete Programme',
      `Are you sure you want to delete "${programme.name}"? This action cannot be undone.`,
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Delete',
          style: 'destructive',
          onPress: () => deleteMutation.mutate(programme.id),
        },
      ]
    );
  };

  const handleClone = (programme: ProgrammeSummary) => {
    setSelectedProgramme(programme);
    setCloneForm({
      startDate: new Date().toISOString().split('T')[0],
    });
    setShowCloneModal(true);
  };

  const handleViewDetail = (programmeId: string) => {
    setSelectedProgrammeId(programmeId);
    setShowDetailModal(true);
  };

  // Tab configuration
  const tabs: Tab[] = [
    { id: 'my', label: 'My Programmes', badge: myProgrammes.length },
    { id: 'created', label: 'Your Templates', badge: createdProgrammes.length },
    { id: 'premade', label: 'Pre-made', badge: premadeProgrammes.length },
  ];

  // Current data based on active tab
  const currentData =
    activeTab === 'my'
      ? myProgrammes
      : activeTab === 'created'
        ? createdProgrammes
        : premadeProgrammes;

  const isLoading =
    activeTab === 'my'
      ? loadingMy
      : activeTab === 'created'
        ? loadingCreated
        : loadingPremade;

  const isRefetching =
    activeTab === 'my'
      ? isRefetchingMy
      : activeTab === 'created'
        ? isRefetchingCreated
        : isRefetchingPremade;

  const refetch =
    activeTab === 'my'
      ? refetchMy
      : activeTab === 'created'
        ? refetchCreated
        : refetchPremade;

  const activeProgramme = myProgrammes.find((p) => p.isActive);
  const isTemplate = activeTab !== 'my';

  // Duration options
  const durationOptions: SelectOption[] = [
    { label: '4 weeks', value: '4' },
    { label: '6 weeks', value: '6' },
    { label: '8 weeks', value: '8' },
    { label: '10 weeks', value: '10' },
    { label: '12 weeks', value: '12' },
  ];

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
    <>
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
          {activeTab === 'my' && (
            <Button
              title="+ New Programme"
              onPress={() => setShowCreateModal(true)}
              variant="primary"
              size="md"
            />
          )}
        </View>

        <Tabs tabs={tabs} activeTabId={activeTab} onTabChange={(id) => setActiveTab(id as any)} />

        {isLoading ? (
          <View style={styles.loadingContainer}>
            <ActivityIndicator size="large" color={colors.primary} />
            <Text style={styles.loadingText}>Loading programmes...</Text>
          </View>
        ) : currentData.length === 0 ? (
          <View style={styles.placeholder}>
            <Text style={styles.placeholderIcon}>
              {activeTab === 'my' ? '📋' : '📚'}
            </Text>
            <Text style={styles.placeholderTitle}>
              {activeTab === 'my'
                ? 'No Programmes Yet'
                : activeTab === 'created'
                  ? 'No Templates Created'
                  : 'No Pre-made Templates'}
            </Text>
            <Text style={styles.placeholderText}>
              {activeTab === 'my'
                ? 'Create your first programme to start tracking'
                : 'Templates will appear here when available'}
            </Text>
            {activeTab === 'my' && (
              <Button
                title="Create Programme"
                onPress={() => setShowCreateModal(true)}
                variant="primary"
              />
            )}
          </View>
        ) : (
          <View style={styles.grid}>
            {activeTab === 'my' && activeProgramme && (
              <View style={styles.section}>
                <Text style={styles.sectionTitle}>Active Program</Text>
                <ProgrammeCard
                  programme={activeProgramme}
                  isActive
                  isTemplate={false}
                  onPress={() => handleViewDetail(activeProgramme.id)}
                  onEdit={() => handleEdit(activeProgramme)}
                  onDelete={() => handleDelete(activeProgramme)}
                  isDark={isDark}
                />
              </View>
            )}

            {activeTab === 'my' && myProgrammes.filter((p) => !p.isActive).length > 0 && (
              <View style={styles.section}>
                <Text style={styles.sectionTitle}>Your Programmes</Text>
                {myProgrammes
                  .filter((p) => !p.isActive)
                  .map((programme) => (
                    <ProgrammeCard
                      key={programme.id}
                      programme={programme}
                      isActive={false}
                      isTemplate={false}
                      onPress={() => handleViewDetail(programme.id)}
                      onEdit={() => handleEdit(programme)}
                      onDelete={() => handleDelete(programme)}
                      isDark={isDark}
                    />
                  ))}
              </View>
            )}

            {(activeTab === 'created' || activeTab === 'premade') && (
              <View style={styles.section}>
                {currentData.map((programme) => (
                  <ProgrammeCard
                    key={programme.id}
                    programme={programme}
                    isActive={false}
                    isTemplate
                    onClone={() => handleClone(programme)}
                    isDark={isDark}
                  />
                ))}
              </View>
            )}
          </View>
        )}
      </ScrollView>

      {/* Create Programme Modal */}
      <Modal
        visible={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="Create New Programme"
        footer={
          <View style={styles.modalFooter}>
            <Button
              title="Cancel"
              onPress={() => setShowCreateModal(false)}
              variant="ghost"
            />
            <Button
              title={createMutation.isPending ? 'Creating...' : 'Create'}
              onPress={() => createMutation.mutate(createForm)}
              variant="primary"
              disabled={!createForm.name || createMutation.isPending}
              loading={createMutation.isPending}
            />
          </View>
        }
      >
        <View style={styles.formGroup}>
          <Input
            label="Programme Name"
            placeholder="e.g., Strength Building Phase"
            value={createForm.name}
            onChangeText={(name) => setCreateForm({ ...createForm, name })}
          />
          <Input
            label="Description (optional)"
            placeholder="What are the goals of this programme?"
            value={createForm.description}
            onChangeText={(description) => setCreateForm({ ...createForm, description })}
            multiline
          />
          <Select
            label="Duration"
            value={createForm.durationWeeks.toString()}
            options={durationOptions}
            onChange={(value) =>
              setCreateForm({ ...createForm, durationWeeks: parseInt(value) })
            }
          />
          <Input
            label="Start Date"
            value={createForm.startDate}
            onChangeText={(startDate) => setCreateForm({ ...createForm, startDate })}
            placeholder="YYYY-MM-DD"
          />
        </View>
      </Modal>

      {/* Edit Programme Modal */}
      <Modal
        visible={showEditModal}
        onClose={() => setShowEditModal(false)}
        title="Edit Programme"
        footer={
          <View style={styles.modalFooter}>
            <Button
              title="Cancel"
              onPress={() => setShowEditModal(false)}
              variant="ghost"
            />
            <Button
              title={updateMutation.isPending ? 'Saving...' : 'Save Changes'}
              onPress={() =>
                selectedProgramme &&
                updateMutation.mutate({ id: selectedProgramme.id, data: editForm })
              }
              variant="primary"
              disabled={!editForm.name || updateMutation.isPending}
              loading={updateMutation.isPending}
            />
          </View>
        }
      >
        <View style={styles.formGroup}>
          <Input
            label="Programme Name"
            placeholder="e.g., Strength Building Phase"
            value={editForm.name}
            onChangeText={(name) => setEditForm({ ...editForm, name })}
          />
          <Input
            label="Description (optional)"
            placeholder="What are the goals of this programme?"
            value={editForm.description}
            onChangeText={(description) => setEditForm({ ...editForm, description })}
            multiline
          />
          <View style={styles.checkboxContainer}>
            <Pressable
              style={styles.checkbox}
              onPress={() => setEditForm({ ...editForm, isActive: !editForm.isActive })}
            >
              <View
                style={[
                  styles.checkboxBox,
                  editForm.isActive && styles.checkboxBoxChecked,
                ]}
              >
                {editForm.isActive && <Text style={styles.checkboxCheck}>✓</Text>}
              </View>
              <Text style={styles.checkboxLabel}>Set as Active Programme</Text>
            </Pressable>
          </View>
        </View>
      </Modal>

      {/* Clone Template Modal */}
      <Modal
        visible={showCloneModal}
        onClose={() => setShowCloneModal(false)}
        title="Start Programme"
        footer={
          <View style={styles.modalFooter}>
            <Button
              title="Cancel"
              onPress={() => setShowCloneModal(false)}
              variant="ghost"
            />
            <Button
              title={cloneMutation.isPending ? 'Starting...' : 'Start Programme'}
              onPress={() =>
                selectedProgramme &&
                cloneMutation.mutate({ programmeId: selectedProgramme.id, data: cloneForm })
              }
              variant="primary"
              disabled={cloneMutation.isPending}
              loading={cloneMutation.isPending}
            />
          </View>
        }
      >
        {selectedProgramme && (
          <View style={styles.formGroup}>
            <View style={styles.templatePreview}>
              <Text style={styles.templatePreviewTitle}>{selectedProgramme.name}</Text>
              <Text style={styles.templatePreviewDesc}>
                {selectedProgramme.description}
              </Text>
              <Text style={styles.templatePreviewMeta}>
                Duration: {selectedProgramme.durationWeeks} weeks
              </Text>
            </View>
            <Input
              label="Start Date"
              value={cloneForm.startDate}
              onChangeText={(startDate) => setCloneForm({ ...cloneForm, startDate })}
              placeholder="YYYY-MM-DD"
            />
          </View>
        )}
      </Modal>

      {/* Programme Detail Modal */}
      {selectedProgrammeId && (
        <ProgrammeDetailModal
          programmeId={selectedProgrammeId}
          visible={showDetailModal}
          onClose={() => {
            setShowDetailModal(false);
            setSelectedProgrammeId(null);
          }}
        />
      )}
    </>
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
      marginBottom: spacing.md,
    },
    grid: {
      gap: spacing.md,
    },
    section: {
      marginTop: spacing.md,
      gap: spacing.sm,
    },
    sectionTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
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
      padding: spacing.xl,
      alignItems: 'center',
      marginTop: spacing.lg,
    },
    placeholderIcon: {
      fontSize: 64,
      marginBottom: spacing.md,
    },
    placeholderTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    placeholderText: {
      ...typography.body,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      textAlign: 'center',
      marginBottom: spacing.md,
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
    cardHeaderBadges: {
      flexDirection: 'row',
      gap: spacing.xs,
    },
    cardTitle: {
      ...typography.body,
      fontWeight: '600',
      color: isDark ? colors.dark.text : colors.text,
      flex: 1,
    },
    cardSubtitle: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.sm,
    },
    cardMeta: {
      gap: spacing.xs,
      marginBottom: spacing.sm,
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
    cardActions: {
      flexDirection: 'row',
      gap: spacing.xs,
    },
    modalFooter: {
      flexDirection: 'row',
      justifyContent: 'flex-end',
      gap: spacing.sm,
    },
    formGroup: {
      gap: spacing.md,
    },
    checkboxContainer: {
      marginTop: spacing.sm,
    },
    checkbox: {
      flexDirection: 'row',
      alignItems: 'center',
      gap: spacing.sm,
    },
    checkboxBox: {
      width: 24,
      height: 24,
      borderRadius: borderRadius.sm,
      borderWidth: 2,
      borderColor: isDark ? colors.dark.border : colors.border,
      alignItems: 'center',
      justifyContent: 'center',
    },
    checkboxBoxChecked: {
      backgroundColor: colors.primary,
      borderColor: colors.primary,
    },
    checkboxCheck: {
      color: '#FFFFFF',
      fontSize: 16,
      fontWeight: 'bold',
    },
    checkboxLabel: {
      ...typography.body,
      color: isDark ? colors.dark.text : colors.text,
    },
    templatePreview: {
      backgroundColor: isDark
        ? 'rgba(99, 102, 241, 0.1)'
        : 'rgba(99, 102, 241, 0.05)',
      borderRadius: borderRadius.md,
      padding: spacing.md,
      marginBottom: spacing.md,
    },
    templatePreviewTitle: {
      ...typography.h3,
      color: isDark ? colors.dark.text : colors.text,
      marginBottom: spacing.xs,
    },
    templatePreviewDesc: {
      ...typography.bodySmall,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
      marginBottom: spacing.sm,
    },
    templatePreviewMeta: {
      ...typography.caption,
      color: isDark ? colors.dark.textSecondary : colors.textSecondary,
    },
  });
