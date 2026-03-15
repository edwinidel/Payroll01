using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Migrations
{
    /// <inheritdoc />
    public partial class AddingPayingBankIdandPaymentMethodfieldtoEmployeeEntity1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PayingBankId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PayingBankId",
                table: "Employees",
                column: "PayingBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Banks_PayingBankId",
                table: "Employees",
                column: "PayingBankId",
                principalTable: "Banks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Banks_PayingBankId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PayingBankId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PayingBankId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Employees");
        }
    }
}
