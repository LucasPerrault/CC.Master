using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Environments.Infra.Migrations
{
    public partial class EnvironmentLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [shared].[EnvironmentLogs] WITH SCHEMABINDING AS
                    SELECT
                       l.[id] id
                      ,l.[userId] userId
                      ,l.[environmentId] environmentId
                      ,l.[activityId] activityId
                      ,l.[createdOn] createdOn
                      ,l.[IsAnonymizedData] IsAnonymizedData
                FROM [dbo].[EnvironmentLogs] l
            ");

            migrationBuilder.Sql($@"
                CREATE VIEW [shared].[EnvironmentLogMessages] WITH SCHEMABINDING AS
                    SELECT
                       m.[id] id
                      ,m.[userId] userId
                      ,m.[environmentLogId] environmentLogId
                      ,m.[createdOn] createdOn
                      ,m.[expiredOn] expiredOn
                      ,m.[message] message
                      ,m.[type] type
                FROM [dbo].[EnvironmentLogMessages] m
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [shared].[EnvironmentLogMessages]
            ");

            migrationBuilder.Sql($@"
                DROP VIEW [shared].[EnvironmentLogs]
            ");
        }
    }
}
