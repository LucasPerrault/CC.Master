using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Contracts.Infra.Migrations
{
    public partial class DeleteClientBillingState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingState",
                schema: "billing",
                table: "Clients");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingState",
                schema: "billing",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
