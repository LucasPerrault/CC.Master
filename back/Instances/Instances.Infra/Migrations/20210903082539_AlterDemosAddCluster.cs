using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class AlterDemosAddCluster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cluster",
                schema: "instances",
                table: "Demos",
                nullable: true);

            migrationBuilder.Sql(@"
                    update instances.Demos
                    set cluster = (select cluster
                            from instances.Instances
                            where instances.Demos.instanceId = instances.Instances.id)

");

            migrationBuilder.Sql(@"
                alter view [instances].[Instances] WITH SCHEMABINDING AS
                    select
                       i.[id] id
                      ,i.[type] type
                      ,i.[isProtected] isProtected
                      ,i.[allUsersImposedPassword] allUsersImposedPassword
                      ,i.[IsAnonymized] isAnonymized
                      ,i.[environmentId] environmentId
                      ,i.[isActive] isActive
                FROM [dbo].[NewInstances] i
");

            migrationBuilder.Sql(@"
                ALTER VIEW [dbo].[Demos] WITH SCHEMABINDING AS
                    SELECT
                    d.[id] id
                    ,d.[subdomain] subdomain
                    ,d.[createdAt] dtCreation
                    ,d.[isActive] isActive
                    ,d.[cluster] cluster
                    ,d.[distributorId] distributorId
                    ,d.[instanceId] instanceId
                    ,d.[comment] comment
                    ,d.[deletionScheduledOn] deletionScheduledOn
                FROM [instances].[Demos] d
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        { }
    }
}
