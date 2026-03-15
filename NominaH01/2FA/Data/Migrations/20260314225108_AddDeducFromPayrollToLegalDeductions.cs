using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeducFromPayrollToLegalDeductions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeducFromPayroll",
                table: "LegalDeductions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql(@"
                UPDATE LegalDeductions
                SET DeducFromPayroll = 0
                WHERE Code = 'RIEGOP';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeducFromPayroll",
                table: "LegalDeductions");
        }
    }
}
