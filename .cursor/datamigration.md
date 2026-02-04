Pulling _every_ exercise definition on every request and shoving it into a prompt is the “works in a demo, dies in production” approach. It burns tokens, latency, and money—and it **still** won’t reliably give better programs because the model is overwhelmed with irrelevant options.

The optimal strategy for your use case is:

## Use RAG-style retrieval: “small, relevant subset per request”

**Store embeddings for exercises** and retrieve only the most relevant exercises for _this user + this goal + this constraints set_, then generate the plan using that subset.

### Why this is the right default

- **Cost scales with relevance, not catalog size.** Adding 5,000 more exercises doesn’t change token usage if you still only send 50–150.
- **Quality improves.** The model sees a curated set instead of noise.
- **Latency stays sane.** Vector search is fast; LLM calls become smaller.

---

# The concrete architecture

## 1) Split the AI workflow into 2 steps (don’t do it in one prompt)

### Step A — “Selector”

Goal: identify the _k_ exercises that fit the request.
Implementation: **vector search + filters** (not the LLM).

Inputs:

- user goal (hypertrophy/strength/fat loss/etc)
- equipment available
- constraints (injuries, time per session, days/week)
- preferences (barbell focus, minimal machines, etc)

Output:

- exercise IDs (e.g., 80 candidates)

### Step B — “Planner”

Goal: build the full program using only the candidate set.
Implementation: LLM call with a **tight schema** (JSON) and **only the necessary exercise fields**.

---

## 2) Build a “thin exercise representation” specifically for prompting

Your DB record can be rich; your prompt should be lean.

For the LLM, you typically only need:

- `id`, `name`
- `movement_pattern` (squat/hinge/push/pull/carry)
- `primary_muscles`, `secondary_muscles`
- `equipment`
- `level`
- `contraindications/tags` (e.g., “knee-friendly”, “low back load”)
- `variants_of` (optional)
- 1-line coaching cue (optional)

**Not**: long descriptions, history, media links, 12 synonyms, etc.

This alone can cut prompt payload by 60–90%.

---

## 3) Retrieval should be hybrid (vector + filters + rules)

Vector search alone will return dumb stuff unless you constrain it.

Use:

- **Hard filters**: equipment, excluded movements, injury flags
- **Soft ranking**: embedding similarity on “goal + constraints”
- **Diversity control**: ensure coverage across movement patterns (don’t return 30 curls)

A practical pattern:

1. Retrieve top 200 by embedding similarity.
2. Filter by equipment/injuries.
3. Re-rank with a scoring function:
   - goal match (tag-based)
   - muscle coverage gaps
   - novelty/variety vs last program

4. Take top 80–120.

---

## 4) Cache aggressively where it matters

You’re likely regenerating similar requests.

Cache layers:

- **Exercise embeddings** (precomputed, updated on exercise change)
- **Retrieval results** keyed by a normalized “request signature”
  - e.g., `goal=strength|days=4|equipment=barbell,dumbbell|injury=none`
  - TTL 24h or until exercise catalog changes

- **LLM outputs** optionally cached for identical signatures (short TTL), especially for “preview program” flows

Also: keep the exercise catalog version hash, and invalidate caches when it changes.

---

## 5) Force structured output and validate it

Don’t let the model “invent” exercises.

- Use JSON schema output: exercise IDs must exist in candidate set.
- Server validates:
  - IDs exist
  - no contraindicated tags
  - volume bounds, weekly sets, etc.

- If validation fails:
  - auto-repair prompt: “fix program to satisfy constraints, keep IDs within allowed set”
  - or rerun with tighter constraints

This saves support tickets and prevents hallucinated garbage.

---

# What this looks like in practice (numbers)

Instead of sending 3,000 exercises:

- You send **80–150** slim entries.
- That typically fits comfortably even with a detailed instruction + schema.
- Cost and latency become predictable and controllable.

---

# Hard truths / blind spots you need to address

- **“Comprehensive dictionary” doesn’t mean “send everything to the model.”** It means _the system can choose from everything_.
- **If you don’t tag your exercises well, retrieval will be bad.** Embeddings help, but a minimal taxonomy is non-negotiable (movement pattern, equipment, muscles, level).
- **One-shot generation is fragile.** Two-step (retrieve → plan) is how you make this reliable.

