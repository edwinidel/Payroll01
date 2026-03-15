using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypeSignatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "DocumentTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SignaturePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SignerTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypeSignatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    SignerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SignerTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SignatureData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SecurityHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SignatureOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_DocumentTypeSignatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTypeSignatures_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_DocumentTypeId",
                table: "DocumentTemplates",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeSignatures_DocumentTypeId",
                table: "DocumentTypeSignatures",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTemplates_DocumentTypes_DocumentTypeId",
                table: "DocumentTemplates",
                column: "DocumentTypeId",
                principalTable: "DocumentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTemplates_DocumentTypes_DocumentTypeId",
                table: "DocumentTemplates");

            migrationBuilder.DropTable(
                name: "DocumentTypeSignatures");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTemplates_DocumentTypeId",
                table: "DocumentTemplates");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "DocumentTemplates");
        }
    }
}
