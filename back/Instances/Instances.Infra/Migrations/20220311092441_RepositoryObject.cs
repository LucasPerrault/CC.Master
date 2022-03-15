using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instances.Infra.Migrations
{
    public partial class RepositoryObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RepoId",
                schema: "instances",
                table: "GithubPullRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RepoId",
                schema: "instances",
                table: "GithubBranches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RepoId",
                schema: "instances",
                table: "CodeSources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GithubRepos",
                schema: "instances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GithubRepos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GithubPullRequests_RepoId",
                schema: "instances",
                table: "GithubPullRequests",
                column: "RepoId");

            migrationBuilder.CreateIndex(
                name: "IX_GithubBranches_RepoId",
                schema: "instances",
                table: "GithubBranches",
                column: "RepoId");

            migrationBuilder.CreateIndex(
                name: "IX_CodeSources_RepoId",
                schema: "instances",
                table: "CodeSources",
                column: "RepoId");

            // Update des nouvelles colonnes
            migrationBuilder.Sql(@"
                insert into instances.GithubRepos(Name, Url)
                    SELECT distinct SUBSTRING(githubRepo , LEN(githubRepo) -  CHARINDEX('/',REVERSE(githubRepo)) + 2  , LEN(githubRepo)  ), githubRepo
                    FROM [CLOUDCONTROL].[instances].CodeSources
            ");
            migrationBuilder.Sql(@"
                update [CLOUDCONTROL].[instances].CodeSources  
                    SET RepoId = (select Id from instances.GithubRepos where Url = githubRepo)
            ");
            migrationBuilder.Sql(@"
                update instances.GithubBranches
                set repoId = (select max(cs.RepoId) from instances.GithubBranchesCodeSources gbs
                   inner join instances.CodeSources cs on cs.id = gbs.codeSourceId
                   where gbs.githubBranchId = instances.GithubBranches.id)
            ");
            migrationBuilder.Sql(@"
                update instances.GithubPullRequests
                set repoId = (select max(cs.RepoId) from instances.GithubPullRequestsCodeSources glr
                   inner join instances.CodeSources cs on cs.id = glr.codeSourceId
                   where glr.githubPullRequestId = instances.GithubPullRequests.id)
            ");

            // Mise à jour de la vue de retro-compatibilité (vieux CC)
            migrationBuilder.Sql(@"
                 create or alter view [dbo].[GithubBranchesCodeSources] with schemabinding as
                    select
                      cs.id codeSourceId,
                      gb.id githubBranchId
                    from [instances].[CodeSources] cs
					inner join [instances].GithubBranches gb on gb.RepoId = cs.RepoId
            ");
            migrationBuilder.Sql(@"
                 create or alter view [dbo].[GithubPullRequestsCodeSources] with schemabinding as
                    select
                      cs.id codeSourceId,
                      gl.id githubPullRequestId
                    from [instances].[CodeSources] cs
					inner join [instances].GithubPullRequests gl on gl.RepoId = cs.RepoId
            ");
            migrationBuilder.Sql(@"
                create or alter view [dbo].[CodeSources] with schemabinding as
                    select
                        cs.id,
                        cs.name,
                        cs.code,
                        cs.jenkinsProjectName,
                        cs.type,
                        gr.url as githubRepo,
                        cs.lifecycle,
                        cs.JenkinsProjectUrl
                    from
                        [instances].[CodeSources] cs
						inner join [instances].[GithubRepos] gr on gr.Id = cs.RepoId
            ");


            migrationBuilder.AddForeignKey(
                name: "FK_CodeSources_GithubRepos_RepoId",
            schema: "instances",
            table: "CodeSources",
            column: "RepoId",
            principalSchema: "instances",
            principalTable: "GithubRepos",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GithubBranches_GithubRepos_RepoId",
                schema: "instances",
                table: "GithubBranches",
                column: "RepoId",
                principalSchema: "instances",
                principalTable: "GithubRepos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GithubPullRequests_GithubRepos_RepoId",
                schema: "instances",
                table: "GithubPullRequests",
                column: "RepoId",
                principalSchema: "instances",
                principalTable: "GithubRepos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropTable(
                name: "GithubBranchesCodeSources",
                schema: "instances");

            migrationBuilder.DropTable(
                name: "GithubPullRequestsCodeSources",
                schema: "instances");

            migrationBuilder.DropColumn(
                name: "GithubRepo",
                schema: "instances",
                table: "CodeSources");
        }
    }
}
