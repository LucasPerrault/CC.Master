﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Users.Infra.Migrations
{
    public partial class CreateTableUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "shared",
                columns: table => new
                {
                    TechnicalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartenairesId = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 300, nullable: true),
                    LastName = table.Column<string>(maxLength: 300, nullable: true),
                    DepartmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.TechnicalId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PartenairesId",
                schema: "shared",
                table: "Users",
                column: "PartenairesId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "shared");
        }
    }
}
