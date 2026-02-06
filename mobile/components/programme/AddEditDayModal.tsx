/**
 * Add/Edit Day Modal
 * Modal for creating and editing workout days
 */

import { useState, useEffect } from "react";
import { View, Text, useColorScheme, Pressable, Alert } from "react-native";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutsApi } from "../../lib/api/workouts";
import type { WorkoutDayResponse } from "../../lib/types/programme";
import { Modal, Button, Input, Select, SelectOption } from "../ui";
import { createStyles } from "./Styles";

const DAY_NAMES = [
  "Sunday",
  "Monday",
  "Tuesday",
  "Wednesday",
  "Thursday",
  "Friday",
  "Saturday",
];

interface AddEditDayModalProps {
  visible: boolean;
  onClose: () => void;
  weekId: string;
  programmeId: string;
  day?: WorkoutDayResponse;
}

export default function AddEditDayModal({
  visible,
  onClose,
  weekId,
  programmeId,
  day,
}: AddEditDayModalProps) {
  const colorScheme = useColorScheme();
  const isDark = colorScheme === "dark";
  const styles = createStyles(isDark);
  const queryClient = useQueryClient();

  const isEditing = !!day;

  const [formData, setFormData] = useState({
    name: "",
    description: "",
    scheduledDate: new Date().toISOString().split("T")[0],
    isRestDay: false,
  });

  // Initialize form data when day changes
  useEffect(() => {
    if (day) {
      setFormData({
        name: day.name,
        description: day.description || "",
        scheduledDate: day.scheduledDate.split("T")[0],
        isRestDay: day.isRestDay,
      });
    } else {
      setFormData({
        name: "",
        description: "",
        scheduledDate: new Date().toISOString().split("T")[0],
        isRestDay: false,
      });
    }
  }, [day, visible]);

  // Create mutation
  const createMutation = useMutation({
    mutationFn: () =>
      workoutsApi.createWorkoutDay(weekId, {
        ...formData,
        scheduledDate: formData.scheduledDate,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
      onClose();
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to create workout day");
    },
  });

  // Update mutation
  const updateMutation = useMutation({
    mutationFn: () =>
      workoutsApi.updateWorkoutDay(day!.id, {
        ...formData,
        scheduledDate: formData.scheduledDate,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["programme", programmeId] });
      onClose();
    },
    onError: (error: Error) => {
      Alert.alert("Error", error.message || "Failed to update workout day");
    },
  });

  const handleSubmit = () => {
    if (!formData.name) {
      Alert.alert("Validation Error", "Please enter a day name");
      return;
    }

    if (isEditing) {
      updateMutation.mutate();
    } else {
      createMutation.mutate();
    }
  };

  const isPending = createMutation.isPending || updateMutation.isPending;

  // Day of week options
  const getDayOfWeekFromDate = (dateString: string): string => {
    const date = new Date(dateString + "T00:00:00");
    return DAY_NAMES[date.getDay()];
  };

  const getDateFromDayOfWeek = (dayName: string): string => {
    const today = new Date();
    const targetDayIndex = DAY_NAMES.indexOf(dayName);
    const todayDayIndex = today.getDay();

    let dayOffset = targetDayIndex - todayDayIndex;
    if (dayOffset <= 0) dayOffset += 7;

    const targetDate = new Date(today);
    targetDate.setDate(today.getDate() + dayOffset);
    return targetDate.toISOString().split("T")[0];
  };

  const dayOptions: SelectOption[] = DAY_NAMES.map((name) => ({
    label: name,
    value: name,
  }));

  return (
    <Modal
      visible={visible}
      onClose={onClose}
      title={isEditing ? "Edit Workout Day" : "Add Workout Day"}
      footer={
        <View style={styles.footer}>
          <Button
            title="Cancel"
            onPress={onClose}
            variant="ghost"
            disabled={isPending}
          />
          <Button
            title={
              isPending
                ? isEditing
                  ? "Saving..."
                  : "Adding..."
                : isEditing
                  ? "Save Changes"
                  : "Add Day"
            }
            onPress={handleSubmit}
            variant="primary"
            disabled={!formData.name || isPending}
            loading={isPending}
          />
        </View>
      }
    >
      <View style={styles.formGroup}>
        <Input
          label="Day Name"
          placeholder="e.g., Push Day, Leg Day"
          value={formData.name}
          onChangeText={(name) => setFormData({ ...formData, name })}
        />

        <Select
          label="Day of Week"
          value={getDayOfWeekFromDate(formData.scheduledDate)}
          options={dayOptions}
          onChange={(dayName) =>
            setFormData({
              ...formData,
              scheduledDate: getDateFromDayOfWeek(dayName),
            })
          }
        />

        <Input
          label="Description (optional)"
          placeholder="Workout focus or notes"
          value={formData.description}
          onChangeText={(description) =>
            setFormData({ ...formData, description })
          }
          multiline
        />

        <View style={styles.checkboxContainer}>
          <Pressable
            style={styles.checkbox}
            onPress={() =>
              setFormData({ ...formData, isRestDay: !formData.isRestDay })
            }
          >
            <View
              style={[
                styles.checkboxBox,
                formData.isRestDay && styles.checkboxBoxChecked,
              ]}
            >
              {formData.isRestDay && (
                <Text style={styles.checkboxCheck}>✓</Text>
              )}
            </View>
            <Text style={styles.checkboxLabel}>Rest Day</Text>
          </Pressable>
        </View>
      </View>
    </Modal>
  );
}
