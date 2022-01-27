using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Contracts.Infra.Migrations
{
    public partial class FixClientsFullTextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE UNIQUE CLUSTERED INDEX PK_Clients ON [billing].[clients] (id);");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON billing.Clients(name, socialReason) KEY INDEX PK_Clients ON [billingCatalog];", true);
            migrationBuilder.Sql("EXEC dbo.WaitForFullTextIndexing;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
