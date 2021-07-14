using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class CodeSourceAndArtifactViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE OR ALTER VIEW [dbo].[CodeSourceArtifacts] WITH SCHEMABINDING AS
                    SELECT
                       a.[Id] Id
                      ,a.[CodeSourceId] CodeSourceId
                      ,a.[FileName] FileName
                      ,a.[ArtifactUrl] ArtifactUrl
                      ,a.[ArtifactType] ArtifactType
                FROM [instances].[CodeSourceArtifacts] a
            ");

            migrationBuilder.Sql($@"
                create or alter view [dbo].[CodeSources] with schemabinding as
                    select
                        id,
                        name,
                        code,
                        jenkinsProjectName,
                        type,
                        githubRepo,
                        lifecycle,
                        JenkinsProjectUrl
                    from
                        [instances].[CodeSources]
            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [instances].[CodeSourceArtifacts]
            ");

            migrationBuilder.Sql($@"
                create or alter view [dbo].[CodeSources] with schemabinding as
                    select
                        id,
                        name,
                        code,
                        jenkinsProjectName,
                        type,
                        githubRepo,
                        lifecycle
                    from
                        [instances].[CodeSources]
            ");
        }
    }
}
