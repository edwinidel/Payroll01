using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Migrations
{
    /// <inheritdoc />
    public partial class AddBankDepositEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankDepositStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StructureName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BankDepositType = table.Column<int>(type: "int", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_BankDepositStructures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankDepositFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankDepositStructureId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldContent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Ignore = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_BankDepositFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankDepositFields_BankDepositStructures_BankDepositStructureId",
                        column: x => x.BankDepositStructureId,
                        principalTable: "BankDepositStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BankDepositFields_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankDepositFields_BankDepositStructureId",
                table: "BankDepositFields",
                column: "BankDepositStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDepositFields_CompanyId",
                table: "BankDepositFields",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankDepositFields");

            migrationBuilder.DropTable(
                name: "BankDepositStructures");
        }
    }
}
