using System;
using System.Data.SqlTypes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProgramTemplateAddCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ProgramTemplates",
                type: "uuid",
                nullable: true,
                defaultValue: null);

            migrationBuilder.AddColumn<bool>(
                name: "IsUserGenerated",
                table: "ProgramTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AiGenerationJobs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProgramTemplates");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AiGenerationJobs");
        }
    }
}
