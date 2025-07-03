using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreationOfTheProductPromotionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductsPromotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "The promotion identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PromotionPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, comment: "The promotion price"),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Marketing label or title for the promotion"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2025, 7, 3, 11, 48, 19, 811, DateTimeKind.Utc).AddTicks(8163), comment: "The promotion start date"),
                    ExpDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The promotion date of expire"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Whether the promotion is currently active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsPromotions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "The promotions in the store");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsPromotions_ProductId",
                table: "ProductsPromotions",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsPromotions");
        }
    }
}
