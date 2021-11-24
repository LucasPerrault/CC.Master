using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddOffersFulltextIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON billing.CommercialOffers(name) KEY INDEX PK_OffreCommerciales ON [billingCatalog];", true);
            migrationBuilder.Sql("EXEC dbo.WaitForFullTextIndexing;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
