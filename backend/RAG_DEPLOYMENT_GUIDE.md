# RAG AI Program Generation - Deployment Guide

## Prerequisites

1. PostgreSQL 14+ with pgvector extension
2. OpenAI API key configured in `appsettings.json` or environment variables
3. Existing trAInr database

## Step-by-Step Deployment

### 1. Enable pgvector Extension

Connect to your PostgreSQL database and run:

```sql
CREATE EXTENSION IF NOT EXISTS vector;
```

This only needs to be done once per database.

### 2. Run Migrations

Apply the new schema changes:

```bash
cd backend/trAInr.Infrastructure
dotnet ef database update --startup-project ../trAInr.API
```

This will create:

- Equipment, Muscles, Tags lookup tables
- ExerciseEquipment, ExerciseMuscle, ExerciseTag, ExerciseVariant join tables
- ExerciseEmbeddings table with vector index
- New columns on ExerciseDefinitions table

### 3. Seed Lookup Tables

You'll need to populate the Equipment, Muscles, and Tags tables. You have two options:

#### Option A: Create a Seed Endpoint (Recommended for Development)

Create an admin endpoint that calls the seed data classes:

```csharp
[HttpPost("admin/seed-lookup-tables")]
public async Task<IActionResult> SeedLookupTables()
{
    var equipment = EquipmentSeed.GetEquipmentSeedData();
    var muscles = MuscleSeed.GetMuscleSeedData();
    var tags = TagSeed.GetTagSeedData();

    await _context.Equipment.AddRangeAsync(equipment);
    await _context.Muscles.AddRangeAsync(muscles);
    await _context.Tags.AddRangeAsync(tags);

    await _context.SaveChangesAsync();

    return Ok(new {
        EquipmentCount = equipment.Count,
        MusclesCount = muscles.Count,
        TagsCount = tags.Count
    });
}
```

Then call: `POST /api/admin/seed-lookup-tables`

#### Option B: SQL Insert Script

A ready-to-run SQL seed script is provided for testing the RAG feature with all new fields populated:

- **`backend/scripts/rag_seed_data.sql`** – Seeds Equipment, Muscles, Tags, ~58 ExerciseDefinitions (~5 per muscle group), and the join tables (ExerciseEquipments, ExerciseMuscles, ExerciseTags). Covers all new columns: SpinalLoad, SetupComplexity, TrackingMode, LoadType, MovementPattern, LevelOfDifficulty, DefaultRepMin/Max, DefaultRestMin/Max, TimePerSetEstimateSec, IsBodyweight, IsUnilateral, RequiresOverheadPosition, Aliases, ShortCue, and EquipmentRequirements JSON.

Run after migrations (e.g. from repo root: `psql $DATABASE_URL -f backend/scripts/rag_seed_data.sql`). For a clean test, use an empty DB or truncate the RAG-related tables first if you already ran the C# lookup seed.

Alternatively, export your own seed data to SQL and run it directly on the database.

### 4. Map Existing Exercise Data

Your existing exercises have:

- `EquipmentRequirements` (JSON array)
- `PrimaryMuscleGroup` / `SecondaryMuscleGroup` (enums)

You need to:

1. **Map equipment to new normalized tables:**

   - Parse JSON `EquipmentRequirements`
   - Look up `Equipment.Id` by name
   - Insert into `ExerciseEquipments` join table

2. **Map muscles to new normalized tables:**

   - Map `PrimaryMuscleGroup` enum to muscle names
   - Look up `Muscle.Id` by name
   - Insert into `ExerciseMuscles` with `Role = Primary`

3. **Set default values for new columns:**
   - `SpinalLoad` = Low (default)
   - `SetupComplexity` = Low (default)
   - `TrackingMode` = RepsWeight (default)
   - `LoadType` = ExternalLoad (default)
   - etc.

**Migration Script Example:**

```csharp
[HttpPost("admin/migrate-exercise-data")]
public async Task<IActionResult> MigrateExerciseData()
{
    var exercises = await _context.ExerciseDefinitions.ToListAsync();

    foreach (var exercise in exercises)
    {
        // Map equipment from JSON to join table
        foreach (var equipmentReq in exercise.EquipmentRequirements)
        {
            var equipment = await _context.Equipment
                .FirstOrDefaultAsync(e => e.Name == equipmentReq.Name);

            if (equipment != null)
            {
                var exerciseEquipment = ExerciseEquipment.Create(
                    exercise.Id,
                    equipment.Id);
                await _context.ExerciseEquipments.AddAsync(exerciseEquipment);
            }
        }

        // Map primary muscle
        var primaryMuscle = await _context.Muscles
            .FirstOrDefaultAsync(m => m.Group == exercise.PrimaryMuscleGroup.ToString());

        if (primaryMuscle != null)
        {
            var exerciseMuscle = ExerciseMuscle.Create(
                exercise.Id,
                primaryMuscle.Id,
                MuscleRole.Primary);
            await _context.ExerciseMuscles.AddAsync(exerciseMuscle);
        }

        // Similar for secondary muscle
    }

    await _context.SaveChangesAsync();
    return Ok();
}
```

