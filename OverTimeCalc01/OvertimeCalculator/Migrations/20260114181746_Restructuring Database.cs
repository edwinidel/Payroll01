using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OvertimeCalculator.Migrations
{
    /// <inheritdoc />
    public partial class RestructuringDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    EmployeeInputJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalculationResultJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaMarcacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Compañía = table.Column<int>(type: "int", nullable: false),
                    TipoDeDía = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDeHorario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalHorasRegulares = table.Column<double>(type: "float", nullable: false),
                    TotalHorasExtras = table.Column<double>(type: "float", nullable: false),
                    Tardanza = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationHistories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
