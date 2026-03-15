using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OvertimeCalculator.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationHistories_EmployeeInputHeaders_EmployeeInputHeaderId",
                table: "CalculationHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Código",
                table: "CalculationHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CalculationHistory_Código",
                table: "CalculationHistories",
                column: "Código");

            migrationBuilder.CreateIndex(
                name: "IX_CalculationHistory_Código_FechaMarcacion",
                table: "CalculationHistories",
                columns: new[] { "Código", "FechaMarcacion" });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationHistory_FechaMarcacion",
                table: "CalculationHistories",
                column: "FechaMarcacion");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationHistories_EmployeeInputHeaders_EmployeeInputHeaderId",
                table: "CalculationHistories",
                column: "EmployeeInputHeaderId",
                principalTable: "EmployeeInputHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationHistories_EmployeeInputHeaders_EmployeeInputHeaderId",
                table: "CalculationHistories");

            migrationBuilder.DropIndex(
                name: "IX_CalculationHistory_Código",
                table: "CalculationHistories");

            migrationBuilder.DropIndex(
                name: "IX_CalculationHistory_Código_FechaMarcacion",
                table: "CalculationHistories");

            migrationBuilder.DropIndex(
                name: "IX_CalculationHistory_FechaMarcacion",
                table: "CalculationHistories");

            migrationBuilder.AlterColumn<string>(
                name: "Código",
                table: "CalculationHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationHistories_EmployeeInputHeaders_EmployeeInputHeaderId",
                table: "CalculationHistories",
                column: "EmployeeInputHeaderId",
                principalTable: "EmployeeInputHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
