using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddBillingCatalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE FULLTEXT CATALOG [billingCatalog] WITH ACCENT_SENSITIVITY = OFF;", true);

            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[Clients] WITH SCHEMABINDING AS
                    SELECT
                        c.[Id] Id,
                        c.[ExternalId] ExternalId,
                        c.[Name] Name,
                        c.[SocialReason] SocialReason,
                        c.[SalesforceId] SalesforceId,
                        c.[BillingStreet] BillingStreet,
                        c.[BillingPostalCode] BillingPostalCode,
                        c.[BillingCity] BillingCity,
                        c.[BillingState] BillingState,
                        c.[BillingCountry] BillingCountry,
                        c.[BillingMail] BillingMail,
                        c.[Phone] Phone
                FROM [dbo].[Clients] c
            ");
            migrationBuilder.Sql("CREATE UNIQUE CLUSTERED INDEX PK_Clients ON [billing].[clients] (id);");


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
            migrationBuilder.Sql("CREATE FULLTEXT INDEX ON billing.Clients(name, socialReason) KEY INDEX PK_Clients ON [billingCatalog];", true);
            migrationBuilder.Sql("EXEC dbo.WaitForFullTextIndexing;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
