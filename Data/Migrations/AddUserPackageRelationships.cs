using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace furnet.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPackageRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add OwnerId column to Packages table
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Packages",
                type: "INTEGER",
                nullable: true);

            // Add ApprovalStatus and RejectionReason columns
            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "Packages",
                type: "TEXT",
                nullable: false,
                defaultValue: "Approved");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Packages",
                type: "TEXT",
                nullable: true);

            // Create PackageMaintainer join table
            migrationBuilder.CreateTable(
                name: "PackageMaintainer",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageMaintainer", x => new { x.PackageId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PackageMaintainer_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageMaintainer_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create foreign key constraint for Package.OwnerId
            migrationBuilder.CreateIndex(
                name: "IX_Packages_OwnerId",
                table: "Packages",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageMaintainer_UserId",
                table: "PackageMaintainer",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Users_OwnerId",
                table: "Packages",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Users_OwnerId",
                table: "Packages");

            migrationBuilder.DropTable(
                name: "PackageMaintainer");

            migrationBuilder.DropIndex(
                name: "IX_Packages_OwnerId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Packages");
        }
    }
}
