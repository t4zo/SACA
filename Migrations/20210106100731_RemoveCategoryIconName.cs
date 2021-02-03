using Microsoft.EntityFrameworkCore.Migrations;

namespace SACA.Migrations
{
    public partial class RemoveCategoryIconName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "icon_name",
                "categories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "icon_name",
                "categories",
                "text",
                nullable: true);
        }
    }
}