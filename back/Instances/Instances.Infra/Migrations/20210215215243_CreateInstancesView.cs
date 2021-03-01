using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class CreateInstancesView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [instances].[Instances] WITH SCHEMABINDING AS
                    SELECT
                       i.[id] id
                      ,i.[server] cluster
                      ,i.[type] type
                      ,i.[isProtected] isProtected
                      ,i.[allUsersImposedPassword] allUsersImposedPassword
                      ,i.[IsAnonymized] isAnonymized
                      ,i.[environmentId] environmentId
                      ,i.[isActive] isActive
                FROM [dbo].[NewInstances] i
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [instances].[Instances]
            ");
        }
    }
}
