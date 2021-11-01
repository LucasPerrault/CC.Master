using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddContractEnvironment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (
                $@"
                CREATE VIEW [billing].[ContractEnvironments] WITH SCHEMABINDING AS
                    SELECT
                        e.[id] id,
                        e.[subdomain] Subdomain
                FROM [dbo].[Environments] e"
            );
            migrationBuilder.Sql
            (
                $@"
                CREATE VIEW [billing].[Establishments] WITH SCHEMABINDING AS
                    SELECT
                        e.[id] id,
                        e.[name] name,
                        e.[environmentID] environmentId,
                        e.[localID] remoteId,
                        e.[isActive] isActive,
                        e.[legalUnitId] legalUnitId
                FROM [dbo].[LegalEntities] e"
            );
            migrationBuilder.Sql
            (
                $@"
                CREATE VIEW [billing].[EstablishmentExclusions] WITH SCHEMABINDING AS
                    SELECT
                        e.[id] id,
                        e.[creatorId] authorId,
                        e.[createdAt] createdAt,
                        e.[productId] productId,
                        e.[legalEntityId] establishmentId
                FROM [dbo].[ExcludedEntities] e"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP VIEW [billing].[EstablishmentExclusions]");
            migrationBuilder.Sql($@"DROP VIEW [billing].[Establishments]");
            migrationBuilder.Sql($@"DROP VIEW [billing].[ContractEnvironments]");
        }
    }
}
