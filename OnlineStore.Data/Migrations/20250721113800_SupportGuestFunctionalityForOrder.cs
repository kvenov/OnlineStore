using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class SupportGuestFunctionalityForOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Contact email (for guest)");

            migrationBuilder.AddColumn<string>(
                name: "GuestId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                comment: "Unique guest identifier for order");

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Full name (for guest)");

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Order number");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GuestId",
                table: "Orders",
                column: "GuestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_GuestId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Orders");
        }
    }
}