---

# Minimal implementation plan (fastest path to production)

1. Create `ExercisePromptView` table/materialized view with only the lean fields.
2. Generate embeddings for each exercise (background job).
3. Add vector search (pgvector if you’re on Postgres).
4. Implement retrieval endpoint:
   - inputs: user request + constraints
   - outputs: list of exercise IDs + lean fields

5. Implement planner call with strict JSON schema and server-side validation.
6. Add caching + catalog version invalidation.

---

If you want the genuinely optimal version after this: add **program-aware retrieval** (avoid repeating last mesocycle, enforce progression), and a **small local scoring model** or heuristic re-ranker. But don’t jump there until basic retrieval + schema output is stable.

If your exercise schema is “just name + description”, your AI feature will always be expensive and sloppy. You need a schema that supports **(1) deterministic filtering**, **(2) good retrieval/ranking**, and **(3) safe program construction**. Anything else is noise.

Below is a battle-tested schema approach: **core normalized tables + a lean “prompt view”** for AI.

---

## Core principles (don’t ignore these)

- **Hard filters must be first-class fields** (equipment, movement pattern, contraindications). If they’re buried in free text, retrieval and safety will suck.
- **Muscles and equipment are many-to-many**. Don’t denormalize into CSV strings unless you enjoy bugs and impossible analytics.
- **AI prompt payload should be a projection**, not your full definition record.

---

## Required fields (MVP that actually works)

### `exercises` (main table)

- `id` (UUID/int)
- `name` (string) — unique-ish, indexed
- `slug` (string) — stable identifier for URLs / imports
- `status` (enum: active, deprecated) — lets you retire exercises safely
- `movement_pattern` (enum) — squat, hinge, horizontal_push, vertical_push, horizontal_pull, vertical_pull, carry, core, isolation, locomotion, olympic, etc.
- `exercise_type` (enum) — compound, accessory, isolation, conditioning, mobility, skill
- `unilateral` (bool)
- `is_bodyweight` (bool)
- `difficulty` (enum: beginner/intermediate/advanced)
- `mechanics` (enum: bilateral/unilateral/isometric/plyometric/eccentric_emphasis etc. optional but useful)
- `created_at`, `updated_at`

### `exercise_equipment` (join)

- `exercise_id`
- `equipment_id`
  Indexes: `(equipment_id)`, `(exercise_id)`

### `equipment` (lookup)

- `id`
- `name` (barbell, dumbbell, cable, machine, kettlebell, bands, bodyweight, trap_bar, smith_machine…)
- `category` (free_weight, machine, cable, bodyweight, cardio)
- `is_portable` (bool)

### `exercise_muscles` (join, weighted)

- `exercise_id`
- `muscle_id`
- `role` (enum: primary, secondary, stabilizer)
- `contribution` (smallint 1–100) — optional but _very_ helpful for balancing programs
  Indexes: `(muscle_id, role)`, `(exercise_id)`

### `muscles` (lookup)

- `id`
- `name` (pec_major, latissimus_dorsi, quads, glutes, hamstrings…)
- `group` (chest, back, legs, shoulders, arms, core)

### `exercise_tags` (join)

- `exercise_id`
- `tag_id`

### `tags` (lookup)

Use tags for retrieval and safety. Minimum tag sets you need:

- **goal**: strength, hypertrophy, power, endurance, fat_loss
- **joint_friendly**: knee_friendly, shoulder_friendly, low_back_friendly
- **risk/contra**: wrist_stress, high_spine_load, overhead_required
- **context**: home_gym, commercial_gym, minimal_setup
- **sport**: boxing, running, etc. (optional)

### `exercise_variants` (graph)

- `exercise_id`
- `variant_of_exercise_id`
- `variant_type` (regression, progression, alternative, grip_change, stance_change)
  This is how you swap exercises without the model inventing replacements.

---

## Strongly recommended fields (make your retrieval + planning much better)

### Standardized “constraints” fields

- `requires_overhead_position` (bool)
- `requires_knee_flexion_deep` (bool)
- `requires_hip_hinge_loaded` (bool)
- `spinal_load` (enum: none, low, moderate, high)
- `skill_requirement` (enum: low/med/high) — olympic lifts, gymnastics, etc.
- `setup_complexity` (enum: low/med/high) — helps avoid busy-gym bottlenecks
- `time_per_set_estimate_sec` (int) — helps session time planning

