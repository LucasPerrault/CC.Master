using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class CreateTableCountries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LegalUnits_CountryId",
                schema: "cafe",
                table: "LegalUnits",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalUnits_Countries_CountryId",
                schema: "cafe",
                table: "LegalUnits",
                column: "CountryId",
                principalSchema: "cafe",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalUnits_Countries_CountryId",
                schema: "cafe",
                table: "LegalUnits");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "cafe");

            migrationBuilder.DropIndex(
                name: "IX_LegalUnits_CountryId",
                schema: "cafe",
                table: "LegalUnits");
        }
    }
}
