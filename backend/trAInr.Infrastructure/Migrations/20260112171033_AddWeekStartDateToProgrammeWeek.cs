using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWeekStartDateToProgrammeWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add column as nullable first
            migrationBuilder.AddColumn<DateOnly>(
                name: "WeekStartDate",
                table: "ProgrammeWeeks",
                type: "date",
                nullable: true);

            // Update existing rows: calculate WeekStartDate from AssignedProgram.StartDate and WeekNumber
            migrationBuilder.Sql(@"
                UPDATE ""ProgrammeWeeks"" pw
                SET ""WeekStartDate"" = ap.""StartDate"" + INTERVAL '1 day' * ((pw.""WeekNumber"" - 1) * 7)
                FROM ""AssignedPrograms"" ap
                WHERE pw.""AssignedProgramId"" = ap.""Id"";
            ");

            // Make column non-nullable
            migrationBuilder.Sql(@"
                ALTER TABLE ""ProgrammeWeeks""
                ALTER COLUMN ""WeekStartDate"" SET NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekStartDate",
                table: "ProgrammeWeeks");
        }
    }
}
