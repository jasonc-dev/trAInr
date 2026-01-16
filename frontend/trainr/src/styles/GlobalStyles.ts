import { createGlobalStyle } from "styled-components";

export const GlobalStyles = createGlobalStyle`
  *, *::before, *::after {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }
  
  html {
    font-size: 16px;
    scroll-behavior: smooth;
    width: 100%;
    overflow-x: hidden;
  }
  
  body {
    font-family: ${({ theme }) => theme.fonts.body};
    background: ${({ theme }) => theme.colors.background};
    color: ${({ theme }) => theme.colors.text};
    line-height: 1.6;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    min-height: 100vh;
    width: 100%;
    overflow-x: hidden;
    
    /* Subtle gradient background */
    background: 
      radial-gradient(ellipse at 20% 0%, rgba(0, 207, 193, 0.08) 0%, transparent 50%),
      radial-gradient(ellipse at 80% 100%, rgba(124, 77, 255, 0.06) 0%, transparent 50%),
      ${({ theme }) => theme.colors.background};
  }
  
  h1, h2, h3, h4, h5, h6 {
    font-family: ${({ theme }) => theme.fonts.heading};
    font-weight: ${({ theme }) => theme.fontWeights.bold};
    line-height: 1.2;
    letter-spacing: -0.02em;
  }

  #root {
    min-height: 100vh;
    width: 100%;
    overflow-x: hidden;
  }
  
  h1 { font-size: ${({ theme }) => theme.fontSizes["5xl"]}; }
  h2 { font-size: ${({ theme }) => theme.fontSizes["4xl"]}; }
  h3 { font-size: ${({ theme }) => theme.fontSizes["3xl"]}; }
  h4 { font-size: ${({ theme }) => theme.fontSizes["2xl"]}; }
  h5 { font-size: ${({ theme }) => theme.fontSizes.xl}; }
  h6 { font-size: ${({ theme }) => theme.fontSizes.lg}; }
  
  a {
    color: ${({ theme }) => theme.colors.primary};
    text-decoration: none;
    transition: color ${({ theme }) => theme.transitions.fast};
    
    &:hover {
      color: ${({ theme }) => theme.colors.primaryLight};
    }
  }
  
  button {
    font-family: ${({ theme }) => theme.fonts.body};
    cursor: pointer;
    border: none;
    outline: none;
    
    &:disabled {
      cursor: not-allowed;
      opacity: 0.6;
    }
  }
  
  input, textarea, select {
    font-family: ${({ theme }) => theme.fonts.body};
    font-size: ${({ theme }) => theme.fontSizes.md};
  }
  
  ::selection {
    background: ${({ theme }) => theme.colors.primary};
    color: ${({ theme }) => theme.colors.background};
  }
  
  /* Custom scrollbar */
  ::-webkit-scrollbar {
    width: 8px;
    height: 8px;
  }
  
  ::-webkit-scrollbar-track {
    background: ${({ theme }) => theme.colors.backgroundSecondary};
  }
  
  ::-webkit-scrollbar-thumb {
    background: ${({ theme }) => theme.colors.border};
    border-radius: ${({ theme }) => theme.radii.full};
    
    &:hover {
      background: ${({ theme }) => theme.colors.borderLight};
    }
  }
  
  /* Focus styles */
  :focus-visible {
    outline: 2px solid ${({ theme }) => theme.colors.primary};
    outline-offset: 2px;
  }
  
  /* Utility classes for animations */
  @keyframes fadeIn {
    from { opacity: 0; }
    to { opacity: 1; }
  }
  
  @keyframes slideUp {
    from { 
      opacity: 0;
      transform: translateY(20px);
    }
    to { 
      opacity: 1;
      transform: translateY(0);
    }
  }
  
  @keyframes pulse {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.5; }
  }

  @keyframes shimmer {
    0% { background-position: -200% 0; }
    100% { background-position: 200% 0; }
  }
`;
