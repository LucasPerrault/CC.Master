using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class DemoTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTemplate",
                schema: "instances",
                table: "Demos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql($@"
                UPDATE instances.Demos
                set isTemplate=1
                where subdomain in ('masterdemo','virgindemo')
            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTemplate",
                schema: "instances",
                table: "Demos");
        }
    }
}
