import styled from "styled-components";
import { Flex } from "../../styled";

export const PageHeader = styled(Flex)`
  margin-bottom: ${({ theme }) => theme.spacing.xl};
  margin-top: 1rem;

  @media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
    flex-direction: column;
    align-items: flex-start;
    gap: ${({ theme }) => theme.spacing.md};
  }
`;