using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Cmrr.Infra.Migrations
{
    public partial class CastToDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [billing].[CmrrCounts] WITH SCHEMABINDING AS
                    SELECT
                    c. idCount id,
                    c.idContract contractId,
                    cast(c.countPeriod as date) countPeriod,
                    c.billingStrategy billingStrategy,
                    c.accountingNumber accountingNumber,
                    a.euroAmount euroTotal
                    FROM [dbo].[Counts] c
                    INNER JOIN [dbo].[Accountings] a on a.entryNumber = c.entryNumber
                    WHERE accountNumber = '401'
            ");
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [billing].[CmrrCounts] WITH SCHEMABINDING AS
                    SELECT
                    c. idCount id,
                    c.idContract contractId,
                    c.countPeriod countPeriod,
                    c.billingStrategy billingStrategy,
                    c.accountingNumber accountingNumber,
                    c.entryNumber entryNumber,
                    c.code code,
                    a.currencyId currencyId,
                    a.currencyAmount currencyAmount,
                    a.euroAmount euroAmount,
                    c.discount1 luccaDiscount,
                    c.discount2 distributorDiscount,
                    c.countDate createdAt
                    FROM [dbo].[Counts] c
                    INNER JOIN [dbo].[Accountings] a on a.entryNumber = c.entryNumber
                    WHERE accountNumber = '401'
            ");
            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
                    SELECT
                        c.idContract id,
                        o.productId productId,
                        c.creationDate creationDate,
                        c.startDate startDate,
                        c.endDate endDate,
                        c.creationCause creationCause,
                        c.endContractReason endReason,
                        c.clientId clientId,
                        c.idDistributor distributorId,
                        c.environmentID environmentId,
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
