using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkoutExerciseFkToExerciseDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises");

            migrationBuilder.RenameColumn(
                name: "ExerciseId",
                table: "WorkoutExercises",
                newName: "ExerciseDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercises_ExerciseId",
                table: "WorkoutExercises",
                newName: "IX_WorkoutExercises_ExerciseDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_ExerciseDefinitions_ExerciseDefinitionId",
                table: "WorkoutExercises",
                column: "ExerciseDefinitionId",
                principalTable: "ExerciseDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_ExerciseDefinitions_ExerciseDefinitionId",
                table: "WorkoutExercises");

            migrationBuilder.RenameColumn(
                name: "ExerciseDefinitionId",
                table: "WorkoutExercises",
                newName: "ExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkoutExercises_ExerciseDefinitionId",
                table: "WorkoutExercises",
                newName: "IX_WorkoutExercises_ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Exercises_ExerciseId",
                table: "WorkoutExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