### Execution / programming fields

- `default_rep_range_min`, `default_rep_range_max`
- `default_rest_sec_min`, `default_rest_sec_max`
- `load_type` (enum: external_load, bodyweight, timed, distance)
- `tracking_mode` (enum: reps, reps_weight, time, distance, time_distance)
  This prevents garbage prescriptions (“5x5 for a 2km run”).

### Search and text fields (used for embeddings + UX)

- `aliases` (array) — “RDL”, “Romanian Deadlift”
- `short_cue` (string, 120 chars) — one coaching cue
- `description` (text) — long form, not sent to AI usually
- `embedding` (vector) — stored separately or in an embeddings table

---

## What the AI should actually see (Lean Prompt Projection)

Create a materialized view or API DTO like:

**`ExercisePromptDTO`**

- `id`
- `name`
- `movement_pattern`
- `exercise_type`
- `equipment` (array of strings)
- `primary_muscles` (array)
- `secondary_muscles` (array)
- `difficulty`
- `spinal_load`
- `setup_complexity`
- `tags` (array)
- `tracking_mode`
- `default_rep_range` (e.g., “6–10”)

That’s it. No essays. No giant technique breakdown.

---

## Indexing for performance (don’t wing this)

- `exercises(name)` btree + trigram index for fuzzy search
- `exercises(movement_pattern, difficulty, status)` composite
- `exercise_equipment(equipment_id, exercise_id)`
- `exercise_muscles(muscle_id, role, exercise_id)`
- `exercise_tags(tag_id, exercise_id)`
- `embeddings`:
  - vector index (HNSW/IVFFlat depending on your setup)
  - and a `catalog_version` column for invalidation

---

## Data quality rules you should enforce (or the system rots)

- Every exercise must have:
  - ≥1 equipment (or bodyweight)
  - ≥1 primary muscle
  - exactly 1 movement pattern
  - tracking mode consistent with load type

- “Deprecated” exercises remain referenceable for history but excluded from retrieval.

---

## A blunt recommendation

Start with the **required set + spinal_load + setup_complexity + tracking_mode**. Those four extras do more for real-world program quality (and reducing stupid outputs) than 30 fluffy metadata fields.

If you want, I can give you:

- a concrete Postgres schema (DDL),
- an `ExercisePromptView` materialized view,
- and a retrieval query pattern (filters + vector search + diversity).

Alright. Here’s a **practical Postgres schema (DDL)** + a **materialized “prompt view”** + a **retrieval query pattern** that scales and keeps the LLM payload lean.

I’m assuming:

- Postgres 14+
- You’ll use **pgvector** for embeddings
- UUID primary keys (swap to BIGINT if you prefer)

---

## 1) Postgres DDL (core tables + joins)

