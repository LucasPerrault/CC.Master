using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterContractsAddCommercialOfferId : Migration
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
                        e.[id] EnvironmentId,
                        e.[subdomain] EnvironmentSubdomain
                FROM [dbo].[Contracts] co
                    LEFT JOIN [dbo].[Clients] cl ON cl.Id = co.clientId
                    LEFT JOIN [dbo].[Environments] e ON e.id = co.environmentID
            "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommercialOfferId",
                schema: "billing",
                table: "Contracts");
        }
    }
}
