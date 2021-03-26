using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class CreateDuplicationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demos_Distributor_distributorID",
                schema: "instances",
                table: "Demos");

            migrationBuilder.DropForeignKey(
                name: "FK_Demos_Instance_instanceId",
                schema: "instances",
                table: "Demos");

            migrationBuilder.DropTable(
                name: "Distributor",
                schema: "instances");

            migrationBuilder.DropTable(
                name: "Instance",
                schema: "instances");

            migrationBuilder.AlterColumn<string>(
                name: "distributorId",
                schema: "instances",
                table: "InstanceDuplications",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_InstanceDuplications_distributorId",
                schema: "instances",
                table: "InstanceDuplications",
                column: "distributorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Demos_Distributors_distributorID",
                schema: "instances",
                table: "Demos",
                column: "distributorID",
                principalSchema: "shared",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Demos_Instances_instanceId",
                schema: "instances",
                table: "Demos",
                column: "instanceId",
                principalSchema: "instances",
                principalTable: "Instances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstanceDuplications_Distributors_distributorId",
                schema: "instances",
                table: "InstanceDuplications",
                column: "distributorId",
                principalSchema: "shared",
                principalTable: "Distributors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demos_Distributors_distributorID",
                schema: "instances",
                table: "Demos");

            migrationBuilder.DropForeignKey(
                name: "FK_Demos_Instances_instanceId",
                schema: "instances",
                table: "Demos");

            migrationBuilder.DropForeignKey(
                name: "FK_InstanceDuplications_Distributors_distributorId",
                schema: "instances",
                table: "InstanceDuplications");

            migrationBuilder.DropIndex(
                name: "IX_InstanceDuplications_distributorId",
                schema: "instances",
                table: "InstanceDuplications");

            migrationBuilder.AlterColumn<string>(
                name: "distributorId",
                schema: "instances",
                table: "InstanceDuplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "Distributor",
                schema: "instances",
                columns: table => new
                {
                    TempId1 = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("AK_Distributor_TempId1", x => x.TempId1);
                });

            migrationBuilder.CreateTable(
                name: "Instance",
                schema: "instances",
                columns: table => new
                {
                    TempId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("AK_Instance_TempId", x => x.TempId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Demos_Distributor_distributorID",
                schema: "instances",
                table: "Demos",
                column: "distributorID",
                principalSchema: "instances",
                principalTable: "Distributor",
                principalColumn: "TempId1",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Demos_Instance_instanceId",
                schema: "instances",
                table: "Demos",
                column: "instanceId",
                principalSchema: "instances",
                principalTable: "Instance",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
