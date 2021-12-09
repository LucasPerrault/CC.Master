using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class MoveOffersSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"update dbo.CommercialOffers set PricingMethod = 1 where PricingMethod = 'Constant'");
            migrationBuilder.Sql(@$"update dbo.CommercialOffers set PricingMethod = 2 where PricingMethod = 'Linear'");
            migrationBuilder.Sql(@$"update dbo.CommercialOffers set PricingMethod = 3 where PricingMethod = 'AnnualCommitment'");
            migrationBuilder.Sql(@$"alter table dbo.CommercialOffers alter column PricingMethod int not null");

            migrationBuilder.Sql(@$"update dbo.CommercialOffers set ForecastMethod = 1 where ForecastMethod = 'LastRealMonth'");
            migrationBuilder.Sql(@$"update dbo.CommercialOffers set ForecastMethod = 2 where ForecastMethod = 'LastYear'");
            migrationBuilder.Sql(@$"update dbo.CommercialOffers set ForecastMethod = 3 where ForecastMethod = 'AnnualCommitment'");
            migrationBuilder.Sql(@$"alter table dbo.CommercialOffers alter column ForecastMethod int not null");

            migrationBuilder.Sql(@"
                ALTER SCHEMA billing TRANSFER dbo.PriceRows;
                ALTER SCHEMA billing TRANSFER dbo.PriceLists;
                EXEC sp_rename 'dbo.CommercialOffers.idCommercialOffer', 'id', 'COLUMN';
                EXEC sp_rename 'dbo.CommercialOffers.idCurrency', 'currencyId', 'COLUMN';
                ALTER SCHEMA billing TRANSFER dbo.CommercialOffers;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW dbo.CommercialOffers WITH SCHEMABINDING AS
                    select id as idCommercialOffer,
                        name,
                        productId,
                        unit,
                        billingMode,
                        currencyId idCurrency,
                        tag,
                        case PricingMethod
                            when 1 then 'Constant'
                            when 2 then 'Linear'
                            when 3 then 'AnnualCommitment'
                            END PricingMethod,
                        case ForecastMethod
                            when 1 then 'LastRealMonth'
                            when 2 then 'LastYear'
                            when 3 then 'AnnualCommitment'
                            END ForecastMethod,
                        isArchived
                    from billing.CommercialOffers;
            ");
            migrationBuilder.Sql(@"
                CREATE VIEW dbo.PriceLists WITH SCHEMABINDING as select id, offerId, startsOn from billing.PriceLists;
            ");
            migrationBuilder.Sql(@"
                CREATE VIEW dbo.PriceRows WITH SCHEMABINDING as select id, listId, maxIncludedCount, unitPrice, fixedPrice from billing.PriceRows;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
