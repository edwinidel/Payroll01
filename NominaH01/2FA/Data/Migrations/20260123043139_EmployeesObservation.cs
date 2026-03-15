using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeesObservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeObservationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ObservationTypeId = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_EmployeeObservationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeObservationHistories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeObservationHistories_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeObservationHistories_ObservationTypes_ObservationTypeId",
                        column: x => x.ObservationTypeId,
                        principalTable: "ObservationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeObservationHistories_CompanyId",
                table: "EmployeeObservationHistories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeObservationHistories_EmployeeId",
                table: "EmployeeObservationHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeObservationHistories_ObservationTypeId",
                table: "EmployeeObservationHistories",
                column: "ObservationTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeObservationHistories");
        }
    }
}
