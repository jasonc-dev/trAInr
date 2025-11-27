import axios from "axios";

// In Docker, the nginx proxy handles /api routing
// In development, we need the full URL
const API_BASE_URL =
  process.env.REACT_APP_API_URL || "http://localhost:8080/api";

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: false, // Changed to false for simpler CORS handling
});

// Request interceptor for logging
apiClient.interceptors.request.use(
  (config) => {
    if (process.env.NODE_ENV === "development") {
      console.log(`[API] ${config.method?.toUpperCase()} ${config.url}`);
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      console.error(
        `[API Error] ${error.response.status}: ${JSON.stringify(error.response.data)}`
      );
    } else if (error.request) {
      console.error("[API Error] No response received");
    } else {
      console.error("[API Error]", error.message);
    }
    return Promise.reject(error);
  }
);

export default apiClient;
