using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsContractorfieldtoEmployeeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsContractor",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsContractor",
                table: "Employees");
        }
    }
}
