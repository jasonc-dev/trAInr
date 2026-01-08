/**
 * Application Router
 * Centralized routing configuration with protected and public routes
 */

import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { MainLayout } from "./layouts";
import { useUser } from "./hooks";

// Lazy load pages for better performance
const LoginPage = React.lazy(() =>
  import("./pages/Login").then((m) => ({ default: m.Login }))
);
const RegisterPage = React.lazy(() =>
  import("./pages/Register").then((m) => ({ default: m.Register }))
);
const DashboardPage = React.lazy(() =>
  import("./pages/Dashboard").then((m) => ({ default: m.Dashboard }))
);
const ProgrammesPage = React.lazy(() =>
  import("./pages/Programmes").then((m) => ({ default: m.Programmes }))
);
const ProgrammeDetailPage = React.lazy(() =>
  import("./pages/ProgrammeDetail").then((m) => ({
    default: m.ProgrammeDetail,
  }))
);
const WorkoutPage = React.lazy(() =>
  import("./pages/Workout").then((m) => ({ default: m.Workout }))
);
const WorkoutDetailPage = React.lazy(() =>
  import("./pages/WorkoutDetail").then((m) => ({ default: m.WorkoutDetail }))
);
const ExercisesPage = React.lazy(() =>
  import("./pages/Exercises").then((m) => ({ default: m.Exercises }))
);

// Loading fallback
const PageLoader: React.FC = () => (
  <div
    style={{
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      height: "100vh",
      color: "#A0AEC0",
    }}
  >
    Loading...
  </div>
);

// Protected route wrapper
interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, loading } = useUser();

  if (loading) {
    return <PageLoader />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

// Public route wrapper (redirects authenticated users)
interface PublicRouteProps {
  children: React.ReactNode;
}

const PublicRoute: React.FC<PublicRouteProps> = ({ children }) => {
  const { isAuthenticated, loading } = useUser();

  if (loading) {
    return <PageLoader />;
  }

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
};

// Route constants for type-safe navigation
export const ROUTES = {
  LOGIN: "/login",
  REGISTER: "/register",
  DASHBOARD: "/dashboard",
  PROGRAMMES: "/programmes",
  PROGRAMME_DETAIL: "/programmes/:id",
  WORKOUT: "/workout",
  WORKOUT_DETAIL: "/workout/:workoutId",
  EXERCISES: "/exercises",
} as const;

// Main router component
export const AppRouter: React.FC = () => {
  return (
    <BrowserRouter>
      <React.Suspense fallback={<PageLoader />}>
        <Routes>
          {/* Public routes */}
          <Route
            path={ROUTES.LOGIN}
            element={
              <PublicRoute>
                <LoginPage />
              </PublicRoute>
            }
          />
          <Route
            path={ROUTES.REGISTER}
            element={
              <PublicRoute>
                <RegisterPage />
              </PublicRoute>
            }
          />

          {/* Protected routes with MainLayout */}
          <Route
            element={
              <ProtectedRoute>
                <MainLayout />
              </ProtectedRoute>
            }
          >
            <Route path={ROUTES.DASHBOARD} element={<DashboardPage />} />
            <Route path={ROUTES.PROGRAMMES} element={<ProgrammesPage />} />
            <Route
              path={ROUTES.PROGRAMME_DETAIL}
              element={<ProgrammeDetailPage />}
            />
            <Route path={ROUTES.WORKOUT} element={<WorkoutPage />} />
            <Route
              path={ROUTES.WORKOUT_DETAIL}
              element={<WorkoutDetailPage />}
            />
            <Route path={ROUTES.EXERCISES} element={<ExercisesPage />} />
          </Route>

          {/* Fallback routes */}
          <Route
            path="/"
            element={<Navigate to={ROUTES.DASHBOARD} replace />}
          />
          <Route path="*" element={<Navigate to={ROUTES.LOGIN} replace />} />
        </Routes>
      </React.Suspense>
    </BrowserRouter>
  );
};
