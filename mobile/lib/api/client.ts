/**
 * API Client for Mobile
 * Configured axios client with secure storage and navigation adapters
 */

import axios, {
  AxiosInstance,
  AxiosError,
  InternalAxiosRequestConfig,
} from "axios";
import { router } from "expo-router";
import { secureStorage } from "../storage/secureStorage";

// API base URL - set EXPO_PUBLIC_API_URL when starting Expo (e.g. for device: http://YOUR_IP:5001/api/v1)
const API_BASE_URL =
  process.env.EXPO_PUBLIC_API_URL || "http://localhost:5001/api/v1";

// Log once so you can verify requests hit your backend (check Metro terminal)
if (__DEV__) {
  console.log("[trAInr API]", "Base URL:", API_BASE_URL);
  if (API_BASE_URL.includes(":5000/")) {
    console.warn(
      "[trAInr API] Using port 5000 — backend uses 5001. Update .env or EXPO_PUBLIC_API_URL to ...:5001/api/v1 and restart Expo."
    );
  }
}

export function getApiBaseUrl(): string {
  return API_BASE_URL;
}

interface ApiError {
  code: string;
  message: string;
  requestId: string;
  retryable: boolean;
}

interface AuthResponse {
  id: string;
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  refreshTokenExpiresAt: string;
}

// Create axios instance
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor - add auth token
apiClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    const token = await secureStorage.getAccessToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor - handle 401 and refresh token
let isRefreshing = false;
let refreshSubscribers: ((token: string) => void)[] = [];

const subscribeTokenRefresh = (callback: (token: string) => void) => {
  refreshSubscribers.push(callback);
};

const onRefreshed = (token: string) => {
  refreshSubscribers.forEach((callback) => callback(token));
  refreshSubscribers = [];
};

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ApiError>) => {
    const originalRequest = error.config;

    // Handle network errors (backend not available)
    if (error.code === "ERR_NETWORK" || !error.response) {
      const friendlyError = new Error(
        "Cannot connect to server. Please check your internet connection and ensure the backend is running."
      );
      return Promise.reject(friendlyError);
    }

    // 403 often means wrong host (request not reaching your backend) or CORS
    if (error.response?.status === 403 && __DEV__) {
      console.warn(
        "[trAInr API] 403 Forbidden. Request may not be reaching your backend. API base:",
        API_BASE_URL
      );
    }

    if (!originalRequest) {
      return Promise.reject(error);
    }

    // Handle 401 Unauthorized
    if (error.response?.status === 401) {
      const refreshToken = await secureStorage.getRefreshToken();

      if (!refreshToken) {
        await secureStorage.clearAll();
        router.replace("/(auth)/login");
        return Promise.reject(error);
      }

      if (!isRefreshing) {
        isRefreshing = true;

        try {
          const response = await axios.post<AuthResponse>(
            `${API_BASE_URL}/auth/refresh`,
            { refreshToken }
          );

          const { accessToken, refreshToken: newRefreshToken } = response.data;

          await secureStorage.setAccessToken(accessToken);
          await secureStorage.setRefreshToken(newRefreshToken);

          onRefreshed(accessToken);
          isRefreshing = false;

          // Retry original request
          originalRequest.headers = originalRequest.headers ?? {};
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return apiClient(originalRequest);
        } catch (refreshError) {
          isRefreshing = false;
          await secureStorage.clearAll();
          router.replace("/(auth)/login");
          return Promise.reject(refreshError);
        }
      }

      // Wait for token refresh
      return new Promise((resolve) => {
        subscribeTokenRefresh((token: string) => {
          originalRequest.headers = originalRequest.headers ?? {};
          originalRequest.headers.Authorization = `Bearer ${token}`;
          resolve(apiClient(originalRequest));
        });
      });
    }

    return Promise.reject(error);
  }
);

export { apiClient };

export function extractApiError(error: unknown): ApiError | null {
  if (axios.isAxiosError(error) && error.response?.data) {
    const data = error.response.data;
    if (data.code && data.message) {
      return data as ApiError;
    }
  }
  return null;
}
