using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class CreationsAfterOffersMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE OR ALTER VIEW [dbo].[Contract_LegalEntityErrorNumber] WITH SCHEMABINDING AS
            WITH ContractsWithProducts AS
              ( SELECT c.idContract contractId ,
                       o.productId productId ,
                       c.environmentID
               FROM dbo.Contracts c
               INNER JOIN dbo.CommercialOffers o ON o.idCommercialOffer = c.idCommercialOffer
               WHERE (c.endDate IS NULL
                      OR c.endDate > GETDATE()) )

            SELECT les.contractId ,
                   COUNT(les.Id) LEErrorNumber
            FROM
              ( SELECT cs.contractId,
                       cs.productId,
                       le.Id,
                       le.environmentID
               FROM dbo.LegalEntities le
               INNER JOIN ContractsWithProducts cs ON cs.environmentID = le.environmentID
               LEFT JOIN dbo.ExcludedEntities ee ON ee.legalEntityID = le.Id
               AND ee.productId = cs.productId
               WHERE le.isActive = 1
                AND le.environmentID IS NOT NULL
                AND ee.id IS NULL

                --detection rules
                AND (

                -- trivial case not handled by exclusion rules : contract with no entities
                NOT EXISTS (SELECT 1
                             FROM dbo.ContractEntities ce
                             INNER JOIN ContractsWithProducts cs2 ON cs2.contractId = ce.contractID
                             WHERE cs.productId = cs2.productId
                                AND ce.legalEntityID = le.id )

                -- exclusion rules : let's describe valid states. Everything else is an error
                OR NOT (

                        -- valid state : exactly one contract entity covering current period
                        (SELECT Count(Id)
                            FROM dbo.ContractEntities ce
                            INNER JOIN ContractsWithProducts cs2 ON cs2.contractId = ce.contractID
                            WHERE cs.productId = cs2.productId
                            AND ce.legalEntityID = le.id
                            AND ce.[start] <= GETDATE()
                            AND (ce.[end] IS NULL OR ce.[end] > GETDATE())) = 1

                        OR

                        -- valid state : no contract entity in the past, everything in the future (contract waiting to be started)
                        NOT EXISTS
                        (SELECT 1
                            FROM dbo.ContractEntities ce
                            INNER JOIN ContractsWithProducts cs2 ON cs2.contractId = ce.contractID
                            WHERE cs.productId = cs2.productId
                            AND ce.legalEntityID = le.id
                            AND (ce.[start] IS NULL
                                OR ce.[start] < GETDATE())
                            AND (ce.[end] IS NULL
                                OR ce.[end] < GETDATE())) ) ) )les
            GROUP BY les.contractId;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW [billing].[CmrrContracts] WITH SCHEMABINDING AS
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

            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[Counts]  WITH CHECK ADD  CONSTRAINT [FK_Counts_OFFER] FOREIGN KEY([idCommercialOffer])
                REFERENCES [billing].[CommercialOffers] (id)
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
