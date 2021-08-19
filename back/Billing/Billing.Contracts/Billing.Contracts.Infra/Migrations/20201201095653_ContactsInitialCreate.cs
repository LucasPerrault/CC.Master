using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class ContactsInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [billing].[Clients] WITH SCHEMABINDING AS
                    SELECT
                        c.[Id] Id,
                        c.[ExternalId] ExternalId,
                        c.[Name] Name,
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
            migrationBuilder.Sql($@"
                CREATE VIEW [billing].[Contracts] WITH SCHEMABINDING AS
                    SELECT
                        co.[idContract] Id,
                        co.[ExternalId] ExternalId,
                        co.[ArchivedAt] ArchivedAt,
                        cl.[Id] ClientId,
                        cl.[ExternalId] ClientExternalId,
                        e.[id] EnvironmentId,
                        e.[subdomain] EnvironmentSubdomain
                FROM [dbo].[Contracts] co
                    LEFT JOIN [dbo].[Clients] cl ON cl.Id = co.clientId
                    LEFT JOIN [dbo].[Environments] e ON e.id = co.environmentID
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [billing].[Clients]
            ");
            migrationBuilder.Sql($@"
                DROP VIEW [billing].[Contracts]
            ");
        }
    }
}
