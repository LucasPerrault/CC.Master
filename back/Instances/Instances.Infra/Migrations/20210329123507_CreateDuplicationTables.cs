using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class CreateDuplicationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "instances");

            migrationBuilder.CreateTable(
                name: "InstanceDuplications",
                schema: "instances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    sourceSubdomain = table.Column<string>(maxLength: 200, nullable: false),
                    targetSubdomain = table.Column<string>(maxLength: 200, nullable: false),
                    sourceCluster = table.Column<string>(nullable: false),
                    targetCluster = table.Column<string>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    distributorId = table.Column<string>(nullable: false),
                    progress = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanceDuplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DemoDuplications",
                schema: "instances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceDuplicationId = table.Column<Guid>(nullable: false),
                    comment = table.Column<string>(nullable: true),
                    password = table.Column<string>(maxLength: 255, nullable: false),
                    sourceDemoId = table.Column<int>(nullable: false),
                    authorId = table.Column<int>(nullable: false),
                    createdAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoDuplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemoDuplications_InstanceDuplications_InstanceDuplicationId",
                        column: x => x.InstanceDuplicationId,
                        principalSchema: "instances",
                        principalTable: "InstanceDuplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DemoDuplications_Demos_sourceDemoId",
                        column: x => x.sourceDemoId,
                        principalSchema: "instances",
                        principalTable: "Demos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemoDuplications_InstanceDuplicationId",
                schema: "instances",
                table: "DemoDuplications",
                column: "InstanceDuplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_DemoDuplications_sourceDemoId",
                schema: "instances",
                table: "DemoDuplications",
                column: "sourceDemoId");

            migrationBuilder.CreateIndex(
                name: "IX_Demos_distributorID",
                schema: "instances",
                table: "Demos",
                column: "distributorID");

            migrationBuilder.CreateIndex(
                name: "IX_Demos_instanceId",
                schema: "instances",
                table: "Demos",
                column: "instanceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstanceDuplications_distributorId",
                schema: "instances",
                table: "InstanceDuplications",
                column: "distributorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemoDuplications",
                schema: "instances");

            migrationBuilder.DropTable(
                name: "InstanceDuplications",
                schema: "instances");

            migrationBuilder.DropTable(
                name: "Demos",
                schema: "instances");
        }
    }
}
