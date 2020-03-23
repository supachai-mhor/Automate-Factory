using Microsoft.EntityFrameworkCore.Migrations;

namespace AutomateBussiness.Migrations
{
    public partial class newChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "messageStatus",
                table: "ChatHistorysTable");

            migrationBuilder.AddColumn<int>(
                name: "receiverMessageStatus",
                table: "ChatHistorysTable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "senderMessageStatus",
                table: "ChatHistorysTable",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "receiverMessageStatus",
                table: "ChatHistorysTable");

            migrationBuilder.DropColumn(
                name: "senderMessageStatus",
                table: "ChatHistorysTable");

            migrationBuilder.AddColumn<int>(
                name: "messageStatus",
                table: "ChatHistorysTable",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
