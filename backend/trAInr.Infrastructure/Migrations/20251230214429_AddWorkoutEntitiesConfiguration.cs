using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutEntitiesConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSet_WorkoutExercise_WorkoutExerciseId",
                table: "ExerciseSet");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutDay_ProgrammeWeeks_ProgrammeWeekId",
                table: "WorkoutDay");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercise_Exercise_ExerciseId",
                table: "WorkoutExercise");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercise_WorkoutDay_WorkoutDayId",
                table: "WorkoutExercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutExercise",
                table: "WorkoutExercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutDay",
                table: "WorkoutDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseSet",
                table: "ExerciseSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercise",
                table: "Exercise");

            migrationBuilder.RenameTable(
                name: "WorkoutExercise",
                newName: "WorkoutExercises");

            migrationBuilder.RenameTable(
                name: "WorkoutDay",
                newName: "WorkoutDays");

            migrationBuilder.RenameTable(
                name: "ExerciseSet",
                newName: "ExerciseSets");

            migrationBuilder.RenameTable(
                name: "Exercise",
                newName: "Exercises");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercise_WorkoutDayId",
                table: "WorkoutExercises",
                newName: "IX_WorkoutExercises_WorkoutDayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercise_ExerciseId",
                table: "WorkoutExercises",
                newName: "IX_WorkoutExercises_ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutDay_ProgrammeWeekId",
                table: "WorkoutDays",
                newName: "IX_WorkoutDays_ProgrammeWeekId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseSet_WorkoutExerciseId",
                table: "ExerciseSets",
                newName: "IX_ExerciseSets_WorkoutExerciseId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TargetWeight",
                table: "WorkoutExercises",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TargetDistance",
                table: "WorkoutExercises",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "WorkoutExercises",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WorkoutDays",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WorkoutDays",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "ExerciseSets",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ExerciseSets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Distance",
                table: "ExerciseSets",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                table: "Exercises",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exercises",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Exercises",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Exercises",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutDays",
                table: "WorkoutDays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseSets",
                table: "ExerciseSets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_Name",
                table: "Exercises",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSets_WorkoutExercises_WorkoutExerciseId",
                table: "ExerciseSets",
                column: "WorkoutExerciseId",
                principalTable: "WorkoutExercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutDays_ProgrammeWeeks_ProgrammeWeekId",
                table: "WorkoutDays",
                column: "ProgrammeWeekId",
                principalTable: "ProgrammeWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_WorkoutDays_WorkoutDayId",
                table: "WorkoutExercises",
                column: "WorkoutDayId",
                principalTable: "WorkoutDays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSets_WorkoutExercises_WorkoutExerciseId",
                table: "ExerciseSets");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutDays_ProgrammeWeeks_ProgrammeWeekId",
                table: "WorkoutDays");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_WorkoutDays_WorkoutDayId",
                table: "WorkoutExercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutExercises",
                table: "WorkoutExercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutDays",
                table: "WorkoutDays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseSets",
                table: "ExerciseSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_Name",
                table: "Exercises");

            migrationBuilder.RenameTable(
                name: "WorkoutExercises",
                newName: "WorkoutExercise");

            migrationBuilder.RenameTable(
                name: "WorkoutDays",
                newName: "WorkoutDay");

            migrationBuilder.RenameTable(
                name: "ExerciseSets",
                newName: "ExerciseSet");

            migrationBuilder.RenameTable(
                name: "Exercises",
                newName: "Exercise");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercises_WorkoutDayId",
                table: "WorkoutExercise",
                newName: "IX_WorkoutExercise_WorkoutDayId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercise",
                newName: "IX_WorkoutExercise_ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutDays_ProgrammeWeekId",
                table: "WorkoutDay",
                newName: "IX_WorkoutDay_ProgrammeWeekId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseSets_WorkoutExerciseId",
                table: "ExerciseSet",
                newName: "IX_ExerciseSet_WorkoutExerciseId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TargetWeight",
                table: "WorkoutExercise",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TargetDistance",
                table: "WorkoutExercise",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "WorkoutExercise",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WorkoutDay",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "WorkoutDay",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "ExerciseSet",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ExerciseSet",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Distance",
                table: "ExerciseSet",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                table: "Exercise",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exercise",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Exercise",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Exercise",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutExercise",
                table: "WorkoutExercise",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutDay",
                table: "WorkoutDay",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseSet",
                table: "ExerciseSet",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercise",
                table: "Exercise",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSet_WorkoutExercise_WorkoutExerciseId",
                table: "ExerciseSet",
                column: "WorkoutExerciseId",
                principalTable: "WorkoutExercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutDay_ProgrammeWeeks_ProgrammeWeekId",
                table: "WorkoutDay",
                column: "ProgrammeWeekId",
                principalTable: "ProgrammeWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercise_Exercise_ExerciseId",
                table: "WorkoutExercise",
                column: "ExerciseId",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercise_WorkoutDay_WorkoutDayId",
                table: "WorkoutExercise",
                column: "WorkoutDayId",
                principalTable: "WorkoutDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