```sql
-- Enable extensions you'll actually use
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE EXTENSION IF NOT EXISTS vector;  -- pgvector

-- =========================
-- Enums (keep them tight)
-- =========================

DO $$ BEGIN
  CREATE TYPE exercise_status AS ENUM ('active', 'deprecated');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE movement_pattern AS ENUM (
    'squat', 'hinge',
    'horizontal_push', 'vertical_push',
    'horizontal_pull', 'vertical_pull',
    'carry', 'core',
    'isolation', 'locomotion',
    'olympic', 'mobility', 'plyometric'
  );
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE exercise_type AS ENUM ('compound','accessory','isolation','conditioning','mobility','skill');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE difficulty_level AS ENUM ('beginner','intermediate','advanced');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE muscle_role AS ENUM ('primary','secondary','stabilizer');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE spinal_load_level AS ENUM ('none','low','moderate','high');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE setup_complexity_level AS ENUM ('low','medium','high');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE load_type AS ENUM ('external_load','bodyweight','timed','distance','time_distance');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE tracking_mode AS ENUM ('reps','reps_weight','time','distance','time_distance');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

DO $$ BEGIN
  CREATE TYPE variant_type AS ENUM ('regression','progression','alternative','grip_change','stance_change','implement_change');
EXCEPTION WHEN duplicate_object THEN NULL; END $$;

-- =========================
-- Lookup tables
-- =========================

CREATE TABLE IF NOT EXISTS equipment (
  id            uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  name          text NOT NULL UNIQUE,         -- 'barbell', 'dumbbell', 'cable', etc
  category      text NOT NULL,                -- keep as text unless you really need an enum
  is_portable   boolean NOT NULL DEFAULT false
);

CREATE TABLE IF NOT EXISTS muscles (
  id      uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  name    text NOT NULL UNIQUE,               -- 'quads', 'glutes', 'latissimus_dorsi'
  "group" text NOT NULL                       -- 'legs', 'back', etc
);

CREATE TABLE IF NOT EXISTS tags (
  id        uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  namespace text NOT NULL,                    -- 'goal', 'joint_friendly', 'contra', 'context'
  name      text NOT NULL,                    -- 'hypertrophy', 'knee_friendly', etc
  UNIQUE(namespace, name)
);

-- =========================
-- Exercises (core)
-- =========================

CREATE TABLE IF NOT EXISTS exercises (
  id                         uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
  name                       text NOT NULL,
  slug                       text NOT NULL UNIQUE,
  status                     exercise_status NOT NULL DEFAULT 'active',

  movement_pattern           movement_pattern NOT NULL,
  exercise_type              exercise_type NOT NULL,

  unilateral                 boolean NOT NULL DEFAULT false,
  is_bodyweight              boolean NOT NULL DEFAULT false,
  difficulty                 difficulty_level NOT NULL DEFAULT 'beginner',

  -- Safety + planning knobs that massively improve output quality
  spinal_load                spinal_load_level NOT NULL DEFAULT 'low',
  setup_complexity           setup_complexity_level NOT NULL DEFAULT 'low',
  requires_overhead_position boolean NOT NULL DEFAULT false,

  load_type                  load_type NOT NULL DEFAULT 'external_load',
  tracking_mode              tracking_mode NOT NULL DEFAULT 'reps_weight',

  default_rep_min            int NULL CHECK (default_rep_min IS NULL OR default_rep_min > 0),
  default_rep_max            int NULL CHECK (default_rep_max IS NULL OR default_rep_max >= default_rep_min),
  default_rest_min_sec       int NULL CHECK (default_rest_min_sec IS NULL OR default_rest_min_sec >= 0),
  default_rest_max_sec       int NULL CHECK (default_rest_max_sec IS NULL OR default_rest_max_sec >= default_rest_min_sec),

  time_per_set_estimate_sec  int NULL CHECK (time_per_set_estimate_sec IS NULL OR time_per_set_estimate_sec > 0),

  aliases                    text[] NOT NULL DEFAULT '{}',
  short_cue                  text NULL,         -- one-liner cue; keep short
  description                text NULL,         -- long form; not sent to AI usually

  created_at                 timestamptz NOT NULL DEFAULT now(),
  updated_at                 timestamptz NOT NULL DEFAULT now()
);

-- Helpful for UI search + admin tools
CREATE INDEX IF NOT EXISTS idx_exercises_name_trgm ON exercises USING gin (name gin_trgm_ops);
CREATE INDEX IF NOT EXISTS idx_exercises_slug ON exercises (slug);
CREATE INDEX IF NOT EXISTS idx_exercises_core_filter ON exercises (status, movement_pattern, difficulty, spinal_load, setup_complexity);

-- =========================
-- Join tables
-- =========================

CREATE TABLE IF NOT EXISTS exercise_equipment (
  exercise_id  uuid NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
  equipment_id uuid NOT NULL REFERENCES equipment(id) ON DELETE RESTRICT,
  PRIMARY KEY (exercise_id, equipment_id)
);
CREATE INDEX IF NOT EXISTS idx_exeq_equipment ON exercise_equipment (equipment_id, exercise_id);

CREATE TABLE IF NOT EXISTS exercise_muscles (
  exercise_id   uuid NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
  muscle_id     uuid NOT NULL REFERENCES muscles(id) ON DELETE RESTRICT,
  role          muscle_role NOT NULL,
  contribution  smallint NULL CHECK (contribution IS NULL OR (contribution BETWEEN 1 AND 100)),
  PRIMARY KEY (exercise_id, muscle_id, role)
);
CREATE INDEX IF NOT EXISTS idx_exmus_muscle_role ON exercise_muscles (muscle_id, role, exercise_id);

CREATE TABLE IF NOT EXISTS exercise_tags (
  exercise_id uuid NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
  tag_id      uuid NOT NULL REFERENCES tags(id) ON DELETE RESTRICT,
  PRIMARY KEY (exercise_id, tag_id)
);
CREATE INDEX IF NOT EXISTS idx_extags_tag ON exercise_tags (tag_id, exercise_id);

-- Variants graph (for swaps/progressions)
CREATE TABLE IF NOT EXISTS exercise_variants (
  exercise_id           uuid NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
  variant_of_exercise_id uuid NOT NULL REFERENCES exercises(id) ON DELETE CASCADE,
  variant_type          variant_type NOT NULL,
  PRIMARY KEY (exercise_id, variant_of_exercise_id, variant_type),
  CHECK (exercise_id <> variant_of_exercise_id)
);

-- =========================
-- Embeddings (separate table = easier versioning)
-- =========================

-- Choose your embedding dimension (example: 1536, 3072, etc). Must match your model.
-- Replace 1536 with your actual dimension.
CREATE TABLE IF NOT EXISTS exercise_embeddings (
  exercise_id      uuid PRIMARY KEY REFERENCES exercises(id) ON DELETE CASCADE,
  embedding        vector(1536) NOT NULL,
  embedding_model  text NOT NULL,
  catalog_version  bigint NOT NULL DEFAULT 1,   -- bump when you change how you build embeddings
  updated_at       timestamptz NOT NULL DEFAULT now()
);

-- Vector index (pick ONE; HNSW is great if supported by your pgvector version)
-- HNSW:
CREATE INDEX IF NOT EXISTS idx_exercise_embeddings_hnsw
  ON exercise_embeddings USING hnsw (embedding vector_cosine_ops);

-- IVFFlat alternative:
-- CREATE INDEX idx_exercise_embeddings_ivf
--   ON exercise_embeddings USING ivfflat (embedding vector_cosine_ops) WITH (lists = 100);
```

