using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingfieldstoEmployeeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiOtcPassword",
                table: "Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiOtcUsername",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiOtcPassword",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ApiOtcUsername",
                table: "Employees");
        }
    }
}
