using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvancedFilters.Infra.Migrations
{
    public partial class AlterDistributorsAddIsLucca : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLucca",
                schema: "cafe",
                table: "Distributors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLucca",
                schema: "cafe",
                table: "Distributors");
        }
    }
}
