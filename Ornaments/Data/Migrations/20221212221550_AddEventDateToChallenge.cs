using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ornaments.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventDateToChallenge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "Challenges",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Challenges",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Challenges");
        }
    }
}
