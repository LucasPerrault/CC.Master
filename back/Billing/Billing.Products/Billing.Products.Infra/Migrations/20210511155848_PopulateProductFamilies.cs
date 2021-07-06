using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class PopulateProductFamilies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                insert into billing.ProductFamilies (name) 
                values
                ('Autres'),
                ('Gratuits'),
                ('Figgo'),
                ('Cleemy'),
                ('Pagga'),
                ('Poplee'),
                ('Timmi'),
                ('Lucca pour la Paie'),
                ('Essentiel SIRH'),
                ('Startup'),
                ('Bloom')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
