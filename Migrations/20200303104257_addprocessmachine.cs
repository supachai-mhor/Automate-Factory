using Microsoft.EntityFrameworkCore.Migrations;

namespace AutomateBussiness.Migrations
{
    public partial class addprocessmachine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "process",
                table: "MachineTable",
                nullable: false,
                defaultValue: "");

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "process",
                table: "MachineTable");
        }
    }
}
