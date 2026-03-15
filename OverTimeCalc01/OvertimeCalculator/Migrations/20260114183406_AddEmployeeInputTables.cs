using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OvertimeCalculator.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeInputTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeInputJson",
                table: "CalculationHistories");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeInputHeaderId",
                table: "CalculationHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmployeeInputHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Compañía = table.Column<int>(type: "int", nullable: false),
                    TipoDeDía = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDeHorario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeInputHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeInputDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeaderId = table.Column<int>(type: "int", nullable: false),
                    IdMarcacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Horas = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeInputDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeInputDetails_EmployeeInputHeaders_HeaderId",
                        column: x => x.HeaderId,
                        principalTable: "EmployeeInputHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationHistories_EmployeeInputHeaderId",
                table: "CalculationHistories",
                column: "EmployeeInputHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeInputDetails_HeaderId",
                table: "EmployeeInputDetails",
                column: "HeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalculationHistories_EmployeeInputHeaders_EmployeeInputHeaderId",
                table: "CalculationHistories",
                column: "EmployeeInputHeaderId",
                principalTable: "EmployeeInputHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalculationHistories_EmployeeInputHeaders_EmployeeInputHeaderId",
                table: "CalculationHistories");

            migrationBuilder.DropTable(
                name: "EmployeeInputDetails");

            migrationBuilder.DropTable(
                name: "EmployeeInputHeaders");

            migrationBuilder.DropIndex(
                name: "IX_CalculationHistories_EmployeeInputHeaderId",
                table: "CalculationHistories");

            migrationBuilder.DropColumn(
                name: "EmployeeInputHeaderId",
                table: "CalculationHistories");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeInputJson",
                table: "CalculationHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
