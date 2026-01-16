/**
 * MainLayout Component
 * Main application layout with navigation for authenticated users
 */

import React from "react";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks";
import { PageWrapper } from "../../components/styled";
import {
  Nav,
  NavContainer,
  Logo,
  NavLinks,
  MenuButton,
  MobileMenu,
  MobileNavLinks,
  NavLink,
  IconWrapper,
  LogoutButton,
} from "./MainLayout.styles";

export const MainLayout: React.FC = () => {
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
    <>
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
      <PageWrapper>
        <Outlet />
      </PageWrapper>
    </>
  );
};
