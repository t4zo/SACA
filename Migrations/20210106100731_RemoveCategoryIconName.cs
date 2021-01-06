using Microsoft.EntityFrameworkCore.Migrations;

namespace SACA.Migrations
{
    public partial class RemoveCategoryIconName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "icon_name",
                table: "categories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "icon_name",
                table: "categories",
                type: "text",
                nullable: true);
        }
    }
}
