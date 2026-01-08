using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignedPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CurrentPhase = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Athletes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    PrimaryGoal = table.Column<int>(type: "integer", nullable: false),
                    WorkoutDaysPerWeek = table.Column<int>(type: "integer", nullable: false),
                    ReadinessScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ReadinessNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    _constraints = table.Column<string>(type: "text", nullable: false),
                    EquipmentPreferences = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Athletes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    PrimaryMuscleGroup = table.Column<int>(type: "integer", nullable: false),
                    SecondaryMuscleGroup = table.Column<int>(type: "integer", nullable: true),
                    Instructions = table.Column<string>(type: "text", nullable: true),
                    VideoUrl = table.Column<string>(type: "text", nullable: true),
                    IsSystemExercise = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MovementPattern = table.Column<int>(type: "integer", nullable: false),
                    PrimaryMuscleGroup = table.Column<int>(type: "integer", nullable: false),
                    SecondaryMuscleGroup = table.Column<int>(type: "integer", nullable: true),
                    Instructions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    VideoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsSystemExercise = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EquipmentRequirements = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AthleteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgrammeWeekId = table.Column<Guid>(type: "uuid", nullable: true),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsRestDay = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeWeeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgrammeWeeks_AssignedPrograms_AssignedProgramId",
                        column: x => x.AssignedProgramId,
                        principalTable: "AssignedPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseInstances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    TargetSets = table.Column<int>(type: "integer", nullable: false),
                    TargetReps = table.Column<int>(type: "integer", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    TargetDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    TargetDistance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseInstances_WorkoutSessions_WorkoutSessionId",
                        column: x => x.WorkoutSessionId,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutDay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgrammeWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsRestDay = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutDay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutDay_ProgrammeWeeks_ProgrammeWeekId",
                        column: x => x.ProgrammeWeekId,
                        principalTable: "ProgrammeWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompletedSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    RPE = table.Column<int>(type: "integer", nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    Distance = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsWarmup = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompletedSets_ExerciseInstances_ExerciseInstanceId",
                        column: x => x.ExerciseInstanceId,
                        principalTable: "ExerciseInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExercise",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutDayId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    TargetSets = table.Column<int>(type: "integer", nullable: false),
                    TargetReps = table.Column<int>(type: "integer", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    TargetDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    TargetDistance = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutExercise_Exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutExercise_WorkoutDay_WorkoutDayId",
                        column: x => x.WorkoutDayId,
                        principalTable: "WorkoutDay",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkoutExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    Distance = table.Column<decimal>(type: "numeric", nullable: true),
                    Difficulty = table.Column<int>(type: "integer", nullable: true),
                    Intensity = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsWarmup = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseSet_WorkoutExercise_WorkoutExerciseId",
                        column: x => x.WorkoutExerciseId,
                        principalTable: "WorkoutExercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExerciseDefinitions",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "Description", "Instructions", "IsSystemExercise", "MovementPattern", "Name", "PrimaryMuscleGroup", "SecondaryMuscleGroup", "Type", "VideoUrl", "EquipmentRequirements" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A compound exercise targeting the chest, shoulders, and triceps. Lie on a bench and press a barbell upward from your chest.", "Lie flat on bench, grip bar slightly wider than shoulders. Lower bar to chest, then press up explosively.", true, 1, "Bench Press", 1, 5, 1, null, "[]" },
                    { new Guid("11111111-1111-1111-1111-111111111112"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A bodyweight exercise that strengthens the chest, shoulders, and triceps. No equipment needed.", "Start in plank position, lower body until chest nearly touches floor, push back up.", true, 1, "Push-ups", 1, 5, 3, null, "[]" },
                    { new Guid("11111111-1111-1111-1111-111111111113"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A vertical pressing movement that primarily targets the shoulders and triceps.", "Stand with feet shoulder-width apart, press barbell or dumbbells overhead from shoulder height.", true, 1, "Overhead Press", 3, 5, 1, null, "[]" },
                    { new Guid("11111111-1111-1111-1111-111111111114"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A bodyweight exercise targeting triceps, chest, and shoulders. Performed on parallel bars or bench.", "Support body on parallel bars, lower until elbows are at 90 degrees, push back up.", true, 1, "Dips", 5, 1, 3, null, "[]" },
                    { new Guid("11111111-1111-1111-1111-111111111115"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An isolation exercise that targets the chest muscles through a wide arc of motion.", "Lie on bench, hold dumbbells with arms slightly bent. Lower weights in wide arc, bring together above chest.", true, 1, "Dumbbell Flyes", 1, null, 1, null, "[]" },
                    { new Guid("22222222-2222-2222-2222-222222222221"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A bodyweight exercise that targets the back and biceps. One of the best upper body exercises.", "Hang from bar with palms facing away, pull body up until chin clears bar, lower with control.", true, 2, "Pull-ups", 2, 4, 3, null, "[]" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A compound pulling exercise that targets the back, biceps, and rear deltoids.", "Bend at hips, keep back straight, pull barbell to lower chest/upper abdomen, squeeze back muscles.", true, 2, "Barbell Rows", 2, 4, 1, null, "[]" },
                    { new Guid("22222222-2222-2222-2222-222222222223"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A machine exercise that targets the latissimus dorsi and biceps.", "Sit at lat pulldown machine, pull bar to upper chest, control the weight on the way up.", true, 2, "Lat Pulldown", 2, 4, 1, null, "[]" },
                    { new Guid("22222222-2222-2222-2222-222222222224"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A rear deltoid and upper back exercise performed with a cable machine.", "Set cable at face height, pull rope to face level, separate handles at end, squeeze rear delts.", true, 2, "Face Pulls", 3, 2, 1, null, "[]" },
                    { new Guid("22222222-2222-2222-2222-222222222225"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A compound exercise targeting the middle and upper back, biceps, and rear deltoids.", "Bend forward at hips, keep back straight, pull dumbbells or barbell to lower chest/upper abdomen.", true, 2, "Bent-Over Rows", 2, 4, 1, null, "[]" },
                    { new Guid("33333333-3333-3333-3333-333333333331"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "The king of leg exercises. A compound movement targeting quadriceps, glutes, and core.", "Bar on upper back, feet shoulder-width apart, squat down until thighs parallel to floor, drive up through heels.", true, 3, "Back Squat", 8, 10, 1, null, "[]" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A squat variation with the bar in front, emphasizing quadriceps and core strength.", "Bar across front deltoids, elbows up, squat down keeping torso upright, drive up through midfoot.", true, 3, "Front Squat", 8, 7, 1, null, "[]" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A beginner-friendly squat variation using a single dumbbell or kettlebell.", "Hold weight at chest, squat down keeping torso upright, drive up through heels.", true, 3, "Goblet Squat", 8, 10, 1, null, "[]" },
                    { new Guid("33333333-3333-3333-3333-333333333334"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A fundamental bodyweight exercise for lower body strength. No equipment needed.", "Feet shoulder-width apart, squat down until thighs parallel to floor, drive up through heels.", true, 3, "Bodyweight Squat", 8, 10, 3, null, "[]" },
                    { new Guid("33333333-3333-3333-3333-333333333335"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A plyometric variation of the squat that adds explosive power and cardiovascular benefits.", "Perform a squat, then explosively jump up, land softly and immediately go into next squat.", true, 3, "Jump Squat", 8, 10, 3, null, "[]" },
                    { new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "The ultimate posterior chain exercise. Targets hamstrings, glutes, and entire back.", "Feet hip-width apart, bar over midfoot, hinge at hips, keep back straight, drive hips forward to stand.", true, 4, "Deadlift", 9, 10, 1, null, "[]" },
                    { new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A deadlift variation emphasizing hamstring and glute stretch and strength.", "Start standing, hinge at hips keeping legs relatively straight, lower bar along legs, feel hamstring stretch, return.", true, 4, "Romanian Deadlift", 9, 10, 1, null, "[]" },
                    { new Guid("44444444-4444-4444-4444-444444444443"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A posterior chain exercise performed with a barbell on the upper back.", "Bar on upper back, hinge at hips keeping back straight, lower torso until parallel to floor, return.", true, 4, "Good Mornings", 9, 10, 1, null, "[]" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An exercise that directly targets the glutes through hip extension.", "Upper back on bench, bar across hips, drive hips up squeezing glutes, lower with control.", true, 4, "Hip Thrust", 10, 9, 1, null, "[]" },
                    { new Guid("44444444-4444-4444-4444-444444444445"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A dynamic hip hinge movement that builds power and endurance in the posterior chain.", "Hinge at hips, swing kettlebell from between legs to chest height using hip drive, not arms.", true, 4, "Kettlebell Swing", 10, 9, 1, null, "[]" },
                    { new Guid("55555555-5555-5555-5555-555555555551"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A dynamic unilateral leg exercise that improves balance and leg strength.", "Step forward into lunge position, both knees at 90 degrees, push off front foot to step forward with other leg.", true, 5, "Walking Lunges", 8, 10, 1, null, "[]" },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A lunge variation that reduces knee stress while targeting quadriceps and glutes.", "Step backward into lunge, front knee at 90 degrees, push through front heel to return to start.", true, 5, "Reverse Lunges", 8, 10, 1, null, "[]" },
                    { new Guid("55555555-5555-5555-5555-555555555553"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An intense unilateral leg exercise performed with rear foot elevated.", "Rear foot on bench, front leg does the work, squat down until front thigh parallel, drive up.", true, 5, "Bulgarian Split Squat", 8, 10, 1, null, "[]" },
                    { new Guid("55555555-5555-5555-5555-555555555554"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A side-to-side lunge variation that targets the inner thighs and glutes.", "Step to the side, shift weight to that leg, squat down keeping other leg straight, return to center.", true, 5, "Lateral Lunges", 8, 10, 1, null, "[]" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A classic lunge variation stepping forward to target quadriceps and glutes.", "Step forward into lunge, both knees at 90 degrees, push through front heel to return to start.", true, 5, "Forward Lunges", 8, 10, 1, null, "[]" },
                    { new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A full-body strength and conditioning exercise. Carry heavy weights while walking.", "Pick up heavy weights (dumbbells, kettlebells, or farmer's walk handles), walk for distance or time, keep core tight.", true, 6, "Farmer's Walk", 12, 7, 1, null, "[]" },
                    { new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A unilateral carry that challenges core stability and grip strength.", "Carry a single heavy weight at your side, walk while resisting lateral flexion, switch sides.", true, 6, "Suitcase Carry", 7, 12, 1, null, "[]" },
                    { new Guid("66666666-6666-6666-6666-666666666663"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A carry variation with weight held overhead, challenging shoulder stability and core.", "Press weight overhead, walk while maintaining overhead position, keep core engaged.", true, 6, "Overhead Carry", 3, 7, 1, null, "[]" },
                    { new Guid("66666666-6666-6666-6666-666666666664"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Carrying weight in the front rack position, challenging core and upper body.", "Hold weight(s) at shoulder height in front rack position, walk maintaining position.", true, 6, "Rack Carry", 7, 12, 1, null, "[]" },
                    { new Guid("77777777-7777-7777-7777-777777777771"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A fundamental core strengthening exercise performed in a static position.", "Hold body in straight line on forearms and toes, keep core tight, don't let hips sag or rise.", true, 7, "Plank", 7, null, 3, null, "[]" },
                    { new Guid("77777777-7777-7777-7777-777777777772"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An isometric leg exercise that builds quadriceps and glute endurance.", "Back against wall, slide down until thighs parallel to floor, hold position, keep back flat against wall.", true, 7, "Wall Sit", 8, 10, 3, null, "[]" },
                    { new Guid("77777777-7777-7777-7777-777777777773"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An advanced core exercise that targets the entire anterior core chain.", "Lie on back, lift shoulders and legs off ground, maintain banana shape, keep lower back pressed to floor.", true, 7, "Hollow Body Hold", 7, null, 3, null, "[]" },
                    { new Guid("77777777-7777-7777-7777-777777777774"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An advanced isometric exercise that challenges core, hip flexors, and triceps.", "Support body on parallel bars or floor, lift legs to form L-shape, hold position.", true, 7, "L-Sit", 7, 5, 3, null, "[]" },
                    { new Guid("77777777-7777-7777-7777-777777777775"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An isometric exercise that improves grip strength and shoulder mobility.", "Hang from pull-up bar with straight arms, hold position, focus on grip and shoulder engagement.", true, 7, "Dead Hang", 2, 6, 3, null, "[]" },
                    { new Guid("88888888-8888-8888-8888-888888888881"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A fundamental cardiovascular exercise that improves endurance and cardiovascular health.", "Maintain steady pace, land on midfoot, keep posture upright, breathe rhythmically.", true, 8, "Running", 13, 12, 2, null, "[]" },
                    { new Guid("88888888-8888-8888-8888-888888888882"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A low-impact cardiovascular exercise that targets the legs and improves endurance.", "Maintain consistent cadence, adjust resistance for intensity, keep core engaged.", true, 8, "Cycling", 8, 13, 2, null, "[]" },
                    { new Guid("88888888-8888-8888-8888-888888888883"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A full-body cardiovascular exercise that targets legs, back, and core.", "Drive with legs, lean back slightly, pull handle to chest, return with control.", true, 8, "Rowing", 12, 13, 2, null, "[]" },
                    { new Guid("88888888-8888-8888-8888-888888888884"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A high-intensity full-body exercise combining squat, push-up, and jump.", "Squat down, jump feet back to plank, do push-up, jump feet forward, jump up with arms overhead.", true, 8, "Burpees", 12, 13, 3, null, "[]" },
                    { new Guid("88888888-8888-8888-8888-888888888885"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A high-intensity cardiovascular exercise that improves coordination and endurance.", "Keep elbows close, jump on balls of feet, maintain rhythm, start slow and build speed.", true, 8, "Jump Rope", 13, 11, 2, null, "[]" },
                    { new Guid("99999999-9999-9999-9999-999999999991"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A stretching exercise that improves hip flexibility and reduces lower back tension.", "Kneel on one knee, other foot forward, push hips forward until stretch felt in front of hip, hold 30-60 seconds.", true, 9, "Hip Flexor Stretch", 7, null, 4, null, "[]" },
                    { new Guid("99999999-9999-9999-9999-999999999992"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A flexibility exercise targeting the hamstring muscles.", "Sit or stand, extend one leg, lean forward until stretch felt in back of thigh, hold 30-60 seconds.", true, 9, "Hamstring Stretch", 9, null, 4, null, "[]" },
                    { new Guid("99999999-9999-9999-9999-999999999993"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A mobility exercise that improves shoulder range of motion and flexibility.", "Perform arm circles, cross-body stretches, and wall slides to improve shoulder mobility.", true, 9, "Shoulder Mobility", 3, null, 4, null, "[]" },
                    { new Guid("99999999-9999-9999-9999-999999999994"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A spinal mobility exercise that improves flexibility in the back and core.", "Start on hands and knees, arch back (cow), then round back (cat), move slowly between positions.", true, 9, "Cat-Cow Stretch", 7, 2, 4, null, "[]" },
                    { new Guid("99999999-9999-9999-9999-999999999995"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A yoga-inspired stretch that targets the hip flexors and glutes.", "From plank, bring one knee forward to same-side wrist, extend other leg back, hold and breathe.", true, 9, "Pigeon Pose", 10, 7, 4, null, "[]" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A full-body exercise combining front squat and overhead press in one movement.", "Hold weight at shoulder height, squat down, drive up explosively and press weight overhead.", true, 3, "Thruster", 12, 3, 1, null, "[]" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A complex full-body movement that improves stability, strength, and coordination.", "Lie on back holding weight overhead, get up to standing position through series of movements, reverse to return.", true, 6, "Turkish Get-Up", 12, 7, 1, null, "[]" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaac"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A compound exercise combining plank and rowing motion for core and back.", "In plank position with dumbbells, row one weight to side while maintaining plank, alternate sides.", true, 2, "Renegade Row", 2, 7, 1, null, "[]" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaad"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A dynamic core and cardio exercise performed in plank position.", "In plank position, alternate bringing knees to chest rapidly, keep core engaged throughout.", true, 8, "Mountain Climbers", 7, 13, 3, null, "[]" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaae"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "An isolation exercise targeting the calf muscles.", "Stand on balls of feet, raise up onto toes, lower with control, can add weight for resistance.", true, 1, "Calf Raises", 11, null, 1, null, "[]" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedPrograms_AthleteId",
                table: "AssignedPrograms",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignedPrograms_IsActive",
                table: "AssignedPrograms",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_Email",
                table: "Athletes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Athletes_Username",
                table: "Athletes",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompletedSets_ExerciseInstanceId",
                table: "CompletedSets",
                column: "ExerciseInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseDefinitions_Name",
                table: "ExerciseDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseDefinitions_PrimaryMuscleGroup",
                table: "ExerciseDefinitions",
                column: "PrimaryMuscleGroup");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseDefinitions_Type",
                table: "ExerciseDefinitions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseInstances_WorkoutSessionId",
                table: "ExerciseInstances",
                column: "WorkoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSet_WorkoutExerciseId",
                table: "ExerciseSet",
                column: "WorkoutExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeWeeks_AssignedProgramId",
                table: "ProgrammeWeeks",
                column: "AssignedProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeWeeks_AssignedProgramId_WeekNumber",
                table: "ProgrammeWeeks",
                columns: new[] { "AssignedProgramId", "WeekNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDay_ProgrammeWeekId",
                table: "WorkoutDay",
                column: "ProgrammeWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercise_ExerciseId",
                table: "WorkoutExercise",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercise_WorkoutDayId",
                table: "WorkoutExercise",
                column: "WorkoutDayId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_AthleteId",
                table: "WorkoutSessions",
                column: "AthleteId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_ProgrammeWeekId",
                table: "WorkoutSessions",
                column: "ProgrammeWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_ScheduledDate",
                table: "WorkoutSessions",
                column: "ScheduledDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Athletes");

            migrationBuilder.DropTable(
                name: "CompletedSets");

            migrationBuilder.DropTable(
                name: "ExerciseDefinitions");

            migrationBuilder.DropTable(
                name: "ExerciseSet");

            migrationBuilder.DropTable(
                name: "ExerciseInstances");

            migrationBuilder.DropTable(
                name: "WorkoutExercise");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");

            migrationBuilder.DropTable(
                name: "Exercise");

            migrationBuilder.DropTable(
                name: "WorkoutDay");

            migrationBuilder.DropTable(
                name: "ProgrammeWeeks");

            migrationBuilder.DropTable(
                name: "AssignedPrograms");
        }
    }
}
