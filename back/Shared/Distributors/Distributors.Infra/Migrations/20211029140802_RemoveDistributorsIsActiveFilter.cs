using Microsoft.EntityFrameworkCore.Migrations;

namespace Distributors.Infra.Migrations
{
    public partial class RemoveDistributorsIsActiveFilter : Migration
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
                    d.isAllowingCommercialCommunication IsAllowingCommercialCommunication,
                    CAST(CASE d.isActive WHEN 1 THEN 1 ELSE 0 END AS BIT) IsActive
                FROM [dbo].[Distributors] d
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
                    d.partenairesDepartmentId departmentId,
                    d.isAllowingCommercialCommunication IsAllowingCommercialCommunication
                FROM [dbo].[Distributors] d
                WHERE d.isActive = 1
            ");
        }
    }
}
