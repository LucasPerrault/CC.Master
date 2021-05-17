using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class AddForeignKeyProductFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FamilyId",
                schema: "billing",
                table: "Products",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Products_FamilyId",
                schema: "billing",
                table: "Products",
                column: "FamilyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductFamilies_FamilyId",
                schema: "billing",
                table: "Products",
                column: "FamilyId",
                principalSchema: "billing",
                principalTable: "ProductFamilies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(@"
                update billing.Products Set FamilyId = 2 where Id in (31)
                update billing.Products Set FamilyId = 3 where Id in (1, 23)
                update billing.Products Set FamilyId = 4 where Id in (2, 18, 19, 22, 32, 34)
                update billing.Products Set FamilyId = 5 where Id in (3)
                update billing.Products Set FamilyId = 6 where Id in (5, 21, 24, 33)
                update billing.Products Set FamilyId = 7 where Id in (17, 20)
                update billing.Products Set FamilyId = 8 where Id in (25)
                update billing.Products Set FamilyId = 9 where Id in (26, 30)
                update billing.Products Set FamilyId = 10 where Id in (27)"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductFamilies_FamilyId",
                schema: "billing",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_FamilyId",
                schema: "billing",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                schema: "billing",
                table: "Products");
        }
    }
}
