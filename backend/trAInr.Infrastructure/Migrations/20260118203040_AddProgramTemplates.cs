using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProgramTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DurationWeeks = table.Column<int>(type: "integer", nullable: false),
                    ExperienceLevel = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramTemplateWeeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTemplateWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramTemplateWeeks_ProgramTemplates_ProgramTemplateId",
                        column: x => x.ProgramTemplateId,
                        principalTable: "ProgramTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramTemplateWorkoutDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramTemplateWeekId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsRestDay = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTemplateWorkoutDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramTemplateWorkoutDays_ProgramTemplateWeeks_ProgramTemp~",
                        column: x => x.ProgramTemplateWeekId,
                        principalTable: "ProgramTemplateWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramTemplateWorkoutExercises",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgramTemplateWorkoutDayId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TargetSets = table.Column<int>(type: "integer", nullable: false),
                    TargetReps = table.Column<int>(type: "integer", nullable: false),
                    TargetWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    TargetDurationSeconds = table.Column<int>(type: "integer", nullable: true),
                    TargetDistance = table.Column<decimal>(type: "numeric", nullable: true),
                    RestSeconds = table.Column<int>(type: "integer", nullable: true),
                    SupersetGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupersetRestSeconds = table.Column<int>(type: "integer", nullable: true),
                    TargetRpe = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramTemplateWorkoutExercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramTemplateWorkoutExercises_ExerciseDefinitions_Exercis~",
                        column: x => x.ExerciseDefinitionId,
                        principalTable: "ExerciseDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramTemplateWorkoutExercises_ProgramTemplateWorkoutDays_~",
                        column: x => x.ProgramTemplateWorkoutDayId,
                        principalTable: "ProgramTemplateWorkoutDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTemplates_ExperienceLevel",
                table: "ProgramTemplates",
                column: "ExperienceLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTemplates_Name",
                table: "ProgramTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTemplateWeeks_ProgramTemplateId_WeekNumber",
                table: "ProgramTemplateWeeks",
                columns: new[] { "ProgramTemplateId", "WeekNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTemplateWorkoutDays_ProgramTemplateWeekId",
                table: "ProgramTemplateWorkoutDays",
                column: "ProgramTemplateWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTemplateWorkoutExercises_ExerciseDefinitionId",
                table: "ProgramTemplateWorkoutExercises",
                column: "ExerciseDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTemplateWorkoutExercises_ProgramTemplateWorkoutDayId",
                table: "ProgramTemplateWorkoutExercises",
                column: "ProgramTemplateWorkoutDayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramTemplateWorkoutExercises");

            migrationBuilder.DropTable(
                name: "ProgramTemplateWorkoutDays");

            migrationBuilder.DropTable(
                name: "ProgramTemplateWeeks");

            migrationBuilder.DropTable(
                name: "ProgramTemplates");
        }
    }
}
