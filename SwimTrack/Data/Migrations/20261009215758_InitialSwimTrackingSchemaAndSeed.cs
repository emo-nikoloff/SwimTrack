using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SwimTrack.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSwimTrackingSchemaAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    StartDate = table.Column<DateTime>(type: "DATE", nullable: false),
                    EndDate = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlans", x => x.Id);
                    table.CheckConstraint("CK_TrainingPlan_ValidDateRange", "[EndDate] >= [StartDate]");
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "DATE", nullable: false),
                    PlannedDistance = table.Column<int>(type: "int", nullable: false),
                    PlannedDuration = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TrainingPlanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                    table.CheckConstraint("CK_Workout_PlannedDistance_Positive", "[PlannedDistance] > 0");
                    table.CheckConstraint("CK_Workout_PlannedDuration_Positive", "[PlannedDuration] > 0");
                    table.ForeignKey(
                        name: "FK_Workouts_TrainingPlans_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "TrainingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActualDistance = table.Column<int>(type: "int", nullable: false),
                    ActualDuration = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WorkoutId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutResults", x => x.Id);
                    table.CheckConstraint("CK_WorkoutResult_ActualDistance_Positive", "[ActualDistance] > 0");
                    table.CheckConstraint("CK_WorkoutResult_ActualDuration_Positive", "[ActualDuration] > 0");
                    table.ForeignKey(
                        name: "FK_WorkoutResults_Workouts_WorkoutId",
                        column: x => x.WorkoutId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TrainingPlans",
                columns: new[] { "Id", "EndDate", "Goal", "Name", "StartDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Изграждане на аеробна издръжливост и подобряване на техниката", "Основна издръжливост", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Подготовка за плуване на 1500 метра под 30 минути", "Подготовка за 1500 м", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Workouts",
                columns: new[] { "Id", "Date", "Notes", "PlannedDistance", "PlannedDuration", "Status", "TrainingPlanId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Фокус върху свободен стил и спокойно темпо.", 2000, 50, 1, 1 },
                    { 2, new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Технически упражнения с дъска и плавници.", 2200, 55, 1, 1 },
                    { 3, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Възстановително плуване.", 1800, 45, 2, 1 },
                    { 4, new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Интервали 10 x 100 м със стабилно темпо.", 2500, 60, 1, 2 },
                    { 5, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Дълга тренировка с акцент върху издръжливостта.", 3000, 70, 0, 2 }
                });

            migrationBuilder.InsertData(
                table: "WorkoutResults",
                columns: new[] { "Id", "ActualDistance", "ActualDuration", "CompletedAt", "Notes", "WorkoutId" },
                values: new object[,]
                {
                    { 1, 2000, 48, new DateTime(2026, 1, 6, 18, 30, 0, 0, DateTimeKind.Unspecified), "Тренировката беше изпълнена с равномерно темпо.", 1 },
                    { 2, 2200, 57, new DateTime(2026, 1, 8, 18, 45, 0, 0, DateTimeKind.Unspecified), "Последните 400 метра бяха с по-висока интензивност.", 2 },
                    { 3, 2500, 59, new DateTime(2026, 2, 3, 19, 0, 0, 0, DateTimeKind.Unspecified), "Интервалите бяха изпълнени по план.", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutResults_WorkoutId",
                table: "WorkoutResults",
                column: "WorkoutId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_TrainingPlanId",
                table: "Workouts",
                column: "TrainingPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkoutResults");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropTable(
                name: "TrainingPlans");
        }
    }
}
