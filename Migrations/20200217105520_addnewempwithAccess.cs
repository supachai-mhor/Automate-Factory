using Microsoft.EntityFrameworkCore.Migrations;

namespace AutomateBussiness.Migrations
{
    public partial class addnewempwithAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "accessType",
                table: "OrganizationTable",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accessType",
                table: "OrganizationTable");
        }
    }
}
