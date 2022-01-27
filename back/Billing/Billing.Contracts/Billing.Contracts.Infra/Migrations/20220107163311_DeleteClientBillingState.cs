using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Contracts.Infra.Migrations
{
    public partial class DeleteClientBillingState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
                SELECT
                    c.idContract id,
                    o.productId productId,
                    cast(c.startDate as date) startDate,
                    cast(c.endDate as date) endDate,
                    c.creationCause creationCause,
                    c.endContractReason endReason,
                    c.clientId clientId,
                    c.idDistributor distributorId,
                    e.CreatedAt environmentCreatedAt,
                    CAST(CASE WHEN c.ArchivedAt is null THEN 0 ELSE 1 END AS bit) AS isArchived
                FROM dbo.[Contracts] c
                INNER JOIN [billing].[CommercialOffers] o on c.idCommercialOffer = o.id
                INNER JOIN [shared].[Environments] e on c.environmentID = e.id;
            ");

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
                        c.[Phone] Phone
                FROM [dbo].[Clients] c;
            ");

            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
                SELECT
                    c.idContract id,
                    o.productId productId,
                    cast(c.startDate as date) startDate,
                    cast(c.endDate as date) endDate,
                    c.creationCause creationCause,
                    c.endContractReason endReason,
                    c.clientId clientId,
                    c.idDistributor distributorId,
                    e.CreatedAt environmentCreatedAt,
                    cl.name clientName,
                    CAST(CASE WHEN c.ArchivedAt is null THEN 0 ELSE 1 END AS bit) AS isArchived
                FROM dbo.[Contracts] c
                INNER JOIN [billing].[CommercialOffers] o on c.idCommercialOffer = o.id
                INNER JOIN [shared].[Environments] e on c.environmentID = e.id
                INNER JOIN [billing].[Clients] cl on c.clientId = cl.id;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
                SELECT
                    c.idContract id,
                    o.productId productId,
                    cast(c.startDate as date) startDate,
                    cast(c.endDate as date) endDate,
                    c.creationCause creationCause,
                    c.endContractReason endReason,
                    c.clientId clientId,
                    c.idDistributor distributorId,
                    e.CreatedAt environmentCreatedAt,
                    CAST(CASE WHEN c.ArchivedAt is null THEN 0 ELSE 1 END AS bit) AS isArchived
                FROM dbo.[Contracts] c
                INNER JOIN [billing].[CommercialOffers] o on c.idCommercialOffer = o.id
                INNER JOIN [shared].[Environments] e on c.environmentID = e.id;
            ");

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
                FROM [dbo].[Clients] c;
            ");

            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
                SELECT
                    c.idContract id,
                    o.productId productId,
                    cast(c.startDate as date) startDate,
                    cast(c.endDate as date) endDate,
                    c.creationCause creationCause,
                    c.endContractReason endReason,
                    c.clientId clientId,
                    c.idDistributor distributorId,
                    e.CreatedAt environmentCreatedAt,
                    cl.name clientName,
                    CAST(CASE WHEN c.ArchivedAt is null THEN 0 ELSE 1 END AS bit) AS isArchived
                FROM dbo.[Contracts] c
                INNER JOIN [billing].[CommercialOffers] o on c.idCommercialOffer = o.id
                INNER JOIN [shared].[Environments] e on c.environmentID = e.id
                INNER JOIN [billing].[Clients] cl on c.clientId = cl.id;
            ");
        }
    }
}
