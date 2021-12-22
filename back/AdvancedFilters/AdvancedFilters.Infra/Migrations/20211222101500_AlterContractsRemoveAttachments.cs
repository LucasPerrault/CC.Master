using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class AlterContractsRemoveAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstablishmentContracts",
                schema: "cafe");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstablishmentContracts",
                schema: "cafe",
                columns: table => new
                {
                    EnvironmentId = table.Column<int>(type: "int", nullable: false),
                    EstablishmentId = table.Column<int>(type: "int", nullable: false),
                    ContractId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentContracts", x => new { x.EnvironmentId, x.EstablishmentId, x.ContractId });
                    table.ForeignKey(
                        name: "FK_EstablishmentContracts_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "cafe",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentContracts_Establishments_EnvironmentId_EstablishmentId",
                        columns: x => new { x.EnvironmentId, x.EstablishmentId },
                        principalSchema: "cafe",
                        principalTable: "Establishments",
                        principalColumns: new[] { "EnvironmentId", "Id" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentContracts_ContractId",
                schema: "cafe",
                table: "EstablishmentContracts",
                column: "ContractId");
        }
    }
}
