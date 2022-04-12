using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class Trainings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingRestorations",
                schema: "instances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorId = table.Column<int>(type: "int", nullable: false),
                    apiKeyStorableId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    instanceDuplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    environmentId = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    commentExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    anonymize = table.Column<bool>(type: "bit", nullable: false),
                    restoreFiles = table.Column<bool>(type: "bit", nullable: false),
                    keepExistingTrainingPassword = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingRestorations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingRestorations_InstanceDuplications_instanceDuplicationId",
                        column: x => x.instanceDuplicationId,
                        principalSchema: "instances",
                        principalTable: "InstanceDuplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });


            migrationBuilder.CreateTable(
                name: "Trainings",
                schema: "instances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    environmentId = table.Column<int>(type: "int", nullable: false),
                    instanceId = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    lastRestoredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    authorId = table.Column<int>(type: "int", nullable: false),
                    apiKeyStorableId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trainingRestorationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trainings_Environments_environmentId",
                        column: x => x.environmentId,
                        principalSchema: "dbo",
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trainings_Instances_instanceId",
                        column: x => x.instanceId,
                        principalSchema: "dbo",
                        principalTable: "NewInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trainings_TrainingRestorations_trainingRestorationId",
                        column: x => x.trainingRestorationId,
                        principalSchema: "instances",
                        principalTable: "TrainingRestorations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trainings_Users_authorId",
                        column: x => x.authorId,
                        principalSchema: "shared",
                        principalTable: "Users",
                        principalColumn: "PartenairesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingRestorations_instanceDuplicationId",
                schema: "instances",
                table: "TrainingRestorations",
                column: "instanceDuplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_authorId",
                schema: "instances",
                table: "Trainings",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_environmentId",
                schema: "instances",
                table: "Trainings",
                column: "environmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_instanceId",
                schema: "instances",
                table: "Trainings",
                column: "instanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_trainingRestorationId",
                schema: "instances",
                table: "Trainings",
                column: "trainingRestorationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trainings",
                schema: "instances");

            migrationBuilder.DropTable(
                name: "TrainingRestorations",
                schema: "instances");
        }
    }
}
