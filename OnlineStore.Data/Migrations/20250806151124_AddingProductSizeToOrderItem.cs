using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingProductSizeToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductSize",
                table: "OrdersItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Order item ProductSize");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSize",
                table: "OrdersItems");

        }
    }
}
