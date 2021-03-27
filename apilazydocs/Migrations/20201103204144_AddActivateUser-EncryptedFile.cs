using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiLazyDoc.Migrations
{
    public partial class AddActivateUserEncryptedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activated",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FacebookId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Encrypted",
                table: "Files",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FacebookId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Encrypted",
                table: "Files");
        }
    }
}
