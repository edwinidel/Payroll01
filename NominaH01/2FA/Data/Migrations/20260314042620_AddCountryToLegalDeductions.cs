using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryToLegalDeductions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "LegalDeductions",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ld
                SET ld.CountryId = c.Id
                FROM LegalDeductions ld
                CROSS JOIN (
                    SELECT TOP 1 Id
                    FROM Countries
                    WHERE Name_es = N'Panamá'
                ) c
                WHERE ld.CountryId IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_LegalDeductions_CountryId",
                table: "LegalDeductions",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalDeductions_Countries_CountryId",
                table: "LegalDeductions",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalDeductions_Countries_CountryId",
                table: "LegalDeductions");

            migrationBuilder.DropIndex(
                name: "IX_LegalDeductions_CountryId",
                table: "LegalDeductions");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "LegalDeductions");
        }
    }
}