---

## 2) Materialized prompt view (lean projection for LLM)

This is what you send to AI. Nothing else.

```sql
CREATE MATERIALIZED VIEW IF NOT EXISTS mv_exercise_prompt AS
SELECT
  e.id,
  e.name,
  e.slug,
  e.movement_pattern,
  e.exercise_type,
  e.unilateral,
  e.is_bodyweight,
  e.difficulty,
  e.spinal_load,
  e.setup_complexity,
  e.requires_overhead_position,
  e.load_type,
  e.tracking_mode,
  e.default_rep_min,
  e.default_rep_max,
  e.default_rest_min_sec,
  e.default_rest_max_sec,
  e.aliases,
  e.short_cue,

  -- equipment list
  COALESCE((
    SELECT array_agg(eq.name ORDER BY eq.name)
    FROM exercise_equipment ee
    JOIN equipment eq ON eq.id = ee.equipment_id
    WHERE ee.exercise_id = e.id
  ), '{}') AS equipment,

  -- muscles, separated by role
  COALESCE((
    SELECT array_agg(m.name ORDER BY m.name)
    FROM exercise_muscles em
    JOIN muscles m ON m.id = em.muscle_id
    WHERE em.exercise_id = e.id AND em.role = 'primary'
  ), '{}') AS primary_muscles,

  COALESCE((
    SELECT array_agg(m.name ORDER BY m.name)
    FROM exercise_muscles em
    JOIN muscles m ON m.id = em.muscle_id
    WHERE em.exercise_id = e.id AND em.role = 'secondary'
  ), '{}') AS secondary_muscles,

  -- tags as namespace:name strings (compact + useful)
  COALESCE((
    SELECT array_agg(t.namespace || ':' || t.name ORDER BY t.namespace, t.name)
    FROM exercise_tags et
    JOIN tags t ON t.id = et.tag_id
    WHERE et.exercise_id = e.id
  ), '{}') AS tags

FROM exercises e
WHERE e.status = 'active';

-- Fast lookup for candidate exercises by common filters
CREATE INDEX IF NOT EXISTS idx_mv_prompt_core
  ON mv_exercise_prompt (movement_pattern, difficulty, spinal_load, setup_complexity, tracking_mode);

-- Refresh strategy:
-- - refresh on a schedule, or
-- - refresh on catalog updates
-- Use CONCURRENTLY if you add a unique index:
CREATE UNIQUE INDEX IF NOT EXISTS uq_mv_exercise_prompt_id ON mv_exercise_prompt (id);

-- Then you can:
-- REFRESH MATERIALIZED VIEW CONCURRENTLY mv_exercise_prompt;
```

