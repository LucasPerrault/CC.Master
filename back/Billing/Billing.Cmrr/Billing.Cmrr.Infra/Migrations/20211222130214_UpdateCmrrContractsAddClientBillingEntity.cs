using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Cmrr.Infra.Migrations
{
    public partial class UpdateCmrrContractsAddClientBillingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
                    SELECT
                        c.idContract id,
                        o.productId productId,
                        cast(c.startDate as date) startDate,
                        cast(c.endDate as date) endDate,
                        c.creationCause creationCause,
                        c.endContractReason endReason,
                        c.clientId clientId,
                        c.idDistributor distributorId,
                        cast(e.dtCreation as date) creationDate,
                        e.dtCreation environmentCreatedAt,
                        cl.name clientName,
                        cl.billingEntity clientBillingEntity,
                        CAST(CASE WHEN c.ArchivedAt is null THEN 0 ELSE 1 END AS bit) AS isArchived
                    FROM [dbo].[Contracts] c
                    INNER JOIN [dbo].[CommercialOffers] o on c.idCommercialOffer = o.idCommercialOffer
                    INNER JOIN [dbo].[Environments] e on c.environmentID = e.id
                    INNER JOIN [dbo].[Clients] cl on c.clientId = cl.id
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
                    SELECT
                        c.idContract id,
                        o.productId productId,
                        cast(c.startDate as date) startDate,
                        cast(c.endDate as date) endDate,
                        c.creationCause creationCause,
                        c.endContractReason endReason,
                        c.clientId clientId,
                        c.idDistributor distributorId,
                        cast(e.dtCreation as date) creationDate,
                        e.dtCreation environmentCreatedAt,
                        cl.name clientName,
                        CAST(CASE WHEN c.ArchivedAt is null THEN 0 ELSE 1 END AS bit) AS isArchived
                    FROM [dbo].[Contracts] c
                    INNER JOIN [dbo].[CommercialOffers] o on c.idCommercialOffer = o.idCommercialOffer
                    INNER JOIN [dbo].[Environments] e on c.environmentID = e.id
                    INNER JOIN [dbo].[Clients] cl on c.clientId = cl.id
            ");
        }
    }
}
