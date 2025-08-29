using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FizzBuzzGame.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRuleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Rules",
                newName: "Divisor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Divisor",
                table: "Rules",
                newName: "Number");
        }
    }
}
