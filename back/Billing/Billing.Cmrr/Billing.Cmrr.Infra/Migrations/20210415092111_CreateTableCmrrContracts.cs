using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Cmrr.Infra.Migrations
{
    public partial class CreateTableCmrrContracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
