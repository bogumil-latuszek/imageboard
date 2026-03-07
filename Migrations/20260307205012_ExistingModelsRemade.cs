using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace imageboard.Migrations
{
    /// <inheritdoc />
    public partial class ExistingModelsRemade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_Uploader",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Uploader",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Items",
                newName: "FileType");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "Items",
                newName: "Hash");

            migrationBuilder.AddColumn<int>(
                name: "UploaderId",
                table: "Items",
                type: "INTEGER",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Hash",
                table: "Items",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UploaderId",
                table: "Items",
                column: "UploaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_Hash",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UploaderId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UploaderId",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Hash",
                table: "Items",
                newName: "Tags");

            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "Items",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Items",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Uploader",
                table: "Items",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Uploader",
                table: "Items",
                column: "Uploader");
        }
    }
}
