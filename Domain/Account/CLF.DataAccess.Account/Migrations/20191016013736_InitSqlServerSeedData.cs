using CLF.DataAccess.Account.DataInitial;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CLF.DataAccess.Account.Migrations
{
    public partial class InitSqlServerSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            DBSqlInitializer.Sql();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
