using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddClientsFulltextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON billing.Clients(name) KEY INDEX PK_Clients;", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
