using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddBillingCatalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [billingCatalog] WITH ACCENT_SENSITIVITY = OFF AS DEFAULT;", true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
