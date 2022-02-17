using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Contracts.Infra.Migrations
{
    public partial class FixClientsFullTextIndex2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[Clients] WITH SCHEMABINDING AS
                    SELECT
                        c.[Id] Id,
                        c.[ExternalId] ExternalId,
                        c.[Name] Name,
                        c.[SocialReason] SocialReason,
                        c.[SalesforceId] SalesforceId,
                        c.[BillingStreet] BillingStreet,
                        c.[BillingPostalCode] BillingPostalCode,
                        c.[BillingCity] BillingCity,
                        c.[BillingCountry] BillingCountry,
                        c.[BillingMail] BillingMail,
                        c.[Phone] Phone,
                        c.[BillingEntity] BillingEntity
                FROM [dbo].[Clients] c;
            ");
            migrationBuilder.Sql("CREATE UNIQUE CLUSTERED INDEX PK_Clients ON [billing].[clients] (id);");
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON billing.Clients(name, socialReason) KEY INDEX PK_Clients ON [billingCatalog];", true);
            migrationBuilder.Sql("EXEC dbo.WaitForFullTextIndexing;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
