using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterAttachmentsAddEndReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (
                $@"
                ALTER VIEW [billing].[EstablishmentAttachments] WITH SCHEMABINDING AS
                    SELECT
                        ce.[id] Id,
                        ce.[ContractId] ContractId,
                        ce.[legalEntityId] EstablishmentId,
                        le.[name] EstablishmentName,
                        le.[localId] EstablishmentRemoteId,
                        ce.[start] startsOn,
                        ce.[end] endsOn,
                        ce.[isActive] isActive,
                        ce.[endReason] endReason,
                        ce.[nbMonthFree] numberOfFreeMonths
                FROM [dbo].[ContractEntities] ce
                    LEFT JOIN [dbo].[LegalEntities] le ON le.Id = ce.legalEntityID
            "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (
                $@"
                ALTER VIEW [billing].[EstablishmentAttachments] WITH SCHEMABINDING AS
                    SELECT
                        ce.[id] Id,
                        ce.[ContractId] ContractId,
                        ce.[legalEntityId] EstablishmentId,
                        le.[name] EstablishmentName,
                        le.[localId] EstablishmentRemoteId,
                        ce.[start] startsOn,
                        ce.[end] endsOn,
                        ce.[isActive] isActive
                FROM [dbo].[ContractEntities] ce
                    LEFT JOIN [dbo].[LegalEntities] le ON le.Id = ce.legalEntityID
            "
            );
        }
    }
}
