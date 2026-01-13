using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using trAInr.Domain.Aggregates;
using trAInr.Domain.Entities;
using trAInr.Domain.Events;
using trAInr.Domain.ValueObjects;

namespace trAInr.Infrastructure.Data;

/// <summary>
///     Entity Framework Core database context for the trAInr application
/// </summary>
public class TrainrDbContext(DbContextOptions<TrainrDbContext> options) : DbContext(options)
{
    private static readonly ValueComparer<List<string>> StringListComparer =
        new(
            (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
            v => v != null ? v.GetHashCode() : 0,
            v => v == null ? new List<string>() : v.ToList());

    private static readonly ValueComparer<List<EquipmentRequirement>> EquipmentRequirementListComparer =
        new(
            (a, b) => (a ?? new List<EquipmentRequirement>()).SequenceEqual(b ?? new List<EquipmentRequirement>()),
            v => v != null ? v.GetHashCode() : 0,
            v => v == null ? new List<EquipmentRequirement>() : v.ToList());


    // DDD Aggregates
    public DbSet<Athlete> Athletes => Set<Athlete>();
    public DbSet<ExerciseDefinition> ExerciseDefinitions => Set<ExerciseDefinition>();
    public DbSet<AssignedProgram> AssignedPrograms => Set<AssignedProgram>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

    // Entities
    public DbSet<ProgrammeWeek> ProgrammeWeeks => Set<ProgrammeWeek>();
    public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
    public DbSet<Exercise> Exercises => Set<Exercise>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Explicitly exclude DomainEvent from the model (it's abstract and handled separately)
        modelBuilder.Ignore<DomainEvent>();

        // DDD Aggregate configurations
        ConfigureAthlete(modelBuilder);
        ConfigureExerciseDefinition(modelBuilder);
        ConfigureAssignedProgram(modelBuilder);
        ConfigureWorkoutSession(modelBuilder);
        ConfigureProgrammeWeek(modelBuilder);
        ConfigureWorkoutDay(modelBuilder);
        ConfigureWorkoutExercise(modelBuilder);
        ConfigureExerciseSet(modelBuilder);
        ConfigureExercise(modelBuilder);

        SeedExercises(modelBuilder);
    }

    private static void ConfigureAthlete(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Athlete>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.ReadinessScore).HasPrecision(5, 2);
            entity.Property(e => e.ReadinessNotes).HasMaxLength(1000);

            // Configure value objects as JSON column (simpler for record structs)
            entity.Property<List<EquipmentRequirement>>("_equipmentPreferences")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<EquipmentRequirement>>(v, (JsonSerializerOptions?)null) ??
                         new List<EquipmentRequirement>())
                .HasColumnName("EquipmentPreferences");
            entity.Property<List<EquipmentRequirement>>("_equipmentPreferences").Metadata
                .SetValueComparer(EquipmentRequirementListComparer);

