# Frontend Dockerfile for trAInr React App

# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY trainr/package*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY trainr/ .

# Set environment variable for API URL
ARG REACT_APP_API_URL=http://localhost:8080/api
ENV REACT_APP_API_URL=$REACT_APP_API_URL

# Build the application
RUN npm run build

# Production stage - serve with nginx
FROM nginx:alpine AS production

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built assets from build stage
COPY --from=build /app/build /usr/share/nginx/html

# Expose port 80
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

