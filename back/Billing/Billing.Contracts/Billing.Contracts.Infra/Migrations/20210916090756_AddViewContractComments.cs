using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddViewContractComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (
                $@"
                CREATE VIEW [billing].[ContractComments] WITH SCHEMABINDING AS
                    SELECT
                        co.[idContract] ContractId,
                        co.[idDistributor] DistributorId,
                        co.[comment] Comment
                FROM [dbo].[Contracts] co
            "
            );
        }
    }
}
