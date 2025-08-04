using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingShippingSupportInCheckoutAndOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryEnd",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Estimated delivery end date");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryStart",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Estimated delivery start date");

            migrationBuilder.AddColumn<string>(
                name: "ShippingOption",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Shipping option for the order");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingPrice",
                table: "Orders",
                type: "DECIMAL(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Shipping price for the order");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryEnd",
                table: "Checkouts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Estimated delivery end date");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryStart",
                table: "Checkouts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Estimated delivery start date");

            migrationBuilder.AddColumn<string>(
                name: "ShippingOption",
                table: "Checkouts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                comment: "Shipping option for the checkout");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingPrice",
                table: "Checkouts",
                type: "DECIMAL(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Shipping price for the checkout");

			migrationBuilder.RenameColumn(
				name: "TotalPrice",
				table: "Checkouts",
				newName: "SubTotal");

			migrationBuilder.AddColumn<decimal>(
				name: "TotalPrice",
				table: "Checkouts",
				type: "DECIMAL(18,2)",
				nullable: false,
				computedColumnSql: "[SubTotal] + [ShippingPrice]",
				stored: true,
				comment: "Total price for the checkout");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryEnd",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryStart",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingOption",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryEnd",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryStart",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "ShippingOption",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "ShippingPrice",
                table: "Checkouts");

			migrationBuilder.DropColumn(name: "TotalPrice", table: "Checkouts");

			migrationBuilder.RenameColumn(
				name: "SubTotal",
				table: "Checkouts",
				newName: "TotalPrice");
		}
    }
}
