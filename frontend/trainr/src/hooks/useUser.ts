import { useState, useEffect, useCallback } from "react";
import { getAuthToken } from "../api/client";

const USER_STORAGE_KEY = "trainr_user";

interface StoredUser {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  expiresAt: string;
}

export const useUser = () => {
  const [user, setUser] = useState<StoredUser | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadUser = useCallback(() => {
    const token = getAuthToken();
    const storedUser = localStorage.getItem(USER_STORAGE_KEY);
    
    if (!token || !storedUser) {
      setUser(null);
      setLoading(false);
      return;
    }

    try {
      const parsed = JSON.parse(storedUser) as StoredUser;
      const expiresAt = new Date(parsed.expiresAt);
      
      if (expiresAt > new Date()) {
        setUser(parsed);
        setError(null);
      } else {
        // Token expired
        localStorage.removeItem(USER_STORAGE_KEY);
        setUser(null);
      }
    } catch (err) {
      console.error("Failed to parse stored user:", err);
      localStorage.removeItem(USER_STORAGE_KEY);
      setUser(null);
      setError("Failed to load user");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadUser();
  }, [loadUser]);

  // Listen for storage changes (e.g., logout in another tab)
  useEffect(() => {
    const handleStorageChange = (e: StorageEvent) => {
      if (e.key === USER_STORAGE_KEY || e.key === "trainr_auth_token") {
        loadUser();
      }
    };

    window.addEventListener("storage", handleStorageChange);
    return () => window.removeEventListener("storage", handleStorageChange);
  }, [loadUser]);

  return {
    user,
    loading,
    error,
    isAuthenticated: !!user,
    userId: user?.id ?? null,
  };
};
