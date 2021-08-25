using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class InitAdvancedFilters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cafe");

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<Guid>(maxLength: 36, nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.UniqueConstraint("AK_Clients_ExternalId", x => x.ExternalId);
                    table.UniqueConstraint("AK_Clients_RemoteId", x => x.RemoteId);
                });

            migrationBuilder.CreateTable(
                name: "Environments",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    Subdomain = table.Column<string>(nullable: false),
                    Domain = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environments", x => x.Id);
                    table.UniqueConstraint("AK_Environments_RemoteId", x => x.RemoteId);
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentContracts",
                schema: "cafe",
                columns: table => new
                {
                    EstablishmentId = table.Column<int>(nullable: false),
                    ContractId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentContracts", x => new { x.ContractId, x.EstablishmentId });
                });

            migrationBuilder.CreateTable(
                name: "SpecializedContacts",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializedContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientContacts",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientContacts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cafe",
                        principalTable: "Clients",
                        principalColumn: "ExternalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<Guid>(maxLength: 36, nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    ClientId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cafe",
                        principalTable: "Clients",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId1",
                        column: x => x.ClientId1,
                        principalSchema: "cafe",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppInstances",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ApplicationId = table.Column<string>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    DeleteAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInstances", x => x.Id);
                    table.UniqueConstraint("AK_AppInstances_RemoteId", x => x.RemoteId);
                    table.ForeignKey(
                        name: "FK_AppInstances_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LegalUnits",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    LegalIdentificationNumber = table.Column<string>(nullable: true),
                    ActivityCode = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    HeadquartersId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    EnvironmentId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalUnits", x => x.Id);
                    table.UniqueConstraint("AK_LegalUnits_RemoteId", x => x.RemoteId);
                    table.ForeignKey(
                        name: "FK_LegalUnits_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalUnits_Environments_EnvironmentId1",
                        column: x => x.EnvironmentId1,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppContacts",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    AppInstanceId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppContacts_AppInstances_AppInstanceId",
                        column: x => x.AppInstanceId,
                        principalSchema: "cafe",
                        principalTable: "AppInstances",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Establishments",
                schema: "cafe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemoteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    LegalUnitId = table.Column<int>(nullable: false),
                    LegalIdentificationNumber = table.Column<string>(nullable: true),
                    ActivityCode = table.Column<string>(nullable: true),
                    TimeZoneId = table.Column<string>(nullable: false),
                    UsersCount = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    LegalUnitId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establishments_LegalUnits_LegalUnitId",
                        column: x => x.LegalUnitId,
                        principalSchema: "cafe",
                        principalTable: "LegalUnits",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Establishments_LegalUnits_LegalUnitId1",
                        column: x => x.LegalUnitId1,
                        principalSchema: "cafe",
                        principalTable: "LegalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppContacts_AppInstanceId",
                schema: "cafe",
                table: "AppContacts",
                column: "AppInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInstances_EnvironmentId",
                schema: "cafe",
                table: "AppInstances",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInstances_RemoteId",
                schema: "cafe",
                table: "AppInstances",
                column: "RemoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ClientId",
                schema: "cafe",
                table: "ClientContacts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ExternalId",
                schema: "cafe",
                table: "Clients",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_RemoteId",
                schema: "cafe",
                table: "Clients",
                column: "RemoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId",
                schema: "cafe",
                table: "Contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId1",
                schema: "cafe",
                table: "Contracts",
                column: "ClientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Environments_RemoteId",
                schema: "cafe",
                table: "Environments",
                column: "RemoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Establishments_LegalUnitId",
                schema: "cafe",
                table: "Establishments",
                column: "LegalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Establishments_LegalUnitId1",
                schema: "cafe",
                table: "Establishments",
                column: "LegalUnitId1");

            migrationBuilder.CreateIndex(
                name: "IX_Establishments_RemoteId",
                schema: "cafe",
                table: "Establishments",
                column: "RemoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LegalUnits_EnvironmentId",
                schema: "cafe",
                table: "LegalUnits",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalUnits_EnvironmentId1",
                schema: "cafe",
                table: "LegalUnits",
                column: "EnvironmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_LegalUnits_RemoteId",
                schema: "cafe",
                table: "LegalUnits",
                column: "RemoteId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppContacts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "ClientContacts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "EstablishmentContracts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Establishments",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "SpecializedContacts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "AppInstances",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "LegalUnits",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Environments",
                schema: "cafe");
        }
    }
}
