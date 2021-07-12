using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class CodeSourceArtifacts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeSourceArtifacts",
                schema: "instances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeSourceId = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(maxLength: 255, nullable: true),
                    ArtifactUrl = table.Column<string>(maxLength: 255, nullable: true),
                    ArtifactType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeSourceArtifacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeSourceArtifacts_CodeSources_CodeSourceId",
                        column: x => x.CodeSourceId,
                        principalSchema: "instances",
                        principalTable: "CodeSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeSourceArtifacts_CodeSourceId",
                schema: "instances",
                table: "CodeSourceArtifacts",
                column: "CodeSourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "CodeSourceArtifacts",
                schema: "instances");
        }
    }
}
