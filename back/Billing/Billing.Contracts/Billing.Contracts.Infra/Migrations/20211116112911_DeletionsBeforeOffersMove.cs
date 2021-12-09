using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class DeletionsBeforeOffersMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop view billing.CommercialOffers; -- will be replaced by the table
            ");
            migrationBuilder.Sql(@"
                alter table Counts drop constraint FK_Counts_OFFER;
            ");
            migrationBuilder.Sql(@"
                alter table [dbo].[CommercialOffers] drop constraint [DF__Commercia__Forec__0C1BC9F9];
            ");
            migrationBuilder.Sql(@"
                drop view billing.CmrrContracts;
            ");
            migrationBuilder.Sql(@"
                drop view Contract_LegalEntityErrorNumber;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
