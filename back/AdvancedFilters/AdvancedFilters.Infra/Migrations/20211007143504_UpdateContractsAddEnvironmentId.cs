using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class UpdateContractsAddEnvironmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvironmentId",
                schema: "cafe",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_EnvironmentId",
                schema: "cafe",
                table: "Contracts",
                column: "EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Environments_EnvironmentId",
                schema: "cafe",
                table: "Contracts",
                column: "EnvironmentId",
                principalSchema: "cafe",
                principalTable: "Environments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Environments_EnvironmentId",
                schema: "cafe",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_EnvironmentId",
                schema: "cafe",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                schema: "cafe",
                table: "Contracts");
        }
    }
}
