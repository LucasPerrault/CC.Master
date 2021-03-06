using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class AddAuthorReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Demos_authorId",
                schema: "instances",
                table: "Demos",
                column: "authorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Demos_Users_authorId",
                schema: "instances",
                table: "Demos",
                column: "authorId",
                principalSchema: "shared",
                principalTable: "Users",
                principalColumn: "PartenairesId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Demos_Users_authorId",
                schema: "instances",
                table: "Demos");

            migrationBuilder.DropIndex(
                name: "IX_Demos_authorId",
                schema: "instances",
                table: "Demos");
        }
    }
}
