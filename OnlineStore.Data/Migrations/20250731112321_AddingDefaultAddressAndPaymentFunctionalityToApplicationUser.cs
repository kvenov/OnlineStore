using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingDefaultAddressAndPaymentFunctionalityToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "DefaultBillingAddressId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultPaymentDetailsId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultPaymentMethodId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultShippingAddressId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DefaultBillingAddressId",
                table: "AspNetUsers",
                column: "DefaultBillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DefaultPaymentDetailsId",
                table: "AspNetUsers",
                column: "DefaultPaymentDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DefaultPaymentMethodId",
                table: "AspNetUsers",
                column: "DefaultPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DefaultShippingAddressId",
                table: "AspNetUsers",
                column: "DefaultShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Addresses_DefaultBillingAddressId",
                table: "AspNetUsers",
                column: "DefaultBillingAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Addresses_DefaultShippingAddressId",
                table: "AspNetUsers",
                column: "DefaultShippingAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PaymentDetails_DefaultPaymentDetailsId",
                table: "AspNetUsers",
                column: "DefaultPaymentDetailsId",
                principalTable: "PaymentDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PaymentMethods_DefaultPaymentMethodId",
                table: "AspNetUsers",
                column: "DefaultPaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Addresses_DefaultBillingAddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Addresses_DefaultShippingAddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PaymentDetails_DefaultPaymentDetailsId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PaymentMethods_DefaultPaymentMethodId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DefaultBillingAddressId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DefaultPaymentDetailsId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DefaultPaymentMethodId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DefaultShippingAddressId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DefaultBillingAddressId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DefaultPaymentDetailsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DefaultPaymentMethodId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DefaultShippingAddressId",
                table: "AspNetUsers");
        }
    }
}
