import styled from "styled-components";

export const Container = styled.div`
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 ${({ theme }) => theme.spacing.lg};

  @media (max-width: ${({ theme }) => theme.breakpoints.md}) {
    padding: 0 ${({ theme }) => theme.spacing.md};
  }
`;

export const PageWrapper = styled.main`
  min-height: 100vh;
  padding-top: 20px;
  padding-bottom: ${({ theme }) => theme.spacing["2xl"]};
`;

export const Grid = styled.div<{ columns?: number; gap?: string }>`
  display: grid;
  grid-template-columns: repeat(${({ columns }) => columns || 1}, 1fr);
  gap: ${({ gap, theme }) => gap || theme.spacing.lg};

  @media (max-width: ${({ theme }) => theme.breakpoints.lg}) {
    grid-template-columns: repeat(
      ${({ columns }) => Math.min(columns || 1, 2)},
      1fr
    );
  }

  @media (max-width: ${({ theme }) => theme.breakpoints.md}) {
    grid-template-columns: 1fr;
  }
`;

export const Flex = styled.div<{
  direction?: "row" | "column";
  align?: string;
  justify?: string;
  gap?: string;
  wrap?: boolean;
}>`
  display: flex;
  flex-direction: ${({ direction }) => direction || "row"};
  align-items: ${({ align }) => align || "stretch"};
  justify-content: ${({ justify }) => justify || "flex-start"};
  gap: ${({ gap, theme }) => gap || theme.spacing.md};
  flex-wrap: ${({ wrap }) => (wrap ? "wrap" : "nowrap")};
`;

export const Stack = styled(Flex)`
  flex-direction: column;
`;

export const Spacer = styled.div<{ size?: string }>`
  height: ${({ size, theme }) => size || theme.spacing.md};
`;

export const Divider = styled.hr`
  border: none;
  height: 1px;
  background: ${({ theme }) => theme.colors.border};
  margin: ${({ theme }) => theme.spacing.lg} 0;
`;
