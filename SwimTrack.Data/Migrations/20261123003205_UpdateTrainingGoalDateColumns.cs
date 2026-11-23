using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwimTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrainingGoalDateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TrainingGoal_ValidDateRange",
                table: "TrainingGoal");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "TrainingGoal",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "TrainingGoal",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TrainingGoal_ValidDateRange",
                table: "TrainingGoal",
                sql: "[EndDate] >= [StartDate]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TrainingGoal_ValidDateRange",
                table: "TrainingGoal");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "TrainingGoal",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "TrainingGoal",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TrainingGoal_ValidDateRange",
                table: "TrainingGoal",
                sql: "[EndDate] >= [StartDate]");
        }
    }
}
