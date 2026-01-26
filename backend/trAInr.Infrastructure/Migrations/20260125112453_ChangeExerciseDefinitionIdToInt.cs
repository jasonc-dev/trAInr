using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace trAInr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeExerciseDefinitionIdToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop foreign key constraints using SQL to handle truncated names
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_WorkoutExercises_ExerciseDefinitions_ExerciseDefinitionId'
                        AND table_name = 'WorkoutExercises'
                    ) THEN
                        ALTER TABLE ""WorkoutExercises"" DROP CONSTRAINT ""FK_WorkoutExercises_ExerciseDefinitions_ExerciseDefinitionId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    constraint_name_var text;
                BEGIN
                    SELECT constraint_name INTO constraint_name_var
                    FROM information_schema.table_constraints 
                    WHERE constraint_name LIKE 'FK_ProgramTemplateWorkoutExercises_ExerciseDefinitions%'
                    AND table_name = 'ProgramTemplateWorkoutExercises'
                    LIMIT 1;
                    
                    IF constraint_name_var IS NOT NULL THEN
                        EXECUTE format('ALTER TABLE ""ProgramTemplateWorkoutExercises"" DROP CONSTRAINT %I', constraint_name_var);
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.table_constraints 
                        WHERE constraint_name = 'FK_ExerciseInstances_ExerciseDefinitions_ExerciseDefinitionId'
                        AND table_name = 'ExerciseInstances'
                    ) THEN
                        ALTER TABLE ""ExerciseInstances"" DROP CONSTRAINT ""FK_ExerciseInstances_ExerciseDefinitions_ExerciseDefinitionId"";
                    END IF;
                END $$;
            ");

            // Step 2: Create temporary integer columns
            migrationBuilder.AddColumn<int>(
                name: "IdTemp",
                table: "ExerciseDefinitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseDefinitionIdTemp",
                table: "WorkoutExercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseDefinitionIdTemp",
                table: "ProgramTemplateWorkoutExercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseDefinitionIdTemp",
                table: "ExerciseInstances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Step 3: Populate temporary integer IDs in ExerciseDefinitions
            migrationBuilder.Sql(@"
                WITH numbered AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (ORDER BY ""CreatedAt"") as new_id
                    FROM ""ExerciseDefinitions""
                )
                UPDATE ""ExerciseDefinitions"" ed
                SET ""IdTemp"" = n.new_id
                FROM numbered n
                WHERE ed.""Id"" = n.""Id"";
            ");

            // Step 4: Update foreign key columns in dependent tables
            migrationBuilder.Sql(@"
                UPDATE ""WorkoutExercises"" we
                SET ""ExerciseDefinitionIdTemp"" = ed.""IdTemp""
                FROM ""ExerciseDefinitions"" ed
                WHERE we.""ExerciseDefinitionId""::uuid = ed.""Id"";
            ");

            migrationBuilder.Sql(@"
                UPDATE ""ProgramTemplateWorkoutExercises"" ptwe
                SET ""ExerciseDefinitionIdTemp"" = ed.""IdTemp""
                FROM ""ExerciseDefinitions"" ed
                WHERE ptwe.""ExerciseDefinitionId""::uuid = ed.""Id"";
            ");

            migrationBuilder.Sql(@"
                UPDATE ""ExerciseInstances"" ei
                SET ""ExerciseDefinitionIdTemp"" = ed.""IdTemp""
                FROM ""ExerciseDefinitions"" ed
                WHERE ei.""ExerciseDefinitionId""::uuid = ed.""Id"";
            ");

            // Step 5: Drop old UUID columns and indexes (conditionally)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE indexname = 'IX_WorkoutExercises_ExerciseDefinitionId'
                        AND tablename = 'WorkoutExercises'
                    ) THEN
                        DROP INDEX ""IX_WorkoutExercises_ExerciseDefinitionId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE indexname = 'IX_ProgramTemplateWorkoutExercises_ExerciseDefinitionId'
                        AND tablename = 'ProgramTemplateWorkoutExercises'
                    ) THEN
                        DROP INDEX ""IX_ProgramTemplateWorkoutExercises_ExerciseDefinitionId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM pg_indexes 
                        WHERE indexname = 'IX_ExerciseInstances_ExerciseDefinitionId'
                        AND tablename = 'ExerciseInstances'
                    ) THEN
                        DROP INDEX ""IX_ExerciseInstances_ExerciseDefinitionId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.DropColumn(
                name: "ExerciseDefinitionId",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseDefinitionId",
                table: "ProgramTemplateWorkoutExercises");

            migrationBuilder.DropColumn(
                name: "ExerciseDefinitionId",
                table: "ExerciseInstances");

            // Step 6: Rename temporary columns to final names
            migrationBuilder.RenameColumn(
                name: "ExerciseDefinitionIdTemp",
                table: "WorkoutExercises",
                newName: "ExerciseDefinitionId");

            migrationBuilder.RenameColumn(
                name: "ExerciseDefinitionIdTemp",
                table: "ProgramTemplateWorkoutExercises",
                newName: "ExerciseDefinitionId");

            migrationBuilder.RenameColumn(
                name: "ExerciseDefinitionIdTemp",
                table: "ExerciseInstances",
                newName: "ExerciseDefinitionId");

            // Step 7: Replace the Id column in ExerciseDefinitions
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseDefinitions",
                table: "ExerciseDefinitions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExerciseDefinitions");

            migrationBuilder.RenameColumn(
                name: "IdTemp",
                table: "ExerciseDefinitions",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseDefinitions",
                table: "ExerciseDefinitions",
                column: "Id");

            // Step 8: Set up identity column
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Remove any default value first
                    ALTER TABLE ""ExerciseDefinitions"" ALTER COLUMN ""Id"" DROP DEFAULT;
                    
                    -- Add identity generation if not already present
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_attribute 
                        WHERE attrelid = 'ExerciseDefinitions'::regclass 
                        AND attname = 'Id' 
                        AND attidentity != ''
                    ) THEN
                        ALTER TABLE ""ExerciseDefinitions""
                        ALTER COLUMN ""Id"" ADD GENERATED BY DEFAULT AS IDENTITY;
                    END IF;
                EXCEPTION
                    WHEN OTHERS THEN
                        -- If default doesn't exist or identity already exists, continue
                        NULL;
                END $$;
            ");

            // Step 9: Recreate indexes
            migrationBuilder.CreateIndex(
                name: "IX_WorkoutExercises_ExerciseDefinitionId",
                table: "WorkoutExercises",
                column: "ExerciseDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramTemplateWorkoutExercises_ExerciseDefinitionId",
                table: "ProgramTemplateWorkoutExercises",
                column: "ExerciseDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseInstances_ExerciseDefinitionId",
                table: "ExerciseInstances",
                column: "ExerciseDefinitionId");

            // Step 10: Recreate foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_ExerciseDefinitions_ExerciseDefinitionId",
                table: "WorkoutExercises",
                column: "ExerciseDefinitionId",
                principalTable: "ExerciseDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramTemplateWorkoutExercises_ExerciseDefinitions_ExerciseDefinitionId",
                table: "ProgramTemplateWorkoutExercises",
                column: "ExerciseDefinitionId",
                principalTable: "ExerciseDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseInstances_ExerciseDefinitions_ExerciseDefinitionId",
                table: "ExerciseInstances",
                column: "ExerciseDefinitionId",
                principalTable: "ExerciseDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ExerciseDefinitionId",
                table: "WorkoutExercises",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExerciseDefinitionId",
                table: "ProgramTemplateWorkoutExercises",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExerciseDefinitionId",
                table: "ExerciseInstances",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ExerciseDefinitions",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
