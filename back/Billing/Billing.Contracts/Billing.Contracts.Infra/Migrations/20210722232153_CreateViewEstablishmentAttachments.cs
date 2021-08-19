using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class CreateViewEstablishmentAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql
            (
                $@"
                Create VIEW [billing].[EstablishmentAttachments] WITH SCHEMABINDING AS
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
