using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDestajoDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "DestajoProductions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DestajoDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientResponsible = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanyResponsible = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    table.PrimaryKey("PK_DestajoDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestajoDocuments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DestajoProductions_DocumentId",
                table: "DestajoProductions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DestajoDocuments_CompanyId",
                table: "DestajoDocuments",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DestajoProductions_DestajoDocuments_DocumentId",
                table: "DestajoProductions",
                column: "DocumentId",
                principalTable: "DestajoDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DestajoProductions_DestajoDocuments_DocumentId",
                table: "DestajoProductions");

            migrationBuilder.DropTable(
                name: "DestajoDocuments");

            migrationBuilder.DropIndex(
                name: "IX_DestajoProductions_DocumentId",
                table: "DestajoProductions");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DestajoProductions");
        }
    }
}
