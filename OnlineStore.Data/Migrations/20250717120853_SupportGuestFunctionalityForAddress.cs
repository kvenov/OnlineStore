using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class SupportGuestFunctionalityForAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Addresses");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "GuestId",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: true,
                comment: "Unique guest identifier for address");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_GuestId",
                table: "Addresses",
                column: "GuestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_GuestId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Addresses");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "The Address Fullname");

        }
    }
}
