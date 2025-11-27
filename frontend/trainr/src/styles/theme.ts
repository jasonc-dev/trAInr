export const theme = {
  colors: {
    // Primary palette - vibrant teal/cyan
    primary: "#00CFC1",
    primaryDark: "#00A89D",
    primaryLight: "#33D9CD",
    primaryGhost: "rgba(0, 207, 193, 0.1)",

    // Accent - energetic coral/orange
    accent: "#FF6B4A",
    accentDark: "#E5523A",
    accentLight: "#FF8A70",

    // Secondary - electric purple
    secondary: "#7C4DFF",
    secondaryDark: "#651FFF",
    secondaryLight: "#B388FF",

    // Backgrounds - deep dark with subtle gradients
    background: "#0A0E17",
    backgroundSecondary: "#121822",
    backgroundTertiary: "#1A2234",
    surface: "#1E2740",
    surfaceHover: "#252F4A",

    // Text
    text: "#FFFFFF",
    textSecondary: "#A0AEC0",
    textMuted: "#64748B",

    // Semantic colors
    success: "#00D68F",
    successLight: "rgba(0, 214, 143, 0.15)",
    warning: "#FFB547",
    warningLight: "rgba(255, 181, 71, 0.15)",
    error: "#FF4757",
    errorLight: "rgba(255, 71, 87, 0.15)",
    info: "#00C2FF",
    infoLight: "rgba(0, 194, 255, 0.15)",

    // Borders
    border: "#2D3748",
    borderLight: "#4A5568",

    // Chart colors
    chart: {
      volume: "#00CFC1",
      intensity: "#FF6B4A",
      reps: "#7C4DFF",
      weight: "#FFB547",
      duration: "#00C2FF",
    },
  },

  fonts: {
    heading: "'Outfit', sans-serif",
    body: "'DM Sans', sans-serif",
    mono: "'JetBrains Mono', monospace",
  },

  fontSizes: {
    xs: "0.75rem",
    sm: "0.875rem",
    md: "1rem",
    lg: "1.125rem",
    xl: "1.25rem",
    "2xl": "1.5rem",
    "3xl": "1.875rem",
    "4xl": "2.25rem",
    "5xl": "3rem",
    "6xl": "3.75rem",
  },

  fontWeights: {
    normal: 400,
    medium: 500,
    semibold: 600,
    bold: 700,
    extrabold: 800,
  },

  spacing: {
    xs: "0.25rem",
    sm: "0.5rem",
    md: "1rem",
    lg: "1.5rem",
    xl: "2rem",
    "2xl": "3rem",
    "3xl": "4rem",
  },

  radii: {
    sm: "0.375rem",
    md: "0.5rem",
    lg: "0.75rem",
    xl: "1rem",
    "2xl": "1.5rem",
    full: "9999px",
  },

  shadows: {
    sm: "0 1px 2px 0 rgba(0, 0, 0, 0.3)",
    md: "0 4px 6px -1px rgba(0, 0, 0, 0.3), 0 2px 4px -1px rgba(0, 0, 0, 0.2)",
    lg: "0 10px 15px -3px rgba(0, 0, 0, 0.3), 0 4px 6px -2px rgba(0, 0, 0, 0.15)",
    xl: "0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 10px 10px -5px rgba(0, 0, 0, 0.1)",
    glow: "0 0 20px rgba(0, 207, 193, 0.3)",
    glowAccent: "0 0 20px rgba(255, 107, 74, 0.3)",
  },

  transitions: {
    fast: "150ms ease",
    normal: "250ms ease",
    slow: "350ms ease",
  },

  breakpoints: {
    sm: "640px",
    md: "768px",
    lg: "1024px",
    xl: "1280px",
    "2xl": "1536px",
  },
};

export type Theme = typeof theme;
