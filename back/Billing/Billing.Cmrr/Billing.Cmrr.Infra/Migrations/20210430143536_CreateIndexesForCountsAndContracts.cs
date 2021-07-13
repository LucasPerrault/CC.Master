using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Cmrr.Infra.Migrations
{
    public partial class CreateIndexesForCountsAndContracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                USE [CLOUDCONTROL]
                GO
                CREATE NONCLUSTERED INDEX [IX_Accountings_AccountNumber_Cmrr]
                ON [dbo].[Accountings] ([accountNumber])
                INCLUDE ([entryNumber],[CurrencyAmount],[CurrencyId],[EuroAmount])
                GO

                USE [CLOUDCONTROL]
                GO
                CREATE NONCLUSTERED INDEX [IX_Contracts_EndDate_Cmrr]
                ON [dbo].[Contracts] ([startDate],[endDate])
                INCLUDE ([idDistributor],[idCommercialOffer],[creationDate],[endContractReason],[environmentID],[ArchivedAt],[clientId],[creationCause])
                GO

                USE [CLOUDCONTROL]
                GO
                CREATE NONCLUSTERED INDEX [IX_Counts_CountPeriod_Cmrr]
                ON [dbo].[Counts] ([countPeriod])
                INCLUDE ([idContract],[discount1],[discount2],[countDate],[accountingNumber],[entryNumber],[Code],[billingStrategy])
                GO
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
