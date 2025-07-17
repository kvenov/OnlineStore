using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class SupportGuestFunctionalityForCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Checkouts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "Checkouts",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Contact email (for guest)");

            migrationBuilder.AddColumn<string>(
                name: "GuestId",
                table: "Checkouts",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Unique guest identifier for checkout");

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                table: "Checkouts",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Full name (for guest)");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "Checkouts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Checkouts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
