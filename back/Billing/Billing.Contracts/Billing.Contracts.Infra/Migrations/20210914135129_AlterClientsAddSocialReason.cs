using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterClientsAddSocialReason : Migration
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
                        c.[BillingState] BillingState,
                        c.[BillingCountry] BillingCountry,
                        c.[BillingMail] BillingMail,
                        c.[Phone] Phone
                FROM [dbo].[Clients] c
            ");
            migrationBuilder.Sql("CREATE UNIQUE CLUSTERED INDEX PK_Clients ON [billing].[clients] (id);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
