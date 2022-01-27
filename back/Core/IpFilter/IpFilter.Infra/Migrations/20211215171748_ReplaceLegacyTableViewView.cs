using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpFilter.Infra.Migrations
{
    public partial class ReplaceLegacyTableViewView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop table [dbo].[UserIpDevices]");
            migrationBuilder.Sql(@"
                create view [dbo].[UserIpDevices] as
                    select
                          Id Id
                        , UserId UserID
                        , IpAddress Ip
                        , Device
                        , CreatedAt
                        , ExpiresAt
                        from [shared].[IpFilterAuthorizations]
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
