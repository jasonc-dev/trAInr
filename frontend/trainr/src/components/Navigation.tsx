import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import styled from 'styled-components';

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
`;

const NavContainer = styled.div`
  max-width: 1400px;
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
  font-size: ${({ theme }) => theme.fontSizes['2xl']};
  font-weight: ${({ theme }) => theme.fontWeights.extrabold};
  color: ${({ theme }) => theme.colors.text};
  text-decoration: none;
  
  span {
    background: linear-gradient(135deg, ${({ theme }) => theme.colors.primary} 0%, ${({ theme }) => theme.colors.secondary} 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }
`;

const NavLinks = styled.div`
  display: flex;
  align-items: center;
  gap: ${({ theme }) => theme.spacing.xs};
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
    content: '';
    position: absolute;
    bottom: -2px;
    left: 50%;
    transform: translateX(-50%) scaleX(${({ $active }) => $active ? 1 : 0});
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

export const Navigation: React.FC = () => {
  const location = useLocation();
  
  const isActive = (path: string) => location.pathname === path;
  
  return (
    <Nav>
      <NavContainer>
        <Logo to="/">
          <span>trAInr</span>
        </Logo>
        
        <NavLinks>
          <NavLink to="/dashboard" $active={isActive('/dashboard')}>
            <IconWrapper>📊</IconWrapper>
            Dashboard
          </NavLink>
          <NavLink to="/programmes" $active={isActive('/programmes')}>
            <IconWrapper>📋</IconWrapper>
            Programmes
          </NavLink>
          <NavLink to="/workout" $active={isActive('/workout')}>
            <IconWrapper>🏋️</IconWrapper>
            Workout
          </NavLink>
          <NavLink to="/exercises" $active={isActive('/exercises')}>
            <IconWrapper>💪</IconWrapper>
            Exercises
          </NavLink>
        </NavLinks>
      </NavContainer>
    </Nav>
  );
};

