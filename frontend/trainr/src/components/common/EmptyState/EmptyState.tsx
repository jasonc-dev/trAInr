/**
 * EmptyState Component
 * Displays a placeholder when there's no data
 */

import React from "react";
import { Card } from "../../styled";
import {
  EmptyStateWrapper,
  Icon,
  Title,
  Description,
} from "./EmptyState.styles";

interface EmptyStateProps {
  icon: string;
  title: string;
  description?: string;
  children?: React.ReactNode;
  actions?: React.ReactNode;
}

export const EmptyState: React.FC<EmptyStateProps> = ({
  icon,
  title,
  description,
  children,
  actions,
}) => {
  return (
    <Card>
      <EmptyStateWrapper>
        <Icon>{icon}</Icon>
        <Title>{title}</Title>
        {description && <Description>{description}</Description>}
        {children || actions}
      </EmptyStateWrapper>
    </Card>
  );
};
