using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class AddClusterDistribsAndAccesses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cluster",
                schema: "cafe",
                table: "Environments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Distributors",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    IsAllowingCommercialCommunication = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distributors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentAccesses",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EnvironmentId = table.Column<int>(type: "int", nullable: false),
                    DistributorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentAccesses_Distributors_DistributorId",
                        column: x => x.DistributorId,
                        principalSchema: "cafe",
                        principalTable: "Distributors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnvironmentAccesses_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Distributors_DepartmentId",
                schema: "cafe",
                table: "Distributors",
                column: "DepartmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentAccesses_DistributorId",
                schema: "cafe",
                table: "EnvironmentAccesses",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentAccesses_EnvironmentId",
                schema: "cafe",
                table: "EnvironmentAccesses",
                column: "EnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvironmentAccesses",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Distributors",
                schema: "cafe");

            migrationBuilder.DropColumn(
                name: "Cluster",
                schema: "cafe",
                table: "Environments");
        }
    }
}
