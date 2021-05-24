using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class CreateBusinessUnitsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusinessUnitId",
                schema: "billing",
                table: "Solutions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BusinessUnit",
                schema: "billing",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_BusinessUnitId",
                schema: "billing",
                table: "Solutions",
                column: "BusinessUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_BusinessUnit_BusinessUnitId",
                schema: "billing",
                table: "Solutions",
                column: "BusinessUnitId",
                principalSchema: "billing",
                principalTable: "BusinessUnit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_BusinessUnit_BusinessUnitId",
                schema: "billing",
                table: "Solutions");

            migrationBuilder.DropTable(
                name: "BusinessUnit",
                schema: "billing");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_BusinessUnitId",
                schema: "billing",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "BusinessUnitId",
                schema: "billing",
                table: "Solutions");
        }
    }
}
