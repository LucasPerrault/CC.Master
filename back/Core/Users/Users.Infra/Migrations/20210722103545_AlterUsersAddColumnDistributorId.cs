using Microsoft.EntityFrameworkCore.Migrations;

namespace Users.Infra.Migrations
{
    public partial class AlterUsersAddColumnDistributorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistributorId",
                schema: "shared",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistributorId",
                schema: "shared",
                table: "Users");
        }
    }
}
