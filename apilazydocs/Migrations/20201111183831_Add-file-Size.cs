using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiLazyDoc.Migrations
{
    public partial class AddfileSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Size",
                table: "Files",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Files");
        }
    }
}