### 5. Generate Embeddings (Optional but Recommended)

Generate embeddings for all exercises to enable semantic search:

**Create an admin endpoint:**

```csharp
[HttpPost("admin/generate-embeddings")]
public async Task<IActionResult> GenerateEmbeddings()
{
    await _embeddingService.GenerateExerciseEmbeddingsAsync();
    return Ok(new { Message = "Embeddings generation started" });
}
```

**Notes:**

- This is a long-running operation (~5-10 minutes for 1000 exercises)
- OpenAI has rate limits - the service includes delays
- Cost: ~$0.013 per 1000 exercises (text-embedding-3-small)
- Only needed once, or when exercises change

### 6. Test the System

Make a program generation request with the new fields:

```bash
POST /api/program-generator/generate

{
  "programName": "Strength Builder",
  "experienceLevel": 2,
  "durationWeeks": 8,
  "workoutDayNames": ["Monday", "Wednesday", "Friday"],
  "description": "Build strength with compound lifts",
  "availableEquipment": ["Barbell", "Dumbbell", "Squat Rack", "Flat Bench"],
  "contraindications": ["Contra:overhead_required"]
}
```

The system will:

1. Retrieve ~80-120 relevant exercises (filtered by equipment)
2. Send only those to the LLM
3. Generate and validate the program

### 7. Monitor Performance

Check logs for:

- Retrieval service execution time (~50-100ms expected)
- Number of candidate exercises retrieved
- LLM token usage (should be 60-75% lower)

## Rollback Plan

If you need to rollback:

```bash
# Remove the last two migrations
dotnet ef migrations remove --startup-project ../trAInr.API
dotnet ef migrations remove --startup-project ../trAInr.API

# Or rollback to a specific migration
dotnet ef database update PreviousMigrationName --startup-project ../trAInr.API
```

The old system will still work because:

- Old columns (`EquipmentRequirements` JSON) are still present
- AI service can fall back to old behavior if needed

## Production Checklist

- [ ] pgvector extension enabled on PostgreSQL
- [ ] Migrations applied successfully
- [ ] Lookup tables seeded (Equipment, Muscles, Tags)
- [ ] Existing exercise data migrated to new schema
- [ ] Embeddings generated (optional but recommended)
- [ ] New request fields added to frontend
- [ ] Performance monitoring in place
- [ ] Rollback plan tested
- [ ] OpenAI API key configured
- [ ] Rate limits reviewed (embedding generation)

## Cost Estimates

**One-time costs:**

- Embedding generation: ~$0.013 per 1000 exercises
- For 5,000 exercises: ~$0.065

**Ongoing costs:**

- LLM program generation: 60-75% reduction
- If previously $0.10 per program: Now ~$0.03 per program
- **Savings: ~$0.07 per program**

## Performance Expectations

**Retrieval Service:**

- ~50-100ms for candidate retrieval
- Returns 80-120 exercises (vs 1000+)

**LLM Call:**

- ~2,000-3,000 tokens per request (vs 8,000-12,000)
- 20-30% faster response times
- More consistent quality

**Database:**

- New indexes on ExerciseDefinitions
- HNSW vector index for similarity search
- Minimal impact on read queries

## Troubleshooting

### "pgvector extension not found"

```bash
# On Ubuntu/Debian
sudo apt install postgresql-14-pgvector

# On macOS with Homebrew
brew install pgvector

# Then in psql:
CREATE EXTENSION vector;
```

### "No exercises returned from retrieval service"

- Check that join tables are populated
- Verify equipment names match exactly (case-sensitive)
- Check that `ExerciseDefinitions.IsSystemExercise = true`

### "Embedding generation fails"

- Verify OpenAI API key is set
- Check rate limits (3,500 requests/minute for text-embedding-3-small)
- Ensure exercises have sufficient data (name, description, muscles, equipment)

### "Migration fails"

- Ensure pgvector extension is installed first
- Check that user has CREATE EXTENSION privilege
- Verify PostgreSQL version is 14+

## Support

For issues or questions:

1. Check build logs: `dotnet build`
2. Check migration status: `dotnet ef migrations list`
3. Review `RAG_IMPLEMENTATION_SUMMARY.md` for architecture details
