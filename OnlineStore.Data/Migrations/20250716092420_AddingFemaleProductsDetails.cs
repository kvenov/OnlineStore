using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingFemaleProductsDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductDetails",
                columns: new[] { "Id", "CareInstructions", "Color", "CountryOfOrigin", "Fit", "Gender", "Material", "ProductId", "Size", "SizeGuideUrl", "Style", "Weight" },
                values: new object[,]
                {
                    { 29, "Machine wash cold", "Black", "Vietnam", "Relaxed", "Female", "Cotton", 31, "S", "https://www.nike.com/size-fit", "Casual", 0.25m },
                    { 30, "Wash with similar colors", "White", "Cambodia", "Athletic", "Female", "Polyester", 32, "M", "https://www.adidas.com/size-chart", "Sport", 0.20m },
                    { 31, "Wash inside out", "Blue", "Mexico", "Skinny", "Female", "Denim", 33, "M", "https://www.levi.com/size-guide", "Streetwear", 0.60m },
                    { 32, "Do not bleach", "Pink", "Indonesia", "Loose", "Female", "Cotton Blend", 34, "L", "https://www.nike.com/size-fit", "Lounge", 0.50m },
                    { 33, "Clean with soft brush", "Grey", "China", "Regular", "Female", "Mesh", 35, "38", "https://www.nike.com/size-fit", "Running", 0.70m },
                    { 34, "Tumble dry low", "Peach", "Bangladesh", "Regular", "Female", "Cotton", 36, "S", "https://www.adidas.com/size-chart", "Casual", 0.22m },
                    { 35, "Do not iron", "Black", "Vietnam", "Athletic", "Female", "Polyester", 37, "M", "https://www.nike.com/size-fit", "Training", 0.30m },
                    { 36, "Cold wash only", "Dark Blue", "India", "Super Skinny", "Female", "Denim", 38, "M", "https://www.levi.com/size-guide", "Urban", 0.65m },
                    { 37, "Wipe clean", "White", "Indonesia", "Regular", "Female", "Synthetic Leather", 39, "39", "https://www.adidas.com/size-chart", "Lifestyle", 0.75m },
                    { 38, "Hand wash only", "Black", "Vietnam", "Relaxed", "Female", "Nylon", 40, "M", "https://www.nike.com/size-fit", "Windbreaker", 0.45m }
                });

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 38);
        }
    }
}
