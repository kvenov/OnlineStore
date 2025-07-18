using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingProductSizeToShoppingCartItemAndRemovingProductSizeFromProductDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "ProductDetails");

            migrationBuilder.AddColumn<string>(
                name: "ProductSize",
                table: "ShoppingCartsItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Shopping cart item size");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSize",
                table: "ShoppingCartsItems");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "ProductDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Product details size");
        }
    }
}
