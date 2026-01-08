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
  NavLink,
  IconWrapper,
  LogoutButton,
} from "./MainLayout.styles";

export const MainLayout: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { logout } = useAuth();

  const isActive = (path: string) => location.pathname === path;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <>
      <Nav>
        <NavContainer>
          <Logo to="/dashboard">
            <span>trAInr</span>
          </Logo>

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
      </Nav>
      <PageWrapper>
        <Outlet />
      </PageWrapper>
    </>
  );
};
