using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class AlterInstanceDuplicationAddSourceAndTargetTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DemoDuplications_Demos_sourceDemoId",
                schema: "instances",
                table: "DemoDuplications");

            migrationBuilder.DropIndex(
                name: "IX_DemoDuplications_sourceDemoId",
                schema: "instances",
                table: "DemoDuplications");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "instances",
                table: "InstanceDuplications");

            migrationBuilder.AddColumn<int>(
                name: "sourceType",
                schema: "instances",
                table: "InstanceDuplications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "targetType",
                schema: "instances",
                table: "InstanceDuplications",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sourceType",
                schema: "instances",
                table: "InstanceDuplications");

            migrationBuilder.DropColumn(
                name: "targetType",
                schema: "instances",
                table: "InstanceDuplications");

            migrationBuilder.AddColumn<int>(
                name: "type",
                schema: "instances",
                table: "InstanceDuplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DemoDuplications_sourceDemoId",
                schema: "instances",
                table: "DemoDuplications",
                column: "sourceDemoId");

            migrationBuilder.AddForeignKey(
                name: "FK_DemoDuplications_Demos_sourceDemoId",
                schema: "instances",
                table: "DemoDuplications",
                column: "sourceDemoId",
                principalSchema: "instances",
                principalTable: "Demos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
