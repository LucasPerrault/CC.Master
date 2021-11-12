using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterContractDropColumnEnvironmentSubdomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.Sql
            (
                $@"
                ALTER VIEW [billing].[Contracts] WITH SCHEMABINDING AS
                    SELECT
                        co.[idContract] Id,
                        co.[ExternalId] ExternalId,
                        co.[ArchivedAt] ArchivedAt,
                        co.[idDistributor] DistributorId,
                        co.[idCommercialOffer] CommercialOfferId,
                        cl.[Id] ClientId,
                        cl.[ExternalId] ClientExternalId,
                        co.[environmentId] EnvironmentId,
                        co.[creationDate] CreatedAt,
                        co.[startDate] TheoreticalStartOn,
                        co.[endDate] TheoreticalEndOn,
                        co.[unityNumberTheorical] CountEstimation,
                        co.[nbMonthRebate] TheoreticalFreeMonths,
                        co.[rebate] RebatePercentage,
                        co.[rebate_endDate] RebateEndsOn,
                        co.[minimalBillingPercentage] MinimalBillingPercentage,
                        co.[billingMonth] BillingPeriodicity,
                        co.[endContractReason] EndReason
                FROM [dbo].[Contracts] co
                    LEFT JOIN [dbo].[Clients] cl ON cl.Id = co.clientId
            "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        { }
    }
}
