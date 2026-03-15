using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OvertimeCalculator.Migrations
{
    /// <inheritdoc />
    public partial class AddCodigoAndIsActiveToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Código",
                table: "CalculationHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CalculationHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Código",
                table: "CalculationHistories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CalculationHistories");
        }
    }
}
