using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class AddContactUserPropsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserFirstName",
                schema: "cafe",
                table: "SpecializedContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLastName",
                schema: "cafe",
                table: "SpecializedContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserMail",
                schema: "cafe",
                table: "SpecializedContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserFirstName",
                schema: "cafe",
                table: "ClientContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLastName",
                schema: "cafe",
                table: "ClientContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserMail",
                schema: "cafe",
                table: "ClientContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserFirstName",
                schema: "cafe",
                table: "AppContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLastName",
                schema: "cafe",
                table: "AppContacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserMail",
                schema: "cafe",
                table: "AppContacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserFirstName",
                schema: "cafe",
                table: "SpecializedContacts");

            migrationBuilder.DropColumn(
                name: "UserLastName",
                schema: "cafe",
                table: "SpecializedContacts");

            migrationBuilder.DropColumn(
                name: "UserMail",
                schema: "cafe",
                table: "SpecializedContacts");

            migrationBuilder.DropColumn(
                name: "UserFirstName",
                schema: "cafe",
                table: "ClientContacts");

            migrationBuilder.DropColumn(
                name: "UserLastName",
                schema: "cafe",
                table: "ClientContacts");

            migrationBuilder.DropColumn(
                name: "UserMail",
                schema: "cafe",
                table: "ClientContacts");

            migrationBuilder.DropColumn(
                name: "UserFirstName",
                schema: "cafe",
                table: "AppContacts");

            migrationBuilder.DropColumn(
                name: "UserLastName",
                schema: "cafe",
                table: "AppContacts");

            migrationBuilder.DropColumn(
                name: "UserMail",
                schema: "cafe",
                table: "AppContacts");
        }
    }
}
