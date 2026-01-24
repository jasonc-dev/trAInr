import React from "react";
import { Select } from "../components/styled";

export type NumberPickerType = 'sets' | 'reps' | 'weight' | 'rest' | 'rpe' | 'drops' | 'percentage';

interface NumberPickerProps {
  label?: string;
  $size?: 'md' | 'lg';
  value: number | null;
  onChange: (value: number) => void;
  type: NumberPickerType;
  disabled?: boolean;
}

export const NumberPicker: React.FC<NumberPickerProps> = ({
  label,
  $size = 'md',
  value,
  onChange,
  type,
  disabled = false
}) => {
  const getOptions = () => {
    switch (type) {
      case 'sets':
        return Array.from({ length: 15 }, (_, i) => ({
          value: (i + 1).toString(),
          label: (i + 1).toString()
        }));
      case 'reps':
        return Array.from({ length: 50 }, (_, i) => ({
          value: (i + 1).toString(),
          label: (i + 1).toString()
        }));
      case 'weight':
        const weightOptions = [];
        for (let i = 0; i <= 600; i += 0.5) {
          weightOptions.push({
            value: i.toString(),
            label: i.toString()
          });
        }
        return weightOptions;
      case 'rest':
        const restOptions = [];
        for (let i = 0; i <= 600; i += 15) {
          const minutes = Math.floor(i / 60);
          const seconds = i % 60;
          const label = minutes > 0
            ? `${minutes}:${seconds.toString().padStart(2, '0')}`
            : `${seconds}s`;
          restOptions.push({
            value: i.toString(),
            label
          });
        }
        return restOptions;
      case 'rpe':
        return Array.from({ length: 10 }, (_, i) => ({
          value: (i + 1).toString(),
          label: (i + 1).toString()
        }));
      case 'drops':
        return Array.from({ length: 5 }, (_, i) => ({
          value: (i + 1).toString(),
          label: (i + 1).toString()
        }));
      case 'percentage':
        return Array.from({ length: 9 }, (_, i) => {
          const value = 10 + i * 5; // 10, 15, 20, 25, 30, 35, 40, 45, 50
          return {
            value: value.toString(),
            label: `${value}%`
          };
        });
      default:
        return [];
    }
  };

  const options = getOptions();

  return (
    <Select
      label={label}
      options={options}
      $size={$size}
      value={value?.toString() || ''}
      onChange={(e) => onChange(parseFloat(e.target.value))}
      disabled={disabled}
    />
  );
};