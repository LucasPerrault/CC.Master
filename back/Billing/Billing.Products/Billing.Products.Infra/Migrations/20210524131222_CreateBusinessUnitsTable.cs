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

            migrationBuilder.Sql(@"
                INSERT INTO [CLOUDCONTROL].[billing].[BusinessUnit]
                VALUES
                ('GTA'),
                ('Finance'),
                ('Talent'),
                ('Pay'),
                ('Core'),
                ('Engagement'),
                ('Autres')
                ");

            migrationBuilder.Sql(@"
                UPDATE [CLOUDCONTROL].[billing].[Solutions]
                SET BusinessUnitId = (
                CASE [Solutions].name
                       WHEN 'Figgo' THEN 1
                       WHEN 'Cleemy' THEN 2
                       WHEN 'Pagga' THEN 4
                       WHEN 'Pay Manager' THEN 7
                       WHEN 'Poplee Core RH' THEN 5
                       WHEN 'Portail' THEN 7
                       WHEN 'Spensy' THEN 7
                       WHEN 'Urba' THEN 7
                       WHEN 'Timesheet (old)' THEN 1
                       WHEN 'Ucal' THEN 7
                       WHEN 'Planner' THEN 7
                       WHEN 'Calendar' THEN 7
                       WHEN 'Autres' THEN 7
                       WHEN 'Admin' THEN 7
                       WHEN 'Project Manager' THEN 7
                       WHEN 'Timmi Timesheet' THEN 1
                       WHEN 'Anytime' THEN 2
                       WHEN 'Budget Insight' THEN 2
                       WHEN 'Timmi Project' THEN 1
                       WHEN 'Poplee REM' THEN 4
                       WHEN 'Dématérialisation' THEN 2
                       WHEN 'Synchronisation Figgo/GXP' THEN 1
                       WHEN 'Poplee Entretiens & Objectifs' THEN 3
                       WHEN 'Signature Electronique' THEN 5
                END
                )
                ");

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
