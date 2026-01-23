import styled from "styled-components";

export const Container = styled.div`
  width: 100%;
  max-width: 100%;
  margin: 0 auto;
  padding: 0 ${({ theme }) => theme.spacing.lg};
  box-sizing: border-box;

  @media (max-width: ${({ theme }) => theme.breakpoints.lg}) {
    padding: 0 ${({ theme }) => theme.spacing.md};
  }

  @media (max-width: ${({ theme }) => theme.breakpoints.md}) {
    width: 100%;
    max-width: 100%;
    padding: 0 ${({ theme }) => theme.spacing.sm};
    display: flex;
    flex-direction: column;
    align-items: center;

    & > * {
      width: 100%;
    }
  }

  @media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
    padding: 0 ${({ theme }) => theme.spacing.sm};
  }
`;

export const PageWrapper = styled.main`
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: stretch;
  width: 100%;
  max-width: 100vw;
  overflow-x: hidden;
  box-sizing: border-box;
  padding-top: ${({ theme }) => theme.spacing.md};
`;

export const Grid = styled.div<{ $columns?: number; $gap?: string }>`
  display: grid;
  grid-template-columns: repeat(${({ $columns }) => $columns || 1}, 1fr);
  gap: ${({ $gap, theme }) => $gap || theme.spacing.md};
  width: 100%;
  max-width: 100%;
  box-sizing: border-box;

  @media (max-width: ${({ theme }) => theme.breakpoints.lg}) {
    grid-template-columns: repeat(
      ${({ $columns }) => Math.min($columns || 1, 2)},
      1fr
    );
  }

  @media (max-width: ${({ theme }) => theme.breakpoints.md}) {
    grid-template-columns: 1fr;
  }
`;

export const Flex = styled.div<{
  direction?: "row" | "column";
  $align?: string;
  $justify?: string;
  $gap?: string;
  $wrap?: boolean;
}>`
  display: flex;
  flex-direction: ${({ direction }) => direction || "row"};
  align-items: ${({ $align }) => $align || "stretch"};
  justify-content: ${({ $justify }) => $justify || "flex-start"};
  gap: ${({ $gap, theme }) => $gap || theme.spacing.md};
  flex-wrap: ${({ $wrap }) => ($wrap ? "wrap" : "nowrap")};
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
