using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class CreateCodeSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.CodeSources;
            ");
            migrationBuilder.Sql($@"
                create view [dbo].[CodeSources] with schemabinding as
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

            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.CodeSourceConfigs;
            ");
            migrationBuilder.Sql($@"
                create view [dbo].[CodeSourceConfigs] with schemabinding as
                    select
                        codeSourceId,
                        appPath,
                        subdomain,
                        iisServerPath,
                        isPrivate
                    from
                        [instances].[CodeSourceConfigs]
            ");

            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.CodeSourcesProductionVersions;
            ");
            migrationBuilder.Sql($@"
                create view [dbo].[CodeSourcesProductionVersions] with schemabinding as
                    select
                        id,
                        date,
                        branchName,
                        JenkinsBuildNumber,
                        CommitHash,
                        CodeSourceId
                    from
                        [instances].[CodeSourcesProductionVersions]
            ");

            migrationBuilder.CreateIndex(
                name: "IX_CodeSourceConfigs_CodeSourceId",
                schema: "instances",
                table: "CodeSourceConfigs",
                column: "CodeSourceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodeSourcesProductionVersions_CodeSourceId",
                schema: "instances",
                table: "CodeSourcesProductionVersions",
                column: "CodeSourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        { }
    }
}
