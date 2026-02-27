using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace imageboard.Migrations
{
    /// <inheritdoc />
    public partial class AddTagUseCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UseCount",
                table: "Tags",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseCount",
                table: "Tags");
        }
    }
}
