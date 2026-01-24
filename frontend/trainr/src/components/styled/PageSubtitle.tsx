import styled from "styled-components";

export const PageSubtitle = styled.p`
color: ${({ theme }) => theme.colors.textSecondary};
margin-bottom: ${({ theme }) => theme.spacing["2xl"]};

@media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
  margin-bottom: ${({ theme }) => theme.spacing.lg};
}
`;