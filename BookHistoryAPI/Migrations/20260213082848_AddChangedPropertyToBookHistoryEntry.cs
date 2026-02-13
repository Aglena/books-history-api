using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookHistoryApi.Migrations
{
    /// <inheritdoc />
    public partial class AddChangedPropertyToBookHistoryEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChangedProperty",
                table: "BookChangeHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedProperty",
                table: "BookChangeHistories");
        }
    }
}
