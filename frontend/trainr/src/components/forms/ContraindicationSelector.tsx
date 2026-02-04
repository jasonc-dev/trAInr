/**
 * ContraindicationSelector Component
 * Dropdown multi-select checkbox component for contraindication tags
 */

import React, { useState, useRef, useEffect } from "react";
import styled from "styled-components";
import { CONTRAINDICATION_OPTIONS } from "../../utils/constants";

interface ContraindicationSelectorProps {
  selected: string[];
  onChange: (selected: string[]) => void;
  label?: string;
}

const SelectorWrapper = styled.div`
  width: 100%;
  position: relative;
`;

const Label = styled.label`
  display: block;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  color: ${({ theme }) => theme.colors.textSecondary};
  margin-bottom: ${({ theme }) => theme.spacing.xs};
  text-transform: uppercase;
  letter-spacing: 0.5px;
`;

const DropdownButton = styled.button<{ $isOpen: boolean }>`
  width: 100%;
  padding: 0.875rem 1rem;
  font-size: ${({ theme }) => theme.fontSizes.md};
  color: ${({ theme }) => theme.colors.text};
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border: 1px solid ${({ theme, $isOpen }) =>
    $isOpen ? theme.colors.primary : theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.lg};
  transition: all ${({ theme }) => theme.transitions.fast};
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: space-between;
  text-align: left;
  box-shadow: ${({ theme, $isOpen }) =>
    $isOpen ? `0 0 0 3px ${theme.colors.primaryGhost}` : "none"};

  &:hover {
    border-color: ${({ theme }) => theme.colors.primary};
  }

  &:focus {
    outline: none;
    border-color: ${({ theme }) => theme.colors.primary};
    box-shadow: 0 0 0 3px ${({ theme }) => theme.colors.primaryGhost};
  }
`;

const DropdownText = styled.span<{ $hasSelection: boolean }>`
  color: ${({ theme, $hasSelection }) =>
    $hasSelection ? theme.colors.text : theme.colors.textMuted};
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
`;

const DropdownIcon = styled.svg<{ $isOpen: boolean }>`
  width: 20px;
  height: 20px;
  min-width: 20px;
  transition: transform ${({ theme }) => theme.transitions.fast};
  transform: ${({ $isOpen }) => ($isOpen ? "rotate(180deg)" : "rotate(0)")};
  color: ${({ theme }) => theme.colors.textSecondary};
`;

const DropdownMenu = styled.div<{ $isOpen: boolean }>`
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  right: 0;
  background: ${({ theme }) => theme.colors.backgroundSecondary};
  border: 1px solid ${({ theme }) => theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.lg};
  box-shadow: ${({ theme }) => theme.shadows.lg};
  max-height: 280px;
  overflow-y: auto;
  z-index: 1000;
  display: ${({ $isOpen }) => ($isOpen ? "block" : "none")};

  &::-webkit-scrollbar {
    width: 8px;
  }

  &::-webkit-scrollbar-track {
    background: ${({ theme }) => theme.colors.background};
    border-radius: ${({ theme }) => theme.radii.md};
  }

  &::-webkit-scrollbar-thumb {
    background: ${({ theme }) => theme.colors.border};
    border-radius: ${({ theme }) => theme.radii.md};

    &:hover {
      background: ${({ theme }) => theme.colors.borderLight};
    }
  }
`;

const CheckboxOption = styled.label`
  display: flex;
  align-items: center;
  padding: ${({ theme }) => theme.spacing.md} ${({ theme }) => theme.spacing.lg};
  cursor: pointer;
  transition: all ${({ theme }) => theme.transitions.fast};
  user-select: none;

  &:hover {
    background: ${({ theme }) => theme.colors.surface};
  }

  &:first-child {
    border-radius: ${({ theme }) => theme.radii.lg} ${({ theme }) => theme.radii.lg} 0 0;
  }

  &:last-child {
    border-radius: 0 0 ${({ theme }) => theme.radii.lg} ${({ theme }) => theme.radii.lg};
  }
`;

const HiddenCheckbox = styled.input.attrs({ type: "checkbox" })`
  position: absolute;
  opacity: 0;
  width: 0;
  height: 0;
`;

const StyledCheckbox = styled.div<{ $checked: boolean }>`
  width: 20px;
  height: 20px;
  min-width: 20px;
  border: 2px solid ${({ theme, $checked }) =>
    $checked ? theme.colors.primary : theme.colors.border};
  border-radius: ${({ theme }) => theme.radii.sm};
  background: ${({ theme, $checked }) =>
    $checked ? theme.colors.primary : "transparent"};
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: ${({ theme }) => theme.spacing.md};
  transition: all ${({ theme }) => theme.transitions.fast};

  &::after {
    content: "";
    display: ${({ $checked }) => ($checked ? "block" : "none")};
    width: 5px;
    height: 9px;
    border: solid white;
    border-width: 0 2px 2px 0;
    transform: rotate(45deg);
  }
`;

const CheckboxText = styled.span`
  font-size: ${({ theme }) => theme.fontSizes.sm};
  color: ${({ theme }) => theme.colors.text};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
`;

export const ContraindicationSelector: React.FC<ContraindicationSelectorProps> = ({
  selected,
  onChange,
  label = "Contraindications",
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const wrapperRef = useRef<HTMLDivElement>(null);

  const handleToggle = (contraindication: string) => {
    if (selected.includes(contraindication)) {
      onChange(selected.filter((item) => item !== contraindication));
    } else {
      onChange([...selected, contraindication]);
    }
  };

  const getDisplayText = () => {
    if (selected.length === 0) {
      return "Select contraindications...";
    }
    if (selected.length === 1) {
      const option = CONTRAINDICATION_OPTIONS.find((opt) => opt.value === selected[0]);
      return option?.label || selected[0];
    }
    return `${selected.length} items selected`;
  };

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };

    if (isOpen) {
      document.addEventListener("mousedown", handleClickOutside);
    }

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isOpen]);

  return (
    <SelectorWrapper ref={wrapperRef}>
      <Label>{label}</Label>
      <DropdownButton
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        $isOpen={isOpen}
      >
        <DropdownText $hasSelection={selected.length > 0}>
          {getDisplayText()}
        </DropdownText>
        <DropdownIcon $isOpen={isOpen} viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <polyline points="6 9 12 15 18 9" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
        </DropdownIcon>
      </DropdownButton>

      <DropdownMenu $isOpen={isOpen}>
        {CONTRAINDICATION_OPTIONS.map((option) => {
          const isChecked = selected.includes(option.value);
          return (
            <CheckboxOption key={option.value}>
              <HiddenCheckbox
                id={`contraindication-${option.value}`}
                checked={isChecked}
                onChange={() => handleToggle(option.value)}
              />
              <StyledCheckbox $checked={isChecked} />
              <CheckboxText>{option.label}</CheckboxText>
            </CheckboxOption>
          );
        })}
      </DropdownMenu>
    </SelectorWrapper>
  );
};
