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

            migrationBuilder.Sql(@"
                update cafe.Distributors
                set IsLucca = 1
                where Id = 37
            ");
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
