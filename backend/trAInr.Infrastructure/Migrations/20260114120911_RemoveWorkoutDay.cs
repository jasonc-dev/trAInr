using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkoutDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "WorkoutDays");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "WorkoutDays",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
