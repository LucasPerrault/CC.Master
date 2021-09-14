using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class WaitForFulltextIndexing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

            CREATE   PROCEDURE [dbo].[WaitForFullTextIndexing]
					AS
					BEGIN
						DECLARE @statusSum int = 1;
						DECLARE @waitLoops int = 0;
						WHILE @statusSum > 0 AND @waitLoops < 600
						BEGIN
							SELECT @statusSum = SUM([status])
							FROM ( SELECT FULLTEXTCATALOGPROPERTY([name],'PopulateStatus') as [status]
									FROM sys.fulltext_catalogs) as FtStatuses
							-- 2 Paused
							-- 8 Disk Full. Paused
							-- 9 Change Tracking
							WHERE [status] NOT IN (2,8,9)


							IF @statusSum > 0
							BEGIN
								WAITFOR DELAY '00:00:00.01'; -- 1 second
							END
							SET @waitLoops = @waitLoops + 1;
						END
					END
GO
");
            migrationBuilder.Sql("EXEC dbo.WaitForFullTextIndexing;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
