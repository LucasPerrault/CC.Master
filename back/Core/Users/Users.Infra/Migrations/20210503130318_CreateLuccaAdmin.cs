using Microsoft.EntityFrameworkCore.Migrations;

namespace Users.Infra.Migrations
{
    public partial class CreateLuccaAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                    @"INSERT INTO [shared].[Users]
                    ([PartenairesId]
                    ,[FirstName]
                    ,[LastName]
                    ,[DepartmentId]
                    ,[IsActive])
                VALUES(0,'Lucca','Admin',22,1)"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
