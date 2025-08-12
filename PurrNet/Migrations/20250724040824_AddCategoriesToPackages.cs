using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Purrnet.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesToPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Categories",
                table: "Packages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Packages");
        }
    }
}
