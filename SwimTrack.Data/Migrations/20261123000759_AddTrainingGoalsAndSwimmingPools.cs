using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwimTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingGoalsAndSwimmingPools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "WorkoutResults");

            migrationBuilder.DropColumn(
                name: "Goal",
                table: "TrainingPlans");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TrainingPlans",
                newName: "Title");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Workouts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "WorkoutResults",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SwimmingPoolId",
                table: "WorkoutResults",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "TrainingPlans",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "TrainingPlans",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AddColumn<int>(
                name: "SwimmingPoolId",
                table: "TrainingPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrainingGoalId",
                table: "TrainingPlans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SwimmingPool",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Length = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwimmingPool", x => x.Id);
                    table.CheckConstraint("CK_SwimmingPool_Length_Positive", "[Length] > 0");
                    table.ForeignKey(
                        name: "FK_SwimmingPool_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingGoal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetDistance = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingGoal", x => x.Id);
                    table.CheckConstraint("CK_TrainingGoal_TargetDistance_Positive", "[TargetDistance] > 0");
                    table.CheckConstraint("CK_TrainingGoal_ValidDateRange", "[EndDate] >= [StartDate]");
                    table.ForeignKey(
                        name: "FK_TrainingGoal_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutResults_SwimmingPoolId",
                table: "WorkoutResults",
                column: "SwimmingPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlans_SwimmingPoolId",
                table: "TrainingPlans",
                column: "SwimmingPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlans_TrainingGoalId",
                table: "TrainingPlans",
                column: "TrainingGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_SwimmingPool_UserId",
                table: "SwimmingPool",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGoal_UserId",
                table: "TrainingGoal",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlans_SwimmingPool_SwimmingPoolId",
                table: "TrainingPlans",
                column: "SwimmingPoolId",
                principalTable: "SwimmingPool",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlans_TrainingGoal_TrainingGoalId",
                table: "TrainingPlans",
                column: "TrainingGoalId",
                principalTable: "TrainingGoal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutResults_SwimmingPool_SwimmingPoolId",
                table: "WorkoutResults",
                column: "SwimmingPoolId",
                principalTable: "SwimmingPool",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlans_SwimmingPool_SwimmingPoolId",
                table: "TrainingPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlans_TrainingGoal_TrainingGoalId",
                table: "TrainingPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutResults_SwimmingPool_SwimmingPoolId",
                table: "WorkoutResults");

            migrationBuilder.DropTable(
                name: "SwimmingPool");

            migrationBuilder.DropTable(
                name: "TrainingGoal");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutResults_SwimmingPoolId",
                table: "WorkoutResults");

            migrationBuilder.DropIndex(
                name: "IX_TrainingPlans_SwimmingPoolId",
                table: "TrainingPlans");

            migrationBuilder.DropIndex(
                name: "IX_TrainingPlans_TrainingGoalId",
                table: "TrainingPlans");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "WorkoutResults");

            migrationBuilder.DropColumn(
                name: "SwimmingPoolId",
                table: "WorkoutResults");

            migrationBuilder.DropColumn(
                name: "SwimmingPoolId",
                table: "TrainingPlans");

            migrationBuilder.DropColumn(
                name: "TrainingGoalId",
                table: "TrainingPlans");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "TrainingPlans",
                newName: "Name");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Workouts",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "WorkoutResults",
                type: "DATE",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "TrainingPlans",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "TrainingPlans",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Goal",
                table: "TrainingPlans",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
