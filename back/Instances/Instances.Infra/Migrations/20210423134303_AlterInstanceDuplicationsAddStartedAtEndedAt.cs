using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class AlterInstanceDuplicationsAddStartedAtEndedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "endedAt",
                schema: "instances",
                table: "InstanceDuplications",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "startedAt",
                schema: "instances",
                table: "InstanceDuplications",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endedAt",
                schema: "instances",
                table: "InstanceDuplications");

            migrationBuilder.DropColumn(
                name: "startedAt",
                schema: "instances",
                table: "InstanceDuplications");
        }
    }
}
