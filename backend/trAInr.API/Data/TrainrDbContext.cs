using Microsoft.EntityFrameworkCore;
using trAInr.API.Models.Domain;

namespace trAInr.API.Data;

/// <summary>
/// Entity Framework Core database context for the trAInr application
/// </summary>
public class TrainrDbContext : DbContext
{
  public TrainrDbContext(DbContextOptions<TrainrDbContext> options) : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<Programme> Programmes => Set<Programme>();
  public DbSet<ProgrammeWeek> ProgrammeWeeks => Set<ProgrammeWeek>();
  public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
  public DbSet<Exercise> Exercises => Set<Exercise>();
  public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
  public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    ConfigureUser(modelBuilder);
    ConfigureProgramme(modelBuilder);
    ConfigureProgrammeWeek(modelBuilder);
    ConfigureWorkoutDay(modelBuilder);
    ConfigureExercise(modelBuilder);
    ConfigureWorkoutExercise(modelBuilder);
    ConfigureExerciseSet(modelBuilder);

    SeedExercises(modelBuilder);
  }

  private static void ConfigureUser(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => e.Email).IsUnique();
      entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
      entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
      entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();

      entity.HasMany(e => e.Programmes)
              .WithOne(p => p.User)
              .HasForeignKey(p => p.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });
  }

  private static void ConfigureProgramme(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Programme>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
      entity.Property(e => e.Description).HasMaxLength(1000);
      entity.HasIndex(e => e.UserId);
      entity.HasIndex(e => e.IsActive);

      entity.HasMany(e => e.Weeks)
              .WithOne(w => w.Programme)
              .HasForeignKey(w => w.ProgrammeId)
              .OnDelete(DeleteBehavior.Cascade);
    });
  }

  private static void ConfigureProgrammeWeek(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ProgrammeWeek>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.HasIndex(e => new { e.ProgrammeId, e.WeekNumber }).IsUnique();
      entity.Property(e => e.Notes).HasMaxLength(500);

      entity.HasMany(e => e.WorkoutDays)
              .WithOne(w => w.ProgrammeWeek)
              .HasForeignKey(w => w.ProgrammeWeekId)
              .OnDelete(DeleteBehavior.Cascade);
    });
  }

  private static void ConfigureWorkoutDay(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<WorkoutDay>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
      entity.Property(e => e.Description).HasMaxLength(500);
      entity.HasIndex(e => e.ProgrammeWeekId);
      entity.HasIndex(e => e.ScheduledDate);

      entity.HasMany(e => e.Exercises)
              .WithOne(e => e.WorkoutDay)
              .HasForeignKey(e => e.WorkoutDayId)
              .OnDelete(DeleteBehavior.Cascade);
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
      entity.HasIndex(e => e.Type);
      entity.HasIndex(e => e.PrimaryMuscleGroup);
    });
  }

  private static void ConfigureWorkoutExercise(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<WorkoutExercise>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Notes).HasMaxLength(500);
      entity.Property(e => e.TargetWeight).HasPrecision(10, 2);
      entity.Property(e => e.TargetDistance).HasPrecision(10, 2);
      entity.HasIndex(e => new { e.WorkoutDayId, e.OrderIndex });

      entity.HasMany(e => e.Sets)
              .WithOne(s => s.WorkoutExercise)
              .HasForeignKey(s => s.WorkoutExerciseId)
              .OnDelete(DeleteBehavior.Cascade);
    });
  }

  private static void ConfigureExerciseSet(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ExerciseSet>(entity =>
    {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Weight).HasPrecision(10, 2);
      entity.Property(e => e.Distance).HasPrecision(10, 2);
      entity.Property(e => e.Notes).HasMaxLength(500);
      entity.HasIndex(e => e.WorkoutExerciseId);
      entity.HasIndex(e => e.CompletedAt);
    });
  }

  private static void SeedExercises(ModelBuilder modelBuilder)
  {
    var exercises = new List<Exercise>
        {
            // Chest exercises
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Name = "Bench Press", Description = "Barbell bench press for chest development", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Chest, SecondaryMuscleGroup = MuscleGroup.Triceps },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Name = "Incline Dumbbell Press", Description = "Incline press targeting upper chest", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Chest, SecondaryMuscleGroup = MuscleGroup.Shoulders },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Name = "Cable Fly", Description = "Cable chest fly for isolation", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Chest },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Name = "Push-ups", Description = "Bodyweight chest exercise", Type = ExerciseType.Bodyweight, PrimaryMuscleGroup = MuscleGroup.Chest, SecondaryMuscleGroup = MuscleGroup.Triceps },
            
            // Back exercises
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111201"), Name = "Deadlift", Description = "Conventional deadlift for overall back development", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = MuscleGroup.Hamstrings },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111202"), Name = "Pull-ups", Description = "Bodyweight back exercise", Type = ExerciseType.Bodyweight, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = MuscleGroup.Biceps },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111203"), Name = "Barbell Row", Description = "Bent-over barbell row", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Back, SecondaryMuscleGroup = MuscleGroup.Biceps },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111204"), Name = "Lat Pulldown", Description = "Cable lat pulldown", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Back },
            
            // Shoulder exercises
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111301"), Name = "Overhead Press", Description = "Standing barbell overhead press", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Shoulders, SecondaryMuscleGroup = MuscleGroup.Triceps },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111302"), Name = "Lateral Raise", Description = "Dumbbell lateral raise", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Shoulders },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111303"), Name = "Face Pull", Description = "Cable face pull for rear delts", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Shoulders },
            
            // Arm exercises
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111401"), Name = "Barbell Curl", Description = "Standing barbell bicep curl", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Biceps },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111402"), Name = "Tricep Pushdown", Description = "Cable tricep pushdown", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Triceps },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111403"), Name = "Hammer Curl", Description = "Dumbbell hammer curl", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Biceps, SecondaryMuscleGroup = MuscleGroup.Forearms },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111404"), Name = "Skull Crushers", Description = "Lying tricep extension", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Triceps },
            
            // Leg exercises
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111501"), Name = "Barbell Squat", Description = "Back squat for leg development", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = MuscleGroup.Glutes },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111502"), Name = "Romanian Deadlift", Description = "RDL for hamstring development", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Hamstrings, SecondaryMuscleGroup = MuscleGroup.Glutes },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111503"), Name = "Leg Press", Description = "Machine leg press", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Quadriceps },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111504"), Name = "Leg Curl", Description = "Lying leg curl", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Hamstrings },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111505"), Name = "Calf Raise", Description = "Standing calf raise", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Calves },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111506"), Name = "Lunges", Description = "Walking or stationary lunges", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Quadriceps, SecondaryMuscleGroup = MuscleGroup.Glutes },
            
            // Core exercises
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111601"), Name = "Plank", Description = "Isometric core exercise", Type = ExerciseType.Bodyweight, PrimaryMuscleGroup = MuscleGroup.Core },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111602"), Name = "Cable Crunch", Description = "Weighted cable crunch", Type = ExerciseType.WeightTraining, PrimaryMuscleGroup = MuscleGroup.Core },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111603"), Name = "Hanging Leg Raise", Description = "Core exercise for lower abs", Type = ExerciseType.Bodyweight, PrimaryMuscleGroup = MuscleGroup.Core },
            
            // Cardio exercises
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111701"), Name = "Running", Description = "Outdoor or treadmill running", Type = ExerciseType.Cardio, PrimaryMuscleGroup = MuscleGroup.Cardio },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111702"), Name = "Cycling", Description = "Stationary or outdoor cycling", Type = ExerciseType.Cardio, PrimaryMuscleGroup = MuscleGroup.Cardio },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111703"), Name = "Rowing", Description = "Rowing machine", Type = ExerciseType.Cardio, PrimaryMuscleGroup = MuscleGroup.Cardio, SecondaryMuscleGroup = MuscleGroup.Back },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111704"), Name = "Swimming", Description = "Swimming laps", Type = ExerciseType.Cardio, PrimaryMuscleGroup = MuscleGroup.FullBody },
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111705"), Name = "Jump Rope", Description = "Skipping rope for cardio", Type = ExerciseType.Cardio, PrimaryMuscleGroup = MuscleGroup.Cardio },
        };

    modelBuilder.Entity<Exercise>().HasData(exercises);
  }
}

