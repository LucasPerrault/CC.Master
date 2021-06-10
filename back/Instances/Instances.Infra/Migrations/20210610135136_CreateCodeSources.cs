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
                ALTER SCHEMA instances TRANSFER dbo.CodeSourceConfigs;
            ");
            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.CodeSourcesProductionVersions;
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