            // Configure constraints - using backing field
            entity.Property<List<string>>("_constraints")
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
            entity.Property<List<string>>("_constraints").Metadata.SetValueComparer(StringListComparer);
        });
    }

    private static void ConfigureExerciseDefinition(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Instructions).HasMaxLength(2000);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.PrimaryMuscleGroup);

            // Configure value objects as JSON column (simpler for record structs)
            entity.Property<List<EquipmentRequirement>>("_equipmentRequirements")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<EquipmentRequirement>>(v, (JsonSerializerOptions?)null) ??
                         new List<EquipmentRequirement>())
                .HasColumnName("EquipmentRequirements");
            entity.Property<List<EquipmentRequirement>>("_equipmentRequirements").Metadata
                .SetValueComparer(EquipmentRequirementListComparer);
        });
    }

    private static void ConfigureAssignedProgram(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignedProgram>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.AthleteId);
            entity.HasIndex(e => e.IsActive);
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.DurationWeeks).IsRequired();
            entity.Property(e => e.ProgramTemplateId).IsRequired();

            // Configure Weeks collection - EF Core will use the backing field automatically
            entity.HasMany(e => e.Weeks)
                .WithOne(w => w.AssignedProgram)
                .HasForeignKey(w => w.AssignedProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure to use backing field for the Weeks property
            entity.Metadata.FindNavigation(nameof(AssignedProgram.Weeks))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Ignore domain events (they're handled separately)
            entity.Ignore(e => e.DomainEvents);
        });
    }

    private static void ConfigureWorkoutSession(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkoutSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.AthleteId);
            entity.HasIndex(e => e.ProgrammeWeekId);
            entity.HasIndex(e => e.ScheduledDate);
            entity.Property(e => e.ScheduledDate).HasColumnType("date");
            entity.Property(e => e.CompletedDate).HasColumnType("timestamp with time zone");

            // Configure owned entities for ExerciseInstance
            entity.OwnsMany(e => e.ExerciseInstances, ei =>
            {
                ei.ToTable("ExerciseInstances");
                ei.WithOwner().HasForeignKey("WorkoutSessionId");
                ei.HasKey(e => e.Id);
                ei.Property(e => e.ExerciseName).HasMaxLength(200).IsRequired();
                ei.Property(e => e.Notes).HasMaxLength(500);
                ei.Property(e => e.TargetWeight).HasPrecision(10, 2);
                ei.Property(e => e.TargetDistance).HasPrecision(10, 2);

                // Configure owned entities for CompletedSet
                ei.OwnsMany(e => e.CompletedSets, cs =>
                {
                    cs.ToTable("CompletedSets");
                    cs.WithOwner().HasForeignKey("ExerciseInstanceId");
                    cs.HasKey(e => e.Id);
                    cs.Property(e => e.Weight).HasPrecision(10, 2);
                    cs.Property(e => e.Distance).HasPrecision(10, 2);
                    cs.Property(e => e.Notes).HasMaxLength(500);
                    cs.Property(e => e.CompletedAt).HasColumnType("timestamp with time zone");
                });
            });

            // Ignore domain events
            entity.Ignore(e => e.DomainEvents);
        });
    }

    private static void ConfigureProgrammeWeek(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgrammeWeek>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasIndex(e => e.AssignedProgramId);
            entity.HasIndex(e => new { e.AssignedProgramId, e.WeekNumber }).IsUnique();
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.WeekStartDate).HasColumnType("date").IsRequired();

            // Configure WorkoutDays collection
            entity.HasMany(e => e.WorkoutDays)
                .WithOne(d => d.ProgrammeWeek)
                .HasForeignKey(d => d.ProgrammeWeekId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureWorkoutDay(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkoutDay>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.ProgrammeWeekId);
            entity.Property(e => e.ScheduledDate).HasColumnType("date");
            entity.Property(e => e.CompletedDate).HasColumnType("timestamp with time zone");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");

            // Configure Exercises collection
            entity.HasMany(e => e.Exercises)
                .WithOne(ex => ex.WorkoutDay)
                .HasForeignKey(ex => ex.WorkoutDayId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureWorkoutExercise(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkoutExercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.TargetWeight).HasPrecision(10, 2);
            entity.Property(e => e.TargetDistance).HasPrecision(10, 2);
            entity.HasIndex(e => e.WorkoutDayId);
            entity.HasIndex(e => e.ExerciseDefinitionId);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");

            // Configure Sets collection
            entity.HasMany(e => e.Sets)
                .WithOne(s => s.WorkoutExercise)
                .HasForeignKey(s => s.WorkoutExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Exercise navigation to ExerciseDefinition (aggregate)
            entity.HasOne(e => e.ExerciseDefinition)
                .WithMany()
                .HasForeignKey(e => e.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureExerciseSet(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseSet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Weight).HasPrecision(10, 2);
            entity.Property(e => e.Distance).HasPrecision(10, 2);
            entity.HasIndex(e => e.WorkoutExerciseId);
            entity.Property(e => e.CompletedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
        });
    }

    private static void ConfigureExercise(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Instructions).HasMaxLength(2000);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
        });
    }

    private static void SeedExercises(ModelBuilder modelBuilder)
    {
        // Static date for all seeded exercises
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // Empty list for EquipmentRequirements (EF Core will serialize to JSON automatically)
        var emptyEquipmentList = new List<EquipmentRequirement>();

        modelBuilder.Entity<ExerciseDefinition>().HasData(
            // PUSH PATTERN - Chest & Shoulders
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Bench Press", Description = "A compound exercise targeting the chest, shoulders, and triceps. Lie on a bench and press a barbell upward from your chest.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Push, PrimaryMuscleGroup = MuscleGroup.Chest, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Triceps, Instructions = "Lie flat on bench, grip bar slightly wider than shoulders. Lower bar to chest, then press up explosively.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Name = "Push-ups", Description = "A bodyweight exercise that strengthens the chest, shoulders, and triceps. No equipment needed.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Push, PrimaryMuscleGroup = MuscleGroup.Chest, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Triceps, Instructions = "Start in plank position, lower body until chest nearly touches floor, push back up.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Name = "Overhead Press", Description = "A vertical pressing movement that primarily targets the shoulders and triceps.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Push, PrimaryMuscleGroup = MuscleGroup.Shoulders, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Triceps, Instructions = "Stand with feet shoulder-width apart, press barbell or dumbbells overhead from shoulder height.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Name = "Dips", Description = "A bodyweight exercise targeting triceps, chest, and shoulders. Performed on parallel bars or bench.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Push, PrimaryMuscleGroup = MuscleGroup.Triceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Chest, Instructions = "Support body on parallel bars, lower until elbows are at 90 degrees, push back up.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Name = "Dumbbell Flyes", Description = "An isolation exercise that targets the chest muscles through a wide arc of motion.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Push, PrimaryMuscleGroup = MuscleGroup.Chest, SecondaryMuscleGroup = (MuscleGroup?)null, Instructions = "Lie on bench, hold dumbbells with arms slightly bent. Lower weights in wide arc, bring together above chest.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222221"), Name = "Pull-ups", Description = "A bodyweight exercise that targets the back and biceps. One of the best upper body exercises.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Pull, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Biceps, Instructions = "Hang from bar with palms facing away, pull body up until chin clears bar, lower with control.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Barbell Rows", Description = "A compound pulling exercise that targets the back, biceps, and rear deltoids.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Pull, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Biceps, Instructions = "Bend at hips, keep back straight, pull barbell to lower chest/upper abdomen, squeeze back muscles.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222223"), Name = "Lat Pulldown", Description = "A machine exercise that targets the latissimus dorsi and biceps.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Pull, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Biceps, Instructions = "Sit at lat pulldown machine, pull bar to upper chest, control the weight on the way up.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222224"), Name = "Face Pulls", Description = "A rear deltoid and upper back exercise performed with a cable machine.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Pull, PrimaryMuscleGroup = MuscleGroup.Shoulders, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Back, Instructions = "Set cable at face height, pull rope to face level, separate handles at end, squeeze rear delts.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("22222222-2222-2222-2222-222222222225"), Name = "Bent-Over Rows", Description = "A compound exercise targeting the middle and upper back, biceps, and rear deltoids.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Pull, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Biceps, Instructions = "Bend forward at hips, keep back straight, pull dumbbells or barbell to lower chest/upper abdomen.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333331"), Name = "Back Squat", Description = "The king of leg exercises. A compound movement targeting quadriceps, glutes, and core.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Squat, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Bar on upper back, feet shoulder-width apart, squat down until thighs parallel to floor, drive up through heels.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333332"), Name = "Front Squat", Description = "A squat variation with the bar in front, emphasizing quadriceps and core strength.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Squat, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Core, Instructions = "Bar across front deltoids, elbows up, squat down keeping torso upright, drive up through midfoot.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Goblet Squat", Description = "A beginner-friendly squat variation using a single dumbbell or kettlebell.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Squat, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Hold weight at chest, squat down keeping torso upright, drive up through heels.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Name = "Bodyweight Squat", Description = "A fundamental bodyweight exercise for lower body strength. No equipment needed.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Squat, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Feet shoulder-width apart, squat down until thighs parallel to floor, drive up through heels.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("33333333-3333-3333-3333-333333333335"), Name = "Jump Squat", Description = "A plyometric variation of the squat that adds explosive power and cardiovascular benefits.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Squat, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Perform a squat, then explosively jump up, land softly and immediately go into next squat.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444441"), Name = "Deadlift", Description = "The ultimate posterior chain exercise. Targets hamstrings, glutes, and entire back.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Hinge, PrimaryMuscleGroup = MuscleGroup.Hamstrings, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Feet hip-width apart, bar over midfoot, hinge at hips, keep back straight, drive hips forward to stand.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444442"), Name = "Romanian Deadlift", Description = "A deadlift variation emphasizing hamstring and glute stretch and strength.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Hinge, PrimaryMuscleGroup = MuscleGroup.Hamstrings, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Start standing, hinge at hips keeping legs relatively straight, lower bar along legs, feel hamstring stretch, return.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444443"), Name = "Good Mornings", Description = "A posterior chain exercise performed with a barbell on the upper back.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Hinge, PrimaryMuscleGroup = MuscleGroup.Hamstrings, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Bar on upper back, hinge at hips keeping back straight, lower torso until parallel to floor, return.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Hip Thrust", Description = "An exercise that directly targets the glutes through hip extension.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Hinge, PrimaryMuscleGroup = MuscleGroup.Glutes, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Hamstrings, Instructions = "Upper back on bench, bar across hips, drive hips up squeezing glutes, lower with control.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("44444444-4444-4444-4444-444444444445"), Name = "Kettlebell Swing", Description = "A dynamic hip hinge movement that builds power and endurance in the posterior chain.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Hinge, PrimaryMuscleGroup = MuscleGroup.Glutes, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Hamstrings, Instructions = "Hinge at hips, swing kettlebell from between legs to chest height using hip drive, not arms.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555551"), Name = "Walking Lunges", Description = "A dynamic unilateral leg exercise that improves balance and leg strength.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Lunge, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Step forward into lunge position, both knees at 90 degrees, push off front foot to step forward with other leg.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555552"), Name = "Reverse Lunges", Description = "A lunge variation that reduces knee stress while targeting quadriceps and glutes.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Lunge, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Step backward into lunge, front knee at 90 degrees, push through front heel to return to start.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555553"), Name = "Bulgarian Split Squat", Description = "An intense unilateral leg exercise performed with rear foot elevated.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Lunge, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Rear foot on bench, front leg does the work, squat down until front thigh parallel, drive up.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555554"), Name = "Lateral Lunges", Description = "A side-to-side lunge variation that targets the inner thighs and glutes.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Lunge, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Step to the side, shift weight to that leg, squat down keeping other leg straight, return to center.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Forward Lunges", Description = "A classic lunge variation stepping forward to target quadriceps and glutes.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Lunge, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Step forward into lunge, both knees at 90 degrees, push through front heel to return to start.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666661"), Name = "Farmer's Walk", Description = "A full-body strength and conditioning exercise. Carry heavy weights while walking.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Carry, PrimaryMuscleGroup = MuscleGroup.FullBody, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Core, Instructions = "Pick up heavy weights (dumbbells, kettlebells, or farmer's walk handles), walk for distance or time, keep core tight.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666662"), Name = "Suitcase Carry", Description = "A unilateral carry that challenges core stability and grip strength.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Carry, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.FullBody, Instructions = "Carry a single heavy weight at your side, walk while resisting lateral flexion, switch sides.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666663"), Name = "Overhead Carry", Description = "A carry variation with weight held overhead, challenging shoulder stability and core.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Carry, PrimaryMuscleGroup = MuscleGroup.Shoulders, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Core, Instructions = "Press weight overhead, walk while maintaining overhead position, keep core engaged.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("66666666-6666-6666-6666-666666666664"), Name = "Rack Carry", Description = "Carrying weight in the front rack position, challenging core and upper body.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Carry, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.FullBody, Instructions = "Hold weight(s) at shoulder height in front rack position, walk maintaining position.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777771"), Name = "Plank", Description = "A fundamental core strengthening exercise performed in a static position.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Isometric, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)null, Instructions = "Hold body in straight line on forearms and toes, keep core tight, don't let hips sag or rise.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777772"), Name = "Wall Sit", Description = "An isometric leg exercise that builds quadriceps and glute endurance.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Isometric, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Glutes, Instructions = "Back against wall, slide down until thighs parallel to floor, hold position, keep back flat against wall.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777773"), Name = "Hollow Body Hold", Description = "An advanced core exercise that targets the entire anterior core chain.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Isometric, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)null, Instructions = "Lie on back, lift shoulders and legs off ground, maintain banana shape, keep lower back pressed to floor.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777774"), Name = "L-Sit", Description = "An advanced isometric exercise that challenges core, hip flexors, and triceps.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Isometric, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Triceps, Instructions = "Support body on parallel bars or floor, lift legs to form L-shape, hold position.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("77777777-7777-7777-7777-777777777775"), Name = "Dead Hang", Description = "An isometric exercise that improves grip strength and shoulder mobility.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Isometric, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Forearms, Instructions = "Hang from pull-up bar with straight arms, hold position, focus on grip and shoulder engagement.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888881"), Name = "Running", Description = "A fundamental cardiovascular exercise that improves endurance and cardiovascular health.", Type = ExerciseType.Cardio, MovementPattern = MovementPattern.Cardio, PrimaryMuscleGroup = MuscleGroup.Cardio, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.FullBody, Instructions = "Maintain steady pace, land on midfoot, keep posture upright, breathe rhythmically.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888882"), Name = "Cycling", Description = "A low-impact cardiovascular exercise that targets the legs and improves endurance.", Type = ExerciseType.Cardio, MovementPattern = MovementPattern.Cardio, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Cardio, Instructions = "Maintain consistent cadence, adjust resistance for intensity, keep core engaged.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888883"), Name = "Rowing", Description = "A full-body cardiovascular exercise that targets legs, back, and core.", Type = ExerciseType.Cardio, MovementPattern = MovementPattern.Cardio, PrimaryMuscleGroup = MuscleGroup.FullBody, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Cardio, Instructions = "Drive with legs, lean back slightly, pull handle to chest, return with control.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888884"), Name = "Burpees", Description = "A high-intensity full-body exercise combining squat, push-up, and jump.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Cardio, PrimaryMuscleGroup = MuscleGroup.FullBody, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Cardio, Instructions = "Squat down, jump feet back to plank, do push-up, jump feet forward, jump up with arms overhead.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("88888888-8888-8888-8888-888888888885"), Name = "Jump Rope", Description = "A high-intensity cardiovascular exercise that improves coordination and endurance.", Type = ExerciseType.Cardio, MovementPattern = MovementPattern.Cardio, PrimaryMuscleGroup = MuscleGroup.Cardio, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Calves, Instructions = "Keep elbows close, jump on balls of feet, maintain rhythm, start slow and build speed.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999991"), Name = "Hip Flexor Stretch", Description = "A stretching exercise that improves hip flexibility and reduces lower back tension.", Type = ExerciseType.Flexibility, MovementPattern = MovementPattern.Flexibility, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)null, Instructions = "Kneel on one knee, other foot forward, push hips forward until stretch felt in front of hip, hold 30-60 seconds.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999992"), Name = "Hamstring Stretch", Description = "A flexibility exercise targeting the hamstring muscles.", Type = ExerciseType.Flexibility, MovementPattern = MovementPattern.Flexibility, PrimaryMuscleGroup = MuscleGroup.Hamstrings, SecondaryMuscleGroup = (MuscleGroup?)null, Instructions = "Sit or stand, extend one leg, lean forward until stretch felt in back of thigh, hold 30-60 seconds.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999993"), Name = "Shoulder Mobility", Description = "A mobility exercise that improves shoulder range of motion and flexibility.", Type = ExerciseType.Flexibility, MovementPattern = MovementPattern.Flexibility, PrimaryMuscleGroup = MuscleGroup.Shoulders, SecondaryMuscleGroup = (MuscleGroup?)null, Instructions = "Perform arm circles, cross-body stretches, and wall slides to improve shoulder mobility.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999994"), Name = "Cat-Cow Stretch", Description = "A spinal mobility exercise that improves flexibility in the back and core.", Type = ExerciseType.Flexibility, MovementPattern = MovementPattern.Flexibility, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Back, Instructions = "Start on hands and knees, arch back (cow), then round back (cat), move slowly between positions.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("99999999-9999-9999-9999-999999999995"), Name = "Pigeon Pose", Description = "A yoga-inspired stretch that targets the hip flexors and glutes.", Type = ExerciseType.Flexibility, MovementPattern = MovementPattern.Flexibility, PrimaryMuscleGroup = MuscleGroup.Glutes, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Core, Instructions = "From plank, bring one knee forward to same-side wrist, extend other leg back, hold and breathe.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), Name = "Thruster", Description = "A full-body exercise combining front squat and overhead press in one movement.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Squat, PrimaryMuscleGroup = MuscleGroup.FullBody, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Shoulders, Instructions = "Hold weight at shoulder height, squat down, drive up explosively and press weight overhead.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAB"), Name = "Turkish Get-Up", Description = "A complex full-body movement that improves stability, strength, and coordination.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Carry, PrimaryMuscleGroup = MuscleGroup.FullBody, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Core, Instructions = "Lie on back holding weight overhead, get up to standing position through series of movements, reverse to return.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAC"), Name = "Renegade Row", Description = "A compound exercise combining plank and rowing motion for core and back.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Pull, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Core, Instructions = "In plank position with dumbbells, row one weight to side while maintaining plank, alternate sides.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAD"), Name = "Mountain Climbers", Description = "A dynamic core and cardio exercise performed in plank position.", Type = ExerciseType.Bodyweight, MovementPattern = MovementPattern.Cardio, PrimaryMuscleGroup = MuscleGroup.Core, SecondaryMuscleGroup = (MuscleGroup?)MuscleGroup.Cardio, Instructions = "In plank position, alternate bringing knees to chest rapidly, keep core engaged throughout.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList },
            new { Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAE"), Name = "Calf Raises", Description = "An isolation exercise targeting the calf muscles.", Type = ExerciseType.WeightTraining, MovementPattern = MovementPattern.Push, PrimaryMuscleGroup = MuscleGroup.Calves, SecondaryMuscleGroup = (MuscleGroup?)null, Instructions = "Stand on balls of feet, raise up onto toes, lower with control, can add weight for resistance.", VideoUrl = (string?)null, IsSystemExercise = true, CreatedByUserId = (Guid?)null, CreatedAt = seedDate, _equipmentRequirements = emptyEquipmentList }
        );
    }
}