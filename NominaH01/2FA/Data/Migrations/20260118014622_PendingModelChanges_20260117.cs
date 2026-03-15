using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2FA.Data.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges_20260117 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordLastChanged",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequirePasswordChangeOnExpiry",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordLastChanged",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RequirePasswordChangeOnExpiry",
                table: "AspNetUsers");
        }
    }
}
