using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Instances.Infra.Migrations
{
    public partial class DemosInLowercase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    update instances.Demos set subdomain = lower(subdomain)
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
