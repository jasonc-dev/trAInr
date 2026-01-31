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
    public DbSet<ProgramTemplate> ProgramTemplates => Set<ProgramTemplate>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

    // Entities
    public DbSet<ProgrammeWeek> ProgrammeWeeks => Set<ProgrammeWeek>();
    public DbSet<ProgramTemplateWeek> ProgramTemplateWeeks => Set<ProgramTemplateWeek>();
    public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
    public DbSet<ProgramTemplateWorkoutDay> ProgramTemplateWorkoutDays => Set<ProgramTemplateWorkoutDay>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<ProgramTemplateWorkoutExercise> ProgramTemplateWorkoutExercises => Set<ProgramTemplateWorkoutExercise>();
    public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<AiGenerationJob> AiGenerationJobs => Set<AiGenerationJob>();

    // New normalized entities for RAG
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Muscle> Muscles => Set<Muscle>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ExerciseEquipment> ExerciseEquipments => Set<ExerciseEquipment>();
    public DbSet<ExerciseMuscle> ExerciseMuscles => Set<ExerciseMuscle>();
    public DbSet<ExerciseTag> ExerciseTags => Set<ExerciseTag>();
    public DbSet<ExerciseVariant> ExerciseVariants => Set<ExerciseVariant>();
    public DbSet<ExerciseEmbedding> ExerciseEmbeddings => Set<ExerciseEmbedding>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Register pgvector type mapping
        optionsBuilder.UseNpgsql(options => options.UseVector());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable pgvector extension
        modelBuilder.HasPostgresExtension("vector");

        // Explicitly exclude DomainEvent from the model (it's abstract and handled separately)
        modelBuilder.Ignore<DomainEvent>();

        // DDD Aggregate configurations
        ConfigureAthlete(modelBuilder);
        ConfigureExerciseDefinition(modelBuilder);
        ConfigureAssignedProgram(modelBuilder);
        ConfigureProgramTemplate(modelBuilder);
        ConfigureWorkoutSession(modelBuilder);
        ConfigureProgrammeWeek(modelBuilder);
        ConfigureProgramTemplateWeek(modelBuilder);
        ConfigureWorkoutDay(modelBuilder);
        ConfigureProgramTemplateWorkoutDay(modelBuilder);
        ConfigureWorkoutExercise(modelBuilder);
        ConfigureProgramTemplateWorkoutExercise(modelBuilder);
        ConfigureExerciseSet(modelBuilder);
        ConfigureExercise(modelBuilder);
        ConfigureAiGenerationJob(modelBuilder);

        // New normalized entity configurations for RAG
        ConfigureEquipment(modelBuilder);
        ConfigureMuscle(modelBuilder);
        ConfigureTag(modelBuilder);
        ConfigureExerciseEquipment(modelBuilder);
        ConfigureExerciseMuscle(modelBuilder);
        ConfigureExerciseTag(modelBuilder);
        ConfigureExerciseVariant(modelBuilder);
        ConfigureExerciseEmbedding(modelBuilder);
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
            entity.Property(e => e.ShortCue).HasMaxLength(120);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.PrimaryMuscleGroup);

            // Add indexes for new fields
            entity.HasIndex(e => new { e.SpinalLoad, e.SetupComplexity, e.TrackingMode });

            // Configure value objects as JSON column (simpler for record structs)
            entity.Property<List<EquipmentRequirement>>("_equipmentRequirements")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<EquipmentRequirement>>(v, (JsonSerializerOptions?)null) ??
                         new List<EquipmentRequirement>())
                .HasColumnName("EquipmentRequirements");
            entity.Property<List<EquipmentRequirement>>("_equipmentRequirements").Metadata
                .SetValueComparer(EquipmentRequirementListComparer);

            // Configure Aliases as array
            entity.Property(e => e.Aliases)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .HasColumnName("Aliases");
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

    private static void ConfigureProgramTemplate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgramTemplate>(entity =>
        {
            entity.HasKey(pt => pt.Id);
            entity.Property(pt => pt.Name).HasMaxLength(200).IsRequired();
            entity.Property(pt => pt.Description).HasMaxLength(1000).IsRequired();
            entity.Property(pt => pt.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(pt => pt.UpdatedAt).HasColumnType("timestamp with time zone");
            entity.HasIndex(pt => pt.Name);
            entity.HasIndex(pt => pt.ExperienceLevel);
        });
    }

    private static void ConfigureProgramTemplateWeek(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgramTemplateWeek>(entity =>
        {
            entity.HasKey(ptw => ptw.Id);
            entity.Property(ptw => ptw.Notes).HasMaxLength(500);
            entity.Property(ptw => ptw.CreatedAt).HasColumnType("timestamp with time zone");
            entity.HasIndex(ptw => new { ptw.ProgramTemplateId, ptw.WeekNumber }).IsUnique();

            entity.HasOne(ptw => ptw.ProgramTemplate)
                .WithMany(pt => pt.Weeks)
                .HasForeignKey(ptw => ptw.ProgramTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProgramTemplateWorkoutDay(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgramTemplateWorkoutDay>(entity =>
        {
            entity.HasKey(ptwd => ptwd.Id);
            entity.Property(ptwd => ptwd.Name).HasMaxLength(100).IsRequired();
            entity.Property(ptwd => ptwd.Description).HasMaxLength(500);
            entity.Property(ptwd => ptwd.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(ptwd => ptwd.ProgramTemplateWeek)
                .WithMany(ptw => ptw.WorkoutDays)
                .HasForeignKey(ptwd => ptwd.ProgramTemplateWeekId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureProgramTemplateWorkoutExercise(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgramTemplateWorkoutExercise>(entity =>
        {
            entity.HasKey(ptwe => ptwe.Id);
            entity.Property(ptwe => ptwe.Notes).HasMaxLength(500);
            entity.Property(ptwe => ptwe.CreatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(ptwe => ptwe.ProgramTemplateWorkoutDay)
                .WithMany(ptwd => ptwd.Exercises)
                .HasForeignKey(ptwe => ptwe.ProgramTemplateWorkoutDayId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ptwe => ptwe.ExerciseDefinition)
                .WithMany()
                .HasForeignKey(ptwe => ptwe.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureAiGenerationJob(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiGenerationJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasMaxLength(20).IsRequired();
            entity.Property(e => e.RequestData).IsRequired();
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp with time zone");
            entity.Property(e => e.CompletedAt).HasColumnType("timestamp with time zone");
        });
    }

    private static void ConfigureEquipment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Category);
        });
    }

    private static void ConfigureMuscle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Muscle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Group).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Group);
        });
    }

    private static void ConfigureTag(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => new { e.Namespace, e.Name }).IsUnique();
        });
    }

    private static void ConfigureExerciseEquipment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseEquipment>(entity =>
        {
            entity.HasKey(ee => new { ee.ExerciseDefinitionId, ee.EquipmentId });

            entity.HasOne(ee => ee.ExerciseDefinition)
                .WithMany(e => e.ExerciseEquipments)
                .HasForeignKey(ee => ee.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ee => ee.Equipment)
                .WithMany(eq => eq.ExerciseEquipments)
                .HasForeignKey(ee => ee.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ee => ee.EquipmentId);
        });
    }

    private static void ConfigureExerciseMuscle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseMuscle>(entity =>
        {
            entity.HasKey(em => new { em.ExerciseDefinitionId, em.MuscleId, em.Role });

            entity.HasOne(em => em.ExerciseDefinition)
                .WithMany(e => e.ExerciseMuscles)
                .HasForeignKey(em => em.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(em => em.Muscle)
                .WithMany(m => m.ExerciseMuscles)
                .HasForeignKey(em => em.MuscleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(em => new { em.MuscleId, em.Role });
        });
    }

    private static void ConfigureExerciseTag(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseTag>(entity =>
        {
            entity.HasKey(et => new { et.ExerciseDefinitionId, et.TagId });

            entity.HasOne(et => et.ExerciseDefinition)
                .WithMany(e => e.ExerciseTags)
                .HasForeignKey(et => et.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(et => et.Tag)
                .WithMany(t => t.ExerciseTags)
                .HasForeignKey(et => et.TagId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(et => et.TagId);
        });
    }

    private static void ConfigureExerciseVariant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseVariant>(entity =>
        {
            entity.HasKey(ev => new { ev.ExerciseDefinitionId, ev.VariantOfExerciseDefinitionId, ev.VariantType });

            entity.HasOne(ev => ev.ExerciseDefinition)
                .WithMany(e => e.ExerciseVariants)
                .HasForeignKey(ev => ev.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ev => ev.VariantOfExerciseDefinition)
                .WithMany(e => e.VariantOfExercises)
                .HasForeignKey(ev => ev.VariantOfExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureExerciseEmbedding(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExerciseEmbedding>(entity =>
        {
            entity.HasKey(e => e.ExerciseDefinitionId);

            entity.Property(e => e.Embedding)
                .HasColumnType("vector(1536)")
                .IsRequired();

            entity.Property(e => e.EmbeddingModel)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasOne(e => e.ExerciseDefinition)
                .WithOne()
                .HasForeignKey<ExerciseEmbedding>(e => e.ExerciseDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create HNSW index for vector similarity search
            entity.HasIndex(e => e.Embedding)
                .HasMethod("hnsw")
                .HasOperators("vector_cosine_ops");
        });
    }
}