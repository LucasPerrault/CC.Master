using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class JenkinsUrlCodeSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER table [instances].[CodeSources]
                ADD JenkinsProjectUrl nvarchar
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER table [instances].[CodeSources]
                DROP COLUMN JenkinsProjectUrl
            ");
        }
    }
}
