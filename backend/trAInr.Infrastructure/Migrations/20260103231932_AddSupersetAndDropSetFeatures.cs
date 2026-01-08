using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupersetAndDropSetFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SupersetGroupId",
                table: "WorkoutExercises",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupersetRestSeconds",
                table: "WorkoutExercises",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DropPercentage",
                table: "ExerciseSets",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetType",
                table: "ExerciseSets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Migrate existing IsWarmup=true rows to SetType=Warmup (1)
            migrationBuilder.Sql(
                @"UPDATE ""ExerciseSets"" 
                  SET ""SetType"" = 1 
                  WHERE ""IsWarmup"" = true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupersetGroupId",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "SupersetRestSeconds",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "DropPercentage",
                table: "ExerciseSets");

            migrationBuilder.DropColumn(
                name: "SetType",
                table: "ExerciseSets");
        }
    }
}
