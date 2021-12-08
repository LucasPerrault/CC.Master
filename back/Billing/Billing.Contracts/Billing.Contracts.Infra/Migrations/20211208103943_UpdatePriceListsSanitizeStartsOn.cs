using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class UpdatePriceListsSanitizeStartsOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                update billing.PriceLists
                set StartsOn = '2002-01-01'
                where StartsOn < '2002-01-01'
");
        }
    }
}