**Why MV?** Because it keeps retrieval _fast_ and your API doesn’t do 6 joins per request. If your catalog changes frequently, use a normal VIEW instead and accept the join cost (or cache at API layer).

---

## 3) Retrieval query pattern (hybrid: filters + vector + diversity)

You want:

- **Hard filtering** (equipment / contraindications / status already handled)
- **Vector similarity** for relevance
- **Diversity** across movement patterns (optional but highly recommended)

### Inputs you’ll pass from API:

- `:query_embedding` = vector(1536)
- `:equipment_whitelist` = text[] (e.g., `{barbell,dumbbell,bodyweight}`)
- `:excludeTZ` tags to avoid or require
- `:k` = number of candidates (e.g., 120)

### Step 1: Hard filter + vector rank

```sql
WITH filtered AS (
  SELECT
    p.*,
    1 - (emb.embedding <=> :query_embedding) AS similarity   -- cosine similarity if vector_cosine_ops
  FROM mv_exercise_prompt p
  JOIN exercise_embeddings emb ON emb.exercise_id = p.id
  WHERE
    -- equipment filter: exercise equipment subset of available equipment
    -- (i.e. every required equipment item is available)
    (
      SELECT bool_and(eq = ANY(:equipment_whitelist))
      FROM unnest(p.equipment) AS eq
    ) = true

    -- Example: exclude contraindications via tags (namespace:contra)
    AND NOT (p.tags && :excluded_tags)    -- array overlap operator

    -- Example: optionally require some tags (like goal tags)
    AND (:required_tags IS NULL OR p.tags @> :required_tags)

    -- optional difficulty cap
    AND (p.difficulty = ANY(:difficulty_whitelist))
)
SELECT *
FROM filtered
ORDER BY similarity DESC
LIMIT :k;
```

This gives you the **top K** relevant exercises _that the user can actually do_.

### Step 2 (optional but smart): enforce diversity

If you don’t, you’ll often get a pile of near-duplicates (e.g., 15 bench variations).

This trick gives you “top N per movement pattern” then takes the best overall.

```sql
WITH ranked AS (
  SELECT
    p.*,
    1 - (emb.embedding <=> :query_embedding) AS similarity,
    row_number() OVER (
      PARTITION BY p.movement_pattern
      ORDER BY 1 - (emb.embedding <=> :query_embedding) DESC
    ) AS rn
  FROM mv_exercise_prompt p
  JOIN exercise_embeddings emb ON emb.exercise_id = p.id
  WHERE
    (
      SELECT bool_and(eq = ANY(:equipment_whitelist))
      FROM unnest(p.equipment) AS eq
    ) = true
    AND NOT (p.tags && :excluded_tags)
    AND (:required_tags IS NULL OR p.tags @> :required_tags)
)
SELECT *
FROM ranked
WHERE rn <= :per_pattern   -- e.g., 20
ORDER BY similarity DESC
LIMIT :k;                 -- e.g., 120
```

That’s your **candidate set** for the LLM.

---

## 4) The exact prompt payload (what you actually send)

Don’t send full rows. You send a minimal JSON array of DTOs:

- `id`, `name`
- `movement_pattern`, `exercise_type`
- `equipment`, `primary_muscles`, `secondary_muscles`
- `difficulty`, `spinal_load`, `setup_complexity`
- `tracking_mode`, `default_rep_min/max`
- `tags`

That’s enough for the model to build a program and for you to validate.

---

## 5) Non-negotiable validation rules (server-side)

When the LLM returns a program:

- Every `exerciseId` must be in the candidate set you provided
- Tracking mode must match prescription (don’t allow weight for “time”)
- Contra tags must not be violated
- Equipment must be available
- Optional: enforce weekly set ranges by muscle group using `exercise_muscles` contribution

---

If you tell me your embedding dimension and whether you’re using pgvector HNSW or IVFFlat, I’ll adjust the index + similarity operators to match your exact setup.
