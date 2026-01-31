using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aliases",
                table: "ExerciseDefinitions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DefaultRepMax",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultRepMin",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultRestMaxSec",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultRestMinSec",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBodyweight",
                table: "ExerciseDefinitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnilateral",
                table: "ExerciseDefinitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LoadType",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresOverheadPosition",
                table: "ExerciseDefinitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SetupComplexity",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShortCue",
                table: "ExerciseDefinitions",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpinalLoad",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimePerSetEstimateSec",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrackingMode",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsPortable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseVariants",
                columns: table => new
                {
                    ExerciseDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    VariantOfExerciseDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    VariantType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseVariants", x => new { x.ExerciseDefinitionId, x.VariantOfExerciseDefinitionId, x.VariantType });
                    table.ForeignKey(
                        name: "FK_ExerciseVariants_ExerciseDefinitions_ExerciseDefinitionId",
                        column: x => x.ExerciseDefinitionId,
                        principalTable: "ExerciseDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseVariants_ExerciseDefinitions_VariantOfExerciseDefin~",
                        column: x => x.VariantOfExerciseDefinitionId,
                        principalTable: "ExerciseDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Muscles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Group = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Namespace = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseEquipments",
                columns: table => new
                {
                    ExerciseDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseEquipments", x => new { x.ExerciseDefinitionId, x.EquipmentId });
                    table.ForeignKey(
                        name: "FK_ExerciseEquipments_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExerciseEquipments_ExerciseDefinitions_ExerciseDefinitionId",
                        column: x => x.ExerciseDefinitionId,
                        principalTable: "ExerciseDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseMuscles",
                columns: table => new
                {
                    ExerciseDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    MuscleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Contribution = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseMuscles", x => new { x.ExerciseDefinitionId, x.MuscleId, x.Role });
                    table.ForeignKey(
                        name: "FK_ExerciseMuscles_ExerciseDefinitions_ExerciseDefinitionId",
                        column: x => x.ExerciseDefinitionId,
                        principalTable: "ExerciseDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseMuscles_Muscles_MuscleId",
                        column: x => x.MuscleId,
                        principalTable: "Muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseTags",
                columns: table => new
                {
                    ExerciseDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTags", x => new { x.ExerciseDefinitionId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ExerciseTags_ExerciseDefinitions_ExerciseDefinitionId",
                        column: x => x.ExerciseDefinitionId,
                        principalTable: "ExerciseDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseDefinitions_SpinalLoad_SetupComplexity_TrackingMode",
                table: "ExerciseDefinitions",
                columns: new[] { "SpinalLoad", "SetupComplexity", "TrackingMode" });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Category",
                table: "Equipment",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Name",
                table: "Equipment",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseEquipments_EquipmentId",
                table: "ExerciseEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseMuscles_MuscleId_Role",
                table: "ExerciseMuscles",
                columns: new[] { "MuscleId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTags_TagId",
                table: "ExerciseTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseVariants_VariantOfExerciseDefinitionId",
                table: "ExerciseVariants",
                column: "VariantOfExerciseDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Muscles_Group",
                table: "Muscles",
                column: "Group");

            migrationBuilder.CreateIndex(
                name: "IX_Muscles_Name",
                table: "Muscles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Namespace_Name",
                table: "Tags",
                columns: new[] { "Namespace", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseEquipments");

            migrationBuilder.DropTable(
                name: "ExerciseMuscles");

            migrationBuilder.DropTable(
                name: "ExerciseTags");

            migrationBuilder.DropTable(
                name: "ExerciseVariants");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "Muscles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseDefinitions_SpinalLoad_SetupComplexity_TrackingMode",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "Aliases",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "DefaultRepMax",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "DefaultRepMin",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "DefaultRestMaxSec",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "DefaultRestMinSec",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "IsBodyweight",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "IsUnilateral",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "LoadType",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "RequiresOverheadPosition",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "SetupComplexity",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "ShortCue",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "SpinalLoad",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "TimePerSetEstimateSec",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "TrackingMode",
                table: "ExerciseDefinitions");
        }
    }
}
