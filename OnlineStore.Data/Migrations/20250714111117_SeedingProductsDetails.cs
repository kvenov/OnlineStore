using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingProductsDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductDetails",
                columns: new[] { "Id", "CareInstructions", "Color", "CountryOfOrigin", "Fit", "Gender", "Material", "ProductId", "Size", "SizeGuideUrl", "Style", "Weight" },
                values: new object[,]
                {
                    { 10, "Machine wash cold, tumble dry low.", "Black", "Vietnam", "Regular", "Men", "Polyester", 12, "L", "https://example.com/size-guide", "Sport", 0.45m },
                    { 11, "Spot clean with mild detergent.", "White", "Indonesia", "Regular", "Unisex", "Textile", 13, "42 EU", "https://example.com/size-guide", "Running", 0.9m },
                    { 12, "Wash inside out with like colors.", "Indigo", "Mexico", "Slim", "Men", "Denim", 14, "32/32", "https://example.com/size-guide", "Casual", 0.75m },
                    { 13, "Machine wash warm.", "Grey", "China", "Regular", "Men", "Fleece", 15, "M", "https://example.com/size-guide", "Streetwear", 0.65m },
                    { 14, "Do not bleach.", "Black", "Thailand", "Athletic", "Men", "Cotton", 16, "L", "https://example.com/size-guide", "Sport", 0.35m },
                    { 15, "Machine wash cold, iron if needed.", "White", "Bangladesh", "Classic", "Unisex", "Cotton", 17, "M", "https://example.com/size-guide", "Casual", 0.4m },
                    { 16, "Wash in cold water.", "Black", "India", "Tight", "Unisex", "Cotton Blend", 18, "M", "https://example.com/size-guide", "Training", 0.15m },
                    { 17, "Hand wash.", "Blue", "China", "Standard", "Unisex", "Polyester", 19, "One Size", "https://example.com/size-guide", "Urban", 0.6m },
                    { 18, "Machine wash cold", "Grey", "Vietnam", "Regular", "Men", "Cotton", 20, "M", "https://www.nike.com/size-fit/men", "Pullover", 0.65m },
                    { 19, "Machine wash", "Black", "Bangladesh", "Relaxed", "Men", "Cotton", 21, "L", "https://www.levi.com/size-guide", "Crewneck", 0.3m },
                    { 20, "Machine wash cold", "Navy", "Thailand", "Regular", "Men", "Polyester", 22, "L", "https://www.nike.com/size-fit/men", "Athletic", 0.25m },
                    { 21, "Machine wash warm", "Grey", "Indonesia", "Loose", "Unisex", "Fleece", 23, "M", "https://www.adidas.com/us/help/size_charts", "Pullover", 0.7m },
                    { 22, "Machine wash cold", "Blue", "China", "Snug", "Unisex", "Cotton/Polyester", 24, "M", "https://www.nike.com/size-fit/unisex", "Low cut", 0.15m },
                    { 23, "Machine wash cold", "Blue", "Mexico", "Regular", "Men", "Denim", 25, "L", "https://www.levi.com/size-guide", "Trucker", 1.2m },
                    { 24, "Wipe with damp cloth", "White", "Vietnam", "True to size", "Men", "Textile", 26, "42", "https://www.adidas.com/us/help/size_charts", "Running", 0.9m },
                    { 25, "Wipe clean", "Red", "China", "Compact", "Unisex", "Polyester", 27, "18L", "https://www.nike.com/size-fit/bags", "Training", 0.8m },
                    { 26, "Wipe with damp cloth", "Black", "Vietnam", "True to size", "Men", "Leather", 28, "43", "https://www.nike.com/size-fit/mens-shoes", "Lifestyle", 1.0m },
                    { 27, "Machine wash cold", "Blue", "Mexico", "Slim", "Men", "Denim", 29, "32", "https://www.levi.com/size-guide", "Jeans", 1.1m },
                    { 28, "Machine wash cold", "Black", "Thailand", "Athletic", "Men", "Polyester", 30, "M", "https://www.nike.com/size-fit/mens-tops", "Training", 0.25m }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 28);
        }
    }
}
