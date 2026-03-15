using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDestajoPrecisionAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DestajoAmount",
                table: "PayrollTmpEmployees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitValue",
                table: "PayrollTmpEmployees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitsProduced",
                table: "PayrollTmpEmployees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDestajo",
                table: "PayrollHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "PayrollHeaders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitValue",
                table: "PayrollHeaders",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DestajoProductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitsProduced = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    UnitValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestajoProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestajoProductions_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DestajoProductions_EmployeeId_ProductionDate",
                table: "DestajoProductions",
                columns: new[] { "EmployeeId", "ProductionDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DestajoProductions");

            migrationBuilder.DropColumn(
                name: "DestajoAmount",
                table: "PayrollTmpEmployees");

            migrationBuilder.DropColumn(
                name: "UnitValue",
                table: "PayrollTmpEmployees");

            migrationBuilder.DropColumn(
                name: "UnitsProduced",
                table: "PayrollTmpEmployees");

            migrationBuilder.DropColumn(
                name: "IsDestajo",
                table: "PayrollHeaders");

            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "PayrollHeaders");

            migrationBuilder.DropColumn(
                name: "UnitValue",
                table: "PayrollHeaders");
        }
    }
}
