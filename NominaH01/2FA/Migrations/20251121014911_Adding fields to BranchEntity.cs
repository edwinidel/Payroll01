using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Migrations
{
    /// <inheritdoc />
    public partial class AddingfieldstoBranchEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Branches",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CellPhone",
                table: "Branches",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Branches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Branches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dv",
                table: "Branches",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Branches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FixedPhone",
                table: "Branches",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ProfessionalRisk",
                table: "Branches",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Ruc",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Branches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CityId",
                table: "Branches",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CountryId",
                table: "Branches",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_StateId",
                table: "Branches",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Cities_CityId",
                table: "Branches",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_Countries_CountryId",
                table: "Branches",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_States_StateId",
                table: "Branches",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Cities_CityId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_Countries_CountryId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_States_StateId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_CityId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_CountryId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_StateId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CellPhone",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Dv",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "FixedPhone",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "ProfessionalRisk",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Ruc",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Branches");
        }
    }
}
