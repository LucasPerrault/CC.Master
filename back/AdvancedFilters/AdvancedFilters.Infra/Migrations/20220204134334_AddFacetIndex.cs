using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedFilters.Infra.Migrations
{
    public partial class AddFacetIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "cafe",
                table: "Facets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationId",
                schema: "cafe",
                table: "Facets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Facets_ApplicationId_Code",
                schema: "cafe",
                table: "Facets",
                columns: new[] { "ApplicationId", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Facets_ApplicationId_Code",
                schema: "cafe",
                table: "Facets");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "cafe",
                table: "Facets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationId",
                schema: "cafe",
                table: "Facets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
