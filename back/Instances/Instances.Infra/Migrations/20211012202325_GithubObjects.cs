using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class GithubObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.GithubBranches;
            ");
            migrationBuilder.Sql($@"
                create view [dbo].[GithubBranches] with schemabinding as
                    select
                      id,
                      name,
                      isDeleted,
                      createdAt,
                      lastPushedAt,
                      deletedAt,
                      headCommitHash,
                      headCommitMessage
                    from
                        [instances].[GithubBranches]
            ");

            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.GithubBranchesCodeSources;
            ");
            migrationBuilder.Sql($@"
                create view [dbo].[GithubBranchesCodeSources] with schemabinding as
                    select
                      codeSourceId,
                      githubBranchId
                    from
                        [instances].[GithubBranchesCodeSources]
            ");

            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.GithubPullRequests;
            ");
            migrationBuilder.Sql($@"
                create view [dbo].[GithubPullRequests] with schemabinding as
                    select
                        id,
                        number,
                        isOpened,
                        openedAt,
                        mergedAt,
                        closedAt,
                        title,
                        originBranchId
                    from
                        [instances].[GithubPullRequests]
            ");

            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.GithubPullRequestsCodeSources;
            ");
            migrationBuilder.Sql($@"
                create view [dbo].[GithubPullRequestsCodeSources] with schemabinding as
                    select
                        codeSourceId,
                        githubPullRequestId
                    from
                        [instances].[GithubPullRequestsCodeSources]
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
