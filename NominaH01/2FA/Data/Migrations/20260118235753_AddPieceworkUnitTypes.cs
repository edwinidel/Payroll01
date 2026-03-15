using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPieceworkUnitTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PieceworkUnitTypeId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PieceworkUnitTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_PieceworkUnitTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PieceworkUnitTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PieceworkUnitTypeId",
                table: "Employees",
                column: "PieceworkUnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PieceworkUnitTypes_CompanyId",
                table: "PieceworkUnitTypes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_PieceworkUnitTypes_PieceworkUnitTypeId",
                table: "Employees",
                column: "PieceworkUnitTypeId",
                principalTable: "PieceworkUnitTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_PieceworkUnitTypes_PieceworkUnitTypeId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "PieceworkUnitTypes");

            migrationBuilder.DropIndex(
                name: "IX_Employees_PieceworkUnitTypeId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PieceworkUnitTypeId",
                table: "Employees");
        }
    }
}
