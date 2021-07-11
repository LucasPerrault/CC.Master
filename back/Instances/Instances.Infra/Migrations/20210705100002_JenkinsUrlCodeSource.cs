using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class JenkinsUrlCodeSource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER table [instances].[CodeSources]
                ADD JenkinsProjectUrl nvarchar(max)
            ");

            migrationBuilder.Sql(@"
                UPDATE [instances].[CodeSources]
                   SET JenkinsProjectUrl = CONCAT('http://jenkins2.lucca.local:8080/job/',
                      CASE Type
                         WHEN 0 THEN CONCAT('NoMonolith', '/job/', jenkinsProjectName)
                         WHEN 1 THEN CONCAT('webservices', '/job/', jenkinsProjectName)
                         WHEN 2 THEN 'CI.Pipeline.Multibranch'
                         WHEN 3 THEN CONCAT('webservices-legacy', '/job/', jenkinsProjectName)
                         ELSE CONCAT('webservices', '/job/', jenkinsProjectName)
                      END)
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
