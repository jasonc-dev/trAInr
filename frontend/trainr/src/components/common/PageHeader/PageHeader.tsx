/**
 * PageHeader Component
 * Consistent page header with title, subtitle, and optional actions
 */

import React from "react";
import { Link } from "react-router-dom";
import styled from "styled-components";
import { Flex } from "../../styled";
import { PageHeaderWrapper, Title, Subtitle } from "./PageHeader.styles";

const BackLink = styled(Link)`
  display: inline-flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.xs};
  color: ${({ theme }) => theme.colors.textSecondary};
  text-decoration: none;
  font-size: ${({ theme }) => theme.fontSizes.sm};
  margin-bottom: ${({ theme }) => theme.spacing.md};
  transition: color ${({ theme }) => theme.transitions.fast};

  &:hover {
    color: ${({ theme }) => theme.colors.primary};
  }
`;

interface PageHeaderProps {
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
  backTo?: string;
  backLabel?: string;
}

export const PageHeader: React.FC<PageHeaderProps> = ({
  title,
  subtitle,
  actions,
  backTo,
  backLabel,
}) => {
  return (
    <div style={{ marginBottom: "2rem" }}>
      {backTo && (
        <BackLink to={backTo}>
          ← {backLabel || "Back"}
        </BackLink>
      )}
      <Flex justify="space-between" align="flex-start">
        <PageHeaderWrapper>
          <Title>{title}</Title>
          {subtitle && <Subtitle>{subtitle}</Subtitle>}
        </PageHeaderWrapper>
        {actions && <div>{actions}</div>}
      </Flex>
    </div>
  );
};
