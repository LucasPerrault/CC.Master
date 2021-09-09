using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddClientsClusteredIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE UNIQUE CLUSTERED INDEX PK_Clients ON [billing].[clients] (id);");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
