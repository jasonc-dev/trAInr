# trAInr - Workout Tracking Application

A full-stack workout tracking application built with React/TypeScript frontend and .NET 10 backend.

## Features

- **User Onboarding**: Collect fitness level, goals, and preferences
- **Programme Management**: Create 4-10 week workout programmes
- **Workout Tracking**: Track sets, reps, weight, difficulty, and intensity
- **Exercise Types**: Support for weight training, cardio, bodyweight, and flexibility exercises
- **Dashboard Analytics**: View volume trends, intensity metrics, and progress over time
- **Exercise Library**: Browse and search exercises by type and muscle group

## Tech Stack

### Frontend
- React 19 with TypeScript
- Styled Components for styling
- React Router for navigation
- Recharts for data visualization
- Axios for API communication

### Backend
- .NET 10 Web API
- Entity Framework Core with SQLite (dev) / PostgreSQL (Docker)
- RESTful API design
- Swagger/OpenAPI documentation

## Quick Start with Docker

The easiest way to run the application is using Docker Compose:

```bash
# Clone the repository
git clone <repo-url>
cd trAInr

# Build and start all services
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build
```

The application will be available at:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **Database**: PostgreSQL on port 5432

To stop the services:
```bash
docker-compose down

# To also remove the database volume
docker-compose down -v
```

## Local Development Setup

### Prerequisites

- Node.js 18+ and npm
- .NET 10 SDK
- SQLite (for local development) or PostgreSQL

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend/trAInr.API
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Run the API:
   ```bash
   dotnet run
   ```

The API will be available at `http://localhost:8080` with Swagger UI at `http://localhost:8080/swagger`.

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend/trainr
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create environment file:
   ```bash
   cp .env.example .env
   ```

4. Start the development server:
   ```bash
   npm start
   ```

The app will be available at `http://localhost:3000`.

### Using PostgreSQL for Local Development

If you want to use PostgreSQL locally instead of SQLite:

1. Start just the database:
   ```bash
   docker-compose -f docker-compose.dev.yml up -d
   ```

2. Update `appsettings.Development.json`:
   ```json
   {
     "UsePostgres": true,
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=trainr;Username=trainr;Password=trainr_password"
     }
   }
   ```

## Project Structure

```
trAInr/
├── docker-compose.yml          # Full stack Docker setup
├── docker-compose.dev.yml      # Development database only
├── frontend/
│   ├── Dockerfile
│   ├── nginx.conf
│   └── trainr/
│       ├── src/
│       │   ├── api/            # API client and endpoints
│       │   ├── components/     # Reusable components
│       │   │   └── styled/     # Styled components
│       │   ├── hooks/          # Custom React hooks
│       │   ├── pages/          # Page components
│       │   ├── styles/         # Theme and global styles
│       │   └── types/          # TypeScript types
│       └── public/
└── backend/
    ├── Dockerfile
    └── trAInr.API/
        ├── Controllers/        # API controllers
        ├── Data/               # Database context
        ├── Models/
        │   ├── Domain/         # Entity models
        │   └── DTOs/           # Data transfer objects
        └── Services/           # Business logic
```

## API Endpoints

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Programmes
- `GET /api/programmes/user/{userId}` - Get user's programmes
- `GET /api/programmes/{id}` - Get programme details
- `POST /api/programmes/user/{userId}` - Create programme
- `PUT /api/programmes/{id}` - Update programme
- `DELETE /api/programmes/{id}` - Delete programme

### Workouts
- `GET /api/workouts/days/{id}` - Get workout day
- `POST /api/workouts/weeks/{weekId}/days` - Create workout day
- `POST /api/workouts/days/{id}/complete` - Complete workout
- `POST /api/workouts/days/{workoutDayId}/exercises` - Add exercise
- `POST /api/workouts/exercises/{id}/sets` - Add set
- `POST /api/workouts/sets/{id}/complete` - Complete set

### Exercises
- `GET /api/exercises` - Get all exercises
- `GET /api/exercises/search` - Search exercises
- `GET /api/exercises/{id}` - Get exercise details

### Dashboard
- `GET /api/dashboard/user/{userId}` - Get dashboard data
- `GET /api/dashboard/programme/{programmeId}/weekly-progress` - Weekly metrics
- `GET /api/dashboard/user/{userId}/exercises` - Exercise metrics

## Data Models

### User
- Personal information (name, email, DOB)
- Fitness level (Beginner/Intermediate/Advanced/Elite)
- Primary goal (Build Muscle/Lose Weight/Improve Endurance/Increase Strength/General Fitness)
- Workout days per week

### Programme
- Name and description
- Duration (4-10 weeks)
- Collection of weeks with workout days

### WorkoutDay
- Day of week assignment
- Collection of exercises with target sets/reps/weight

### ExerciseSet
- Tracked metrics (reps, weight, duration, distance)
- Difficulty and intensity ratings
- Completion status

## Environment Variables

### Backend
| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Docker) | Development |
| `ConnectionStrings__DefaultConnection` | Database connection string | SQLite file |
| `UsePostgres` | Use PostgreSQL instead of SQLite | false |
| `AllowedOrigins__0` | CORS allowed origin | http://localhost:3000 |

### Frontend
| Variable | Description | Default |
|----------|-------------|---------|
| `REACT_APP_API_URL` | Backend API URL | http://localhost:8080/api |

## License

MIT
