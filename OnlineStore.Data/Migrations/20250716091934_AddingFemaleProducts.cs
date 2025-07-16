using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingFemaleProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AverageRating", "BrandId", "CategoryId", "CreatedAt", "Description", "DiscountPrice", "ImageUrl", "IsActive", "IsDeleted", "Name", "Price", "StockQuantity", "TotalRatings" },
                values: new object[,]
                {
                    { 31, 0.0, 1, 2, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6192), "A soft cotton t-shirt with classic Nike branding.", null, "https://static.nike.com/a/images/t_PDP_936_v1/f_auto,q_auto:eco/437747c8-ed66-457f-9a03-648dab68ebf0/W+NSW+ESSNTL+TOP+HBR.png", true, false, "Nike Sportswear Women's Tee", 34.99m, 120, 0 },
                    { 32, 0.0, 2, 2, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6196), "Breathable and sustainable tank top ideal for workouts.", 24.99m, "https://media.strefatenisa.com.pl/public/media/66/cf/8a/1721077427/adidas-match-tank-top-w-white-black-1.jpg", true, false, "Adidas Primeblue Women's Tank", 29.99m, 100, 0 },
                    { 33, 0.0, 3, 3, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6200), "Figure-flattering high-rise denim made to shape and contour.", null, "https://lsco.scene7.com/is/image/lsco/188820047-front-pdp", true, false, "Levi's Women's 721 High Rise Skinny Jeans", 69.99m, 90, 0 },
                    { 34, 0.0, 1, 6, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6204), "Soft fleece in a relaxed fit for all-day comfort.", null, "https://www.nike.com.kw/dw/image/v2/BDVB_PRD/on/demandware.static/-/Sites-akeneo-master-catalog/default/dw7b0ab06e/nk/a29/6/0/c/f/d/a2960cfd_ba5b_4f09_83c5_712deadc351e.jpg", true, false, "Nike Phoenix Fleece Women's Sweatshirt", 54.99m, 110, 0 },
                    { 35, 0.0, 1, 12, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6209), "Lightweight and responsive shoes for daily running.", 49.99m, "https://i.sportisimo.com/products/images/1625/1625218/700x700/nike-downshifter-12_3.jpg", true, false, "Nike Downshifter 12 Women's Running Shoes", 59.99m, 100, 0 },
                    { 36, 0.0, 2, 2, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6213), "Stylish peach crop tee made with soft cotton.", null, "https://cms-cdn.thesolesupplier.co.uk/2021/06/adidas-originals-3-stripes-essential-crop-t-shirt-orange-tsw_w672_h672.jpg.webp", true, false, "Adidas Essentials Crop Tee", 27.99m, 85, 0 },
                    { 37, 0.0, 1, 7, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6269), "Moisture-wicking shorts for training or casual wear.", 29.99m, "https://static.nike.com/a/images/c_limit,w_592,f_auto/t_product_v1/a0293382-4be4-4b65-8e3a-481ba7a2cc07/W+NK+ONE+DF+HR+3IN+2N1+SHORT.png", true, false, "Nike Women's Dri-FIT Shorts", 39.99m, 95, 0 },
                    { 38, 0.0, 3, 3, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6275), "Tight-fitting denim for a sleek, modern silhouette.", null, "https://m.media-amazon.com/images/I/41eZ5ojjPYL._SY1000_.jpg", true, false, "Levi's Women's High Rise Super Skinny", 74.99m, 80, 0 },
                    { 39, 0.0, 2, 13, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6279), "Lifestyle sneakers with a minimalist look and comfortable feel.", 54.99m, "https://static.glami.bg/img/800x800bt/424662600-snik-rsi-adidas-grand-court-2-0-shoes-id4483-byal.jpg", true, false, "Adidas Grand Court 2.0 Women's Shoes", 64.99m, 95, 0 },
                    { 40, 0.0, 1, 5, new DateTime(2025, 7, 16, 9, 19, 32, 339, DateTimeKind.Utc).AddTicks(6283), "Iconic windbreaker jacket with water-resistant fabric.", 74.99m, "https://static.nike.com/a/images/t_default/0eea8284-8970-4e9d-8ab7-2bff450c7b7c/W+NSW+NK+LIQ+SHINE+WR+JKT.png", true, false, "Nike Windrunner Women's Jacket", 84.99m, 90, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);
        }
    }
}
