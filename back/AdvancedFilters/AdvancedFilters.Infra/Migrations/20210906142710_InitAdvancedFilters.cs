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
                    RemoteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(maxLength: 36, nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.RemoteId);
                    table.UniqueConstraint("AK_Clients_ExternalId", x => x.ExternalId);
                });

            migrationBuilder.CreateTable(
                name: "Environments",
                schema: "cafe",
                columns: table => new
                {
                    RemoteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subdomain = table.Column<string>(nullable: false),
                    Domain = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environments", x => x.RemoteId);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "cafe",
                columns: table => new
                {
                    RemoteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<Guid>(maxLength: 36, nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    ClientRemoteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.RemoteId);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cafe",
                        principalTable: "Clients",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientRemoteId",
                        column: x => x.ClientRemoteId,
                        principalSchema: "cafe",
                        principalTable: "Clients",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppInstances",
                schema: "cafe",
                columns: table => new
                {
                    RemoteId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ApplicationId = table.Column<string>(nullable: false),
                    DeleteAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInstances", x => new { x.EnvironmentId, x.RemoteId });
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
                    EnvironmentRemoteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalUnits", x => new { x.EnvironmentId, x.RemoteId });
                    table.ForeignKey(
                        name: "FK_LegalUnits_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LegalUnits_Environments_EnvironmentRemoteId",
                        column: x => x.EnvironmentRemoteId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Establishments",
                schema: "cafe",
                columns: table => new
                {
                    RemoteId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    LegalUnitId = table.Column<int>(nullable: false),
                    LegalIdentificationNumber = table.Column<string>(nullable: true),
                    ActivityCode = table.Column<string>(nullable: true),
                    TimeZoneId = table.Column<string>(nullable: false),
                    UsersCount = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: false),
                    LegalUnitEnvironmentId = table.Column<int>(nullable: true),
                    LegalUnitRemoteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishments", x => new { x.EnvironmentId, x.RemoteId });
                    table.ForeignKey(
                        name: "FK_Establishments_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Establishments_LegalUnits_EnvironmentId_LegalUnitId",
                        columns: x => new { x.EnvironmentId, x.LegalUnitId },
                        principalSchema: "cafe",
                        principalTable: "LegalUnits",
                        principalColumns: new[] { "EnvironmentId", "RemoteId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Establishments_LegalUnits_LegalUnitEnvironmentId_LegalUnitRemoteId",
                        columns: x => new { x.LegalUnitEnvironmentId, x.LegalUnitRemoteId },
                        principalSchema: "cafe",
                        principalTable: "LegalUnits",
                        principalColumns: new[] { "EnvironmentId", "RemoteId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppContacts",
                schema: "cafe",
                columns: table => new
                {
                    RemoteId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    AppInstanceId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    EstablishmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppContacts", x => new { x.EnvironmentId, x.RemoteId });
                    table.ForeignKey(
                        name: "FK_AppContacts_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppContacts_AppInstances_EnvironmentId_AppInstanceId",
                        columns: x => new { x.EnvironmentId, x.AppInstanceId },
                        principalSchema: "cafe",
                        principalTable: "AppInstances",
                        principalColumns: new[] { "EnvironmentId", "RemoteId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppContacts_Establishments_EnvironmentId_EstablishmentId",
                        columns: x => new { x.EnvironmentId, x.EstablishmentId },
                        principalSchema: "cafe",
                        principalTable: "Establishments",
                        principalColumns: new[] { "EnvironmentId", "RemoteId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientContacts",
                schema: "cafe",
                columns: table => new
                {
                    RemoteId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    EstablishmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContacts", x => new { x.EnvironmentId, x.RemoteId });
                    table.ForeignKey(
                        name: "FK_ClientContacts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cafe",
                        principalTable: "Clients",
                        principalColumn: "ExternalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientContacts_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientContacts_Establishments_EnvironmentId_EstablishmentId",
                        columns: x => new { x.EnvironmentId, x.EstablishmentId },
                        principalSchema: "cafe",
                        principalTable: "Establishments",
                        principalColumns: new[] { "EnvironmentId", "RemoteId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstablishmentContracts",
                schema: "cafe",
                columns: table => new
                {
                    ContractId = table.Column<int>(nullable: false),
                    EstablishmentId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    ContractRemoteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablishmentContracts", x => new { x.EnvironmentId, x.EstablishmentId, x.ContractId });
                    table.ForeignKey(
                        name: "FK_EstablishmentContracts_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "cafe",
                        principalTable: "Contracts",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablishmentContracts_Contracts_ContractRemoteId",
                        column: x => x.ContractRemoteId,
                        principalSchema: "cafe",
                        principalTable: "Contracts",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstablishmentContracts_Establishments_EnvironmentId_EstablishmentId",
                        columns: x => new { x.EnvironmentId, x.EstablishmentId },
                        principalSchema: "cafe",
                        principalTable: "Establishments",
                        principalColumns: new[] { "EnvironmentId", "RemoteId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecializedContacts",
                schema: "cafe",
                columns: table => new
                {
                    RemoteId = table.Column<int>(nullable: false),
                    EnvironmentId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    EstablishmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecializedContacts", x => new { x.EnvironmentId, x.RemoteId });
                    table.ForeignKey(
                        name: "FK_SpecializedContacts_Environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalSchema: "cafe",
                        principalTable: "Environments",
                        principalColumn: "RemoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecializedContacts_Establishments_EnvironmentId_EstablishmentId",
                        columns: x => new { x.EnvironmentId, x.EstablishmentId },
                        principalSchema: "cafe",
                        principalTable: "Establishments",
                        principalColumns: new[] { "EnvironmentId", "RemoteId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppContacts_EnvironmentId_AppInstanceId",
                schema: "cafe",
                table: "AppContacts",
                columns: new[] { "EnvironmentId", "AppInstanceId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppContacts_EnvironmentId_EstablishmentId",
                schema: "cafe",
                table: "AppContacts",
                columns: new[] { "EnvironmentId", "EstablishmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ClientId",
                schema: "cafe",
                table: "ClientContacts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_EnvironmentId_EstablishmentId",
                schema: "cafe",
                table: "ClientContacts",
                columns: new[] { "EnvironmentId", "EstablishmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ExternalId",
                schema: "cafe",
                table: "Clients",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId",
                schema: "cafe",
                table: "Contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientRemoteId",
                schema: "cafe",
                table: "Contracts",
                column: "ClientRemoteId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentContracts_ContractId",
                schema: "cafe",
                table: "EstablishmentContracts",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablishmentContracts_ContractRemoteId",
                schema: "cafe",
                table: "EstablishmentContracts",
                column: "ContractRemoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Establishments_EnvironmentId_LegalUnitId",
                schema: "cafe",
                table: "Establishments",
                columns: new[] { "EnvironmentId", "LegalUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_Establishments_LegalUnitEnvironmentId_LegalUnitRemoteId",
                schema: "cafe",
                table: "Establishments",
                columns: new[] { "LegalUnitEnvironmentId", "LegalUnitRemoteId" });

            migrationBuilder.CreateIndex(
                name: "IX_LegalUnits_EnvironmentRemoteId",
                schema: "cafe",
                table: "LegalUnits",
                column: "EnvironmentRemoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecializedContacts_EnvironmentId_EstablishmentId",
                schema: "cafe",
                table: "SpecializedContacts",
                columns: new[] { "EnvironmentId", "EstablishmentId" });
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
                name: "EstablishmentContracts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "SpecializedContacts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "AppInstances",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "cafe");

            migrationBuilder.DropTable(
                name: "Establishments",
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
