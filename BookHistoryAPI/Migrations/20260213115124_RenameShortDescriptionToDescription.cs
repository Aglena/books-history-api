using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHistoryApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameShortDescriptionToDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "Books",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Books",
                newName: "ShortDescription");
        }
    }
}
