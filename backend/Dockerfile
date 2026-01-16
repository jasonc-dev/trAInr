# Backend Dockerfile for trAInr API
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["backend/trAInr.API/trAInr.API.csproj", "trAInr.API/"]
COPY ["backend/trAInr.Application/trAInr.Application.csproj", "trAInr.Application/"]
COPY ["backend/trAInr.Domain/trAInr.Domain.csproj", "trAInr.Domain/"]
COPY ["backend/trAInr.Infrastructure/trAInr.Infrastructure.csproj", "trAInr.Infrastructure/"]
RUN dotnet restore "trAInr.API/trAInr.API.csproj"

# Copy the rest of the source code
COPY backend/ .
WORKDIR "/src/trAInr.API"

# Build the application
RUN dotnet build "trAInr.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "trAInr.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy seed data
COPY --from=build /src/trAInr.Infrastructure/Data ./Data/

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "trAInr.API.dll"]

