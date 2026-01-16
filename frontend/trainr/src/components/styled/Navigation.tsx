import React from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import styled from "styled-components";
import { useAuth } from "../../hooks/useAuth";

const Nav = styled.nav`
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 72px;
  background: rgba(10, 14, 23, 0.85);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid ${({ theme }) => theme.colors.border};
  z-index: 1000;
  overflow: visible;
`;

const NavContainer = styled.div`
  width: 100%;
  margin: 0 auto;
  height: 100%;
  padding: 0 ${({ theme }) => theme.spacing.lg};
  display: flex;
  align-items: center;
  justify-content: space-between;
`;

const Logo = styled(Link)`
  display: flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.sm};
  font-family: ${({ theme }) => theme.fonts.heading};
  font-size: ${({ theme }) => theme.fontSizes["2xl"]};
  font-weight: ${({ theme }) => theme.fontWeights.extrabold};
  color: ${({ theme }) => theme.colors.text};
  text-decoration: none;

  span {
    background: linear-gradient(
      135deg,
      ${({ theme }) => theme.colors.primary} 0%,
      ${({ theme }) => theme.colors.secondary} 100%
    );
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }
`;

const NavLinks = styled.div`
  display: flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.xs};

  @media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
    display: none;
  }
`;

const MenuButton = styled.button`
  display: none;
  padding: ${({ theme }) => theme.spacing.xs} ${({ theme }) =>
  theme.spacing.sm};
  border-radius: ${({ theme }) => theme.radii.md};
  border: 1px solid ${({ theme }) => theme.colors.border};
  background: ${({ theme }) => theme.colors.surface};
  color: ${({ theme }) => theme.colors.text};
  font-size: ${({ theme }) => theme.fontSizes.sm};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  cursor: pointer;
  transition: all ${({ theme }) => theme.transitions.fast};

  &:hover {
    background: ${({ theme }) => theme.colors.surfaceHover};
  }

  @media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    height: 36px;
  }
`;

const MobileMenu = styled.div<{ $isOpen: boolean }>`
  display: none;
  position: absolute;
  left: 0;
  right: 0;
  top: 100%;
  background: ${({ theme }) => theme.colors.surface};
  border-bottom: 1px solid ${({ theme }) => theme.colors.border};
  box-shadow: ${({ theme }) => theme.shadows.md};
  padding: ${({ theme }) => theme.spacing.sm} 0;

  @media (max-width: ${({ theme }) => theme.breakpoints.sm}) {
    display: ${({ $isOpen }) => ($isOpen ? "block" : "none")};
  }
`;

const MobileNavLinks = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${({ theme }) => theme.spacing.xs};
  padding: 0 ${({ theme }) => theme.spacing.sm};

  & > a,
  & > button {
    width: 100%;
    justify-content: flex-start;
  }
`;

const NavLink = styled(Link)<{ $active?: boolean }>`
  padding: ${({ theme }) => theme.spacing.sm} ${({ theme }) => theme.spacing.md};
  font-size: ${({ theme }) => theme.fontSizes.md};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  color: ${({ theme, $active }) =>
    $active ? theme.colors.primary : theme.colors.textSecondary};
  text-decoration: none;
  border-radius: ${({ theme }) => theme.radii.lg};
  transition: all ${({ theme }) => theme.transitions.fast};
  position: relative;

  &::after {
    content: "";
    position: absolute;
    bottom: -2px;
    left: 50%;
    transform: translateX(-50%) scaleX(${({ $active }) => ($active ? 1 : 0)});
    width: 60%;
    height: 2px;
    background: ${({ theme }) => theme.colors.primary};
    border-radius: ${({ theme }) => theme.radii.full};
    transition: transform ${({ theme }) => theme.transitions.fast};
  }

  &:hover {
    color: ${({ theme }) => theme.colors.text};
    background: ${({ theme }) => theme.colors.surface};

    &::after {
      transform: translateX(-50%) scaleX(1);
    }
  }
`;

const IconWrapper = styled.span`
  display: inline-flex;
  margin-right: ${({ theme }) => theme.spacing.xs};
`;

const LogoutButton = styled.button`
  padding: ${({ theme }) => theme.spacing.sm} ${({ theme }) => theme.spacing.md};
  font-size: ${({ theme }) => theme.fontSizes.md};
  font-weight: ${({ theme }) => theme.fontWeights.medium};
  color: ${({ theme }) => theme.colors.textSecondary};
  background: none;
  border: none;
  border-radius: ${({ theme }) => theme.radii.lg};
  transition: all ${({ theme }) => theme.transitions.fast};
  cursor: pointer;
  display: flex;
  align-items: center;

  &:hover {
    color: ${({ theme }) => theme.colors.error};
    background: ${({ theme }) => theme.colors.errorLight};
  }
`;

export const Navigation: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { logout } = useAuth();
  const [isMenuOpen, setIsMenuOpen] = React.useState(false);
  const isActive = (path: string) => location.pathname === path;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  React.useEffect(() => {
    setIsMenuOpen(false);
  }, [location.pathname]);

  return (
    <Nav>
      <NavContainer>
        <Logo to="/dashboard" onClick={() => setIsMenuOpen(false)}>
          <span>trAInr</span>
        </Logo>

        <MenuButton
          onClick={() => setIsMenuOpen((prev) => !prev)}
          aria-expanded={isMenuOpen}
          aria-label="Toggle navigation menu"
        >
          Menu
        </MenuButton>

        <NavLinks>
          <NavLink to="/dashboard" $active={isActive("/dashboard")}>
            <IconWrapper>📊</IconWrapper>
            Dashboard
          </NavLink>
          <NavLink to="/programmes" $active={isActive("/programmes")}>
            <IconWrapper>📋</IconWrapper>
            Programmes
          </NavLink>
          <NavLink to="/workout" $active={isActive("/workout")}>
            <IconWrapper>🏋️</IconWrapper>
            Workout
          </NavLink>
          <NavLink to="/exercises" $active={isActive("/exercises")}>
            <IconWrapper>💪</IconWrapper>
            Exercises
          </NavLink>
          <LogoutButton onClick={handleLogout}>
            <IconWrapper>🚪</IconWrapper>
            Logout
          </LogoutButton>
        </NavLinks>
      </NavContainer>
      <MobileMenu $isOpen={isMenuOpen}>
        <MobileNavLinks>
          <NavLink
            to="/dashboard"
            $active={isActive("/dashboard")}
            onClick={() => setIsMenuOpen(false)}
          >
            <IconWrapper>📊</IconWrapper>
            Dashboard
          </NavLink>
          <NavLink
            to="/programmes"
            $active={isActive("/programmes")}
            onClick={() => setIsMenuOpen(false)}
          >
            <IconWrapper>📋</IconWrapper>
            Programmes
          </NavLink>
          <NavLink
            to="/workout"
            $active={isActive("/workout")}
            onClick={() => setIsMenuOpen(false)}
          >
            <IconWrapper>🏋️</IconWrapper>
            Workout
          </NavLink>
          <NavLink
            to="/exercises"
            $active={isActive("/exercises")}
            onClick={() => setIsMenuOpen(false)}
          >
            <IconWrapper>💪</IconWrapper>
            Exercises
          </NavLink>
          <LogoutButton
            onClick={() => {
              setIsMenuOpen(false);
              handleLogout();
            }}
          >
            <IconWrapper>🚪</IconWrapper>
            Logout
          </LogoutButton>
        </MobileNavLinks>
      </MobileMenu>
    </Nav>
  );
};
