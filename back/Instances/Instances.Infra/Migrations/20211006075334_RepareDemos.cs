using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class RepareDemos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    update instances.Demos set cluster =
                    (
                        select top(1) targetCluster from instances.InstanceDuplications
                            where progress = 3
                            and sourceType = 3
                            and startedAt > '2021-10-04'
                            and targetSubdomain = instances.Demos.subdomain
                            order by endedAt desc

                    )
                    where cluster is null
");
        }
    }
}
