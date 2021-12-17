using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpFilter.Infra.Migrations
{
    public partial class FillTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                insert into [shared].[IpFilterAuthorizations]
                            (UserID, IpAddress, Device, CreatedAt, ExpiresAt, RequestId)
                select UserId, Ip, Device, CreatedAt, ExpiresAt, null
                from [dbo].[UserIpDevices]
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
