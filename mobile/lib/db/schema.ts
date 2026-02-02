/**
 * SQLite Database Schema
 * Mirrors backend entities for offline storage
 */

export const createTablesSQL = `
-- Exercise definitions (cached from server)
CREATE TABLE IF NOT EXISTS exercises (
  id INTEGER PRIMARY KEY,
  name TEXT NOT NULL,
  description TEXT,
  type INTEGER NOT NULL,
  primary_muscle_group INTEGER NOT NULL,
  secondary_muscle_group INTEGER,
  instructions TEXT,
  video_url TEXT,
  is_system_exercise INTEGER DEFAULT 1,
  synced_at INTEGER
);

-- Programs
CREATE TABLE IF NOT EXISTS programs (
  id TEXT PRIMARY KEY,
  athlete_id TEXT NOT NULL,
  name TEXT NOT NULL,
  description TEXT,
  duration_weeks INTEGER NOT NULL,
  is_active INTEGER DEFAULT 0,
  is_pre_made INTEGER DEFAULT 0,
  start_date TEXT,
  end_date TEXT,
  created_at INTEGER,
  synced_at INTEGER,
  last_modified_at INTEGER,
  version INTEGER DEFAULT 1
);

-- Program weeks
CREATE TABLE IF NOT EXISTS program_weeks (
  id TEXT PRIMARY KEY,
  program_id TEXT NOT NULL,
  week_number INTEGER NOT NULL,
  week_start_date TEXT,
  notes TEXT,
  is_completed INTEGER DEFAULT 0,
  created_at INTEGER,
  synced_at INTEGER,
  last_modified_at INTEGER,
  version INTEGER DEFAULT 1,
  FOREIGN KEY (program_id) REFERENCES programs(id) ON DELETE CASCADE
);

-- Workout days
CREATE TABLE IF NOT EXISTS workout_days (
  id TEXT PRIMARY KEY,
  week_id TEXT NOT NULL,
  name TEXT NOT NULL,
  description TEXT,
  scheduled_date TEXT,
  completed_date TEXT,
  is_completed INTEGER DEFAULT 0,
  is_rest_day INTEGER DEFAULT 0,
  created_at INTEGER,
  synced_at INTEGER,
  last_modified_at INTEGER,
  version INTEGER DEFAULT 1,
  FOREIGN KEY (week_id) REFERENCES program_weeks(id) ON DELETE CASCADE
);

-- Workout exercises
CREATE TABLE IF NOT EXISTS workout_exercises (
  id TEXT PRIMARY KEY,
  workout_day_id TEXT NOT NULL,
  exercise_id INTEGER NOT NULL,
  order_index INTEGER NOT NULL,
  target_sets INTEGER,
  target_reps INTEGER,
  target_weight REAL,
  target_duration_seconds INTEGER,
  target_distance REAL,
  rest_seconds INTEGER,
  target_rpe INTEGER,
  notes TEXT,
  superset_group_id TEXT,
  superset_rest_seconds INTEGER,
  created_at INTEGER,
  synced_at INTEGER,
  last_modified_at INTEGER,
  version INTEGER DEFAULT 1,
  FOREIGN KEY (workout_day_id) REFERENCES workout_days(id) ON DELETE CASCADE,
  FOREIGN KEY (exercise_id) REFERENCES exercises(id)
);

-- Exercise sets
CREATE TABLE IF NOT EXISTS exercise_sets (
  id TEXT PRIMARY KEY,
  workout_exercise_id TEXT NOT NULL,
  set_number INTEGER NOT NULL,
  reps INTEGER,
  weight REAL,
  duration_seconds INTEGER,
  distance REAL,
  difficulty INTEGER,
  intensity INTEGER,
  is_completed INTEGER DEFAULT 0,
  set_type INTEGER DEFAULT 0,
  drop_percentage REAL,
  notes TEXT,
  completed_at INTEGER,
  created_at INTEGER,
  synced_at INTEGER,
  last_modified_at INTEGER,
  version INTEGER DEFAULT 1,
  FOREIGN KEY (workout_exercise_id) REFERENCES workout_exercises(id) ON DELETE CASCADE
);

-- Sync queue for pending changes
CREATE TABLE IF NOT EXISTS sync_queue (
  id TEXT PRIMARY KEY,
  entity_type TEXT NOT NULL,
  entity_id TEXT NOT NULL,
  operation TEXT NOT NULL,
  payload TEXT NOT NULL,
  idempotency_key TEXT NOT NULL UNIQUE,
  created_at INTEGER NOT NULL,
  synced_at INTEGER,
  retry_count INTEGER DEFAULT 0,
  last_error TEXT
);

-- Sync state tracking
CREATE TABLE IF NOT EXISTS sync_state (
  key TEXT PRIMARY KEY,
  value TEXT NOT NULL
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_programs_athlete ON programs(athlete_id);
CREATE INDEX IF NOT EXISTS idx_weeks_program ON program_weeks(program_id);
CREATE INDEX IF NOT EXISTS idx_days_week ON workout_days(week_id);
CREATE INDEX IF NOT EXISTS idx_exercises_day ON workout_exercises(workout_day_id);
CREATE INDEX IF NOT EXISTS idx_sets_exercise ON exercise_sets(workout_exercise_id);
CREATE INDEX IF NOT EXISTS idx_sync_queue_entity ON sync_queue(entity_type, entity_id);
`;
