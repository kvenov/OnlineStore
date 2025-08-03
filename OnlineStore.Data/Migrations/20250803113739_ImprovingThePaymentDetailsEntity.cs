using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImprovingThePaymentDetailsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentDetails_OrderId",
                table: "PaymentDetails");

            migrationBuilder.DropIndex(
                name: "IX_PaymentDetails_PaidAt",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "PaymentDetails");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "PaymentDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ExpYear",
                table: "PaymentDetails",
                type: "int",
                precision: 4,
                nullable: false,
                defaultValue: 0,
                comment: "Payment details card expiry data year",
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 4,
                oldNullable: true,
                oldComment: "Payment details card expiry data year");

            migrationBuilder.AlterColumn<int>(
                name: "ExpMonth",
                table: "PaymentDetails",
                type: "int",
                precision: 2,
                nullable: false,
                defaultValue: 0,
                comment: "Payment details card expiry data month",
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 2,
                oldNullable: true,
                oldComment: "Payment details card expiry data month");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_OrderId",
                table: "PaymentDetails",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentDetails_OrderId",
                table: "PaymentDetails");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "PaymentDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExpYear",
                table: "PaymentDetails",
                type: "int",
                precision: 4,
                nullable: true,
                comment: "Payment details card expiry data year",
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 4,
                oldComment: "Payment details card expiry data year");

            migrationBuilder.AlterColumn<int>(
                name: "ExpMonth",
                table: "PaymentDetails",
                type: "int",
                precision: 2,
                nullable: true,
                comment: "Payment details card expiry data month",
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 2,
                oldComment: "Payment details card expiry data month");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "PaymentDetails",
                type: "datetime2",
                nullable: true,
                comment: "Payment details payment date");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_OrderId",
                table: "PaymentDetails",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_PaidAt",
                table: "PaymentDetails",
                column: "PaidAt");
        }
    }
}
