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
                ('Figgo'),
                ('Timmi Timesheet'),
                ('Timmi Project'),
                ('Cleemy Expenses'),
                ('Poplee Entretiens & Objectifs'),
                ('Pagga'),
                ('Poplee REM'),
                ('Poplee Core RH'),
                ('Essentiel'),
                ('SIRH et paie'),
                ('Suite Startup'),
                ('Bloom')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
