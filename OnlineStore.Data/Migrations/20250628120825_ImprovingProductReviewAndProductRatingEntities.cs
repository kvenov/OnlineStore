using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImprovingProductReviewAndProductRatingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsRatings_AspNetUsers_UserId",
                table: "ProductsRatings");

            migrationBuilder.DropIndex(
                name: "IX_ProductsRatings_ProductId_UserId",
                table: "ProductsRatings");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "Review",
                table: "ProductsRatings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProductsRatings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ProductsRatings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ProductReviews",
                type: "DATETIME2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Product Review Creation date");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductReviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductReviews",
                type: "DATETIME2",
                nullable: true,
                comment: "Product Review UpdatedAt timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsRatings_ProductId_UserId",
                table: "ProductsRatings",
                columns: new[] { "ProductId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId_ProductId",
                table: "ProductReviews",
                columns: new[] { "UserId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsRatings_AspNetUsers_UserId",
                table: "ProductsRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductsRatings_AspNetUsers_UserId",
                table: "ProductsRatings");

            migrationBuilder.DropIndex(
                name: "IX_ProductsRatings_ProductId_UserId",
                table: "ProductsRatings");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_UserId_ProductId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductReviews");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProductsRatings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ProductsRatings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Review",
                table: "ProductsRatings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Rating review");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsRatings_ProductId_UserId",
                table: "ProductsRatings",
                columns: new[] { "ProductId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductsRatings_AspNetUsers_UserId",
                table: "ProductsRatings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
