using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace trAInr.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    PrimaryMuscleGroup = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondaryMuscleGroup = table.Column<int>(type: "INTEGER", nullable: true),
                    Instructions = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    VideoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsSystemExercise = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FitnessLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    PrimaryGoal = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkoutDaysPerWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programmes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DurationWeeks = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPreMade = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programmes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammeWeeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProgrammeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WeekNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammeWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgrammeWeeks_Programmes_ProgrammeId",
                        column: x => x.ProgrammeId,
                        principalTable: "Programmes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProgrammeWeekId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRestDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutDays_ProgrammeWeeks_ProgrammeWeekId",
                        column: x => x.ProgrammeWeekId,
                        principalTable: "ProgrammeWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WorkoutDayId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TargetSets = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetReps = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    TargetDurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetDistance = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutExercises_WorkoutDays_WorkoutDayId",
                        column: x => x.WorkoutDayId,
                        principalTable: "WorkoutDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WorkoutExerciseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SetNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: true),
                    Weight = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    Distance = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: true),
                    Intensity = table.Column<int>(type: "INTEGER", nullable: true),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsWarmup = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseSets_WorkoutExercises_WorkoutExerciseId",
                        column: x => x.WorkoutExerciseId,
                        principalTable: "WorkoutExercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "Description", "Instructions", "IsSystemExercise", "Name", "PrimaryMuscleGroup", "SecondaryMuscleGroup", "Type", "VideoUrl" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(1800), null, "Barbell bench press for chest development", null, true, "Bench Press", 1, 5, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111102"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5630), null, "Incline press targeting upper chest", null, true, "Incline Dumbbell Press", 1, 3, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111103"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5660), null, "Cable chest fly for isolation", null, true, "Cable Fly", 1, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111104"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5660), null, "Bodyweight chest exercise", null, true, "Push-ups", 1, 5, 3, null },
                    { new Guid("11111111-1111-1111-1111-111111111201"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5660), null, "Conventional deadlift for overall back development", null, true, "Deadlift", 2, 9, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111202"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5680), null, "Bodyweight back exercise", null, true, "Pull-ups", 2, 4, 3, null },
                    { new Guid("11111111-1111-1111-1111-111111111203"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5680), null, "Bent-over barbell row", null, true, "Barbell Row", 2, 4, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111204"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5680), null, "Cable lat pulldown", null, true, "Lat Pulldown", 2, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111301"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5680), null, "Standing barbell overhead press", null, true, "Overhead Press", 3, 5, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111302"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5730), null, "Dumbbell lateral raise", null, true, "Lateral Raise", 3, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111303"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5740), null, "Cable face pull for rear delts", null, true, "Face Pull", 3, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111401"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5740), null, "Standing barbell bicep curl", null, true, "Barbell Curl", 4, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111402"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5740), null, "Cable tricep pushdown", null, true, "Tricep Pushdown", 5, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111403"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5740), null, "Dumbbell hammer curl", null, true, "Hammer Curl", 4, 6, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111404"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5750), null, "Lying tricep extension", null, true, "Skull Crushers", 5, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111501"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5750), null, "Back squat for leg development", null, true, "Barbell Squat", 8, 10, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111502"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5750), null, "RDL for hamstring development", null, true, "Romanian Deadlift", 9, 10, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111503"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5750), null, "Machine leg press", null, true, "Leg Press", 8, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111504"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5750), null, "Lying leg curl", null, true, "Leg Curl", 9, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111505"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5760), null, "Standing calf raise", null, true, "Calf Raise", 11, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111506"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5760), null, "Walking or stationary lunges", null, true, "Lunges", 8, 10, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111601"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5760), null, "Isometric core exercise", null, true, "Plank", 7, null, 3, null },
                    { new Guid("11111111-1111-1111-1111-111111111602"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5760), null, "Weighted cable crunch", null, true, "Cable Crunch", 7, null, 1, null },
                    { new Guid("11111111-1111-1111-1111-111111111603"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5760), null, "Core exercise for lower abs", null, true, "Hanging Leg Raise", 7, null, 3, null },
                    { new Guid("11111111-1111-1111-1111-111111111701"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5760), null, "Outdoor or treadmill running", null, true, "Running", 13, null, 2, null },
                    { new Guid("11111111-1111-1111-1111-111111111702"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5770), null, "Stationary or outdoor cycling", null, true, "Cycling", 13, null, 2, null },
                    { new Guid("11111111-1111-1111-1111-111111111703"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5770), null, "Rowing machine", null, true, "Rowing", 13, 2, 2, null },
                    { new Guid("11111111-1111-1111-1111-111111111704"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5770), null, "Swimming laps", null, true, "Swimming", 12, null, 2, null },
                    { new Guid("11111111-1111-1111-1111-111111111705"), new DateTime(2025, 11, 27, 22, 53, 15, 468, DateTimeKind.Utc).AddTicks(5770), null, "Skipping rope for cardio", null, true, "Jump Rope", 13, null, 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Name",
                table: "Exercises",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_PrimaryMuscleGroup",
                table: "Exercises",
                column: "PrimaryMuscleGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Type",
                table: "Exercises",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_CompletedAt",
                table: "ExerciseSets",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_WorkoutExerciseId",
                table: "ExerciseSets",
                column: "WorkoutExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_IsActive",
                table: "Programmes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Programmes_UserId",
                table: "Programmes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammeWeeks_ProgrammeId_WeekNumber",
                table: "ProgrammeWeeks",
                columns: new[] { "ProgrammeId", "WeekNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDays_ProgrammeWeekId",
                table: "WorkoutDays",
                column: "ProgrammeWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDays_ScheduledDate",
                table: "WorkoutDays",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_WorkoutDayId_OrderIndex",
                table: "WorkoutExercises",
                columns: new[] { "WorkoutDayId", "OrderIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseSets");

            migrationBuilder.DropTable(
                name: "WorkoutExercises");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "WorkoutDays");

            migrationBuilder.DropTable(
                name: "ProgrammeWeeks");

            migrationBuilder.DropTable(
                name: "Programmes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
