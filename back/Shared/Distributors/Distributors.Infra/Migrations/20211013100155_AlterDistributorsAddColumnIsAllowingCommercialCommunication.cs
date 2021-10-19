using Microsoft.EntityFrameworkCore.Migrations;

namespace Distributors.Infra.Migrations
{
    public partial class AlterDistributorsAddColumnIsAllowingCommercialCommunication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER VIEW [shared].[Distributors] WITH SCHEMABINDING AS
                    SELECT
                    d.id id,
                    d.distributorName name,
                    d.distributorCode code,
                    d.partenairesDepartmentId departmentId,
                    d.isAllowingCommercialCommunication IsAllowingCommercialCommunication
                FROM [dbo].[Distributors] d
                WHERE d.isActive = 1
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER VIEW [shared].[Distributors] WITH SCHEMABINDING AS
                    SELECT
                    d.id id,
                    d.distributorName name,
                    d.distributorCode code,
                    d.partenairesDepartmentId departmentId
                FROM [dbo].[Distributors] d
                WHERE d.isActive = 1
            ");
        }
    }
}
