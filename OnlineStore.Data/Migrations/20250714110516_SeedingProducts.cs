using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AverageRating", "BrandId", "CategoryId", "CreatedAt", "Description", "DiscountPrice", "ImageUrl", "IsActive", "IsDeleted", "Name", "Price", "StockQuantity", "TotalRatings" },
                values: new object[,]
                {
                    { 12, 0.0, 1, 12, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9677), "Men's Road Running Shoes", null, "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/09f3dd58-91e6-4d94-a5ea-1b2c8f7e9ff0/revolution-6-road-running-shoes-8Xf5w3.png", true, false, "Nike Revolution 6", 75.00m, 100, 0 },
                    { 13, 0.0, 2, 12, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9746), "Running shoes made for energy return.", 170.00m, "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/f228f1f8750b4a10929faf6f011c0b99_9366/Ultraboost_Light_Shoes_Black_GY9356_01_standard.jpg", true, false, "Adidas Ultraboost Light", 190.00m, 80, 0 },
                    { 14, 0.0, 3, 3, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9752), "Men's Jeans with a modern slim cut.", 69.99m, "https://lsco.scene7.com/is/image/lsco/levis/clothing/005120939-front-pdp.jpg", true, false, "Levi's 512 Slim Taper Fit", 89.95m, 120, 0 },
                    { 15, 0.0, 1, 6, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9757), "Comfortable fleece joggers for everyday wear.", null, "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/9bdce8c1-1c62-40fd-84d0-d0d658d3e883/sportswear-club-fleece-joggers-QDkzGC.png", true, false, "Nike Club Fleece Joggers", 55.00m, 95, 0 },
                    { 16, 0.0, 1, 2, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9762), "Men's moisture-wicking training tee.", 25.00m, "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/590d39a5-36ad-434c-b073-15376b493f3a/dri-fit-training-t-shirt-Fz4gD3.png", true, false, "Nike Dri-FIT Training T-Shirt", 30.00m, 150, 0 },
                    { 17, 0.0, 2, 2, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9769), "Classic Adidas t-shirt for daily wear.", null, "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/10d12fc1e6e24c73bc03ad2d00cd6e4f_9366/Essentials_Logo_Tee_White_IC9319_01_laydown.jpg", true, false, "Adidas Essentials Tee", 25.00m, 90, 0 },
                    { 18, 0.0, 1, 8, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9773), "Comfortable crew socks for all-day wear.", 15.00m, "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/a4f39e13-4e96-4e8d-bc71-3476bb8ac719/everyday-plus-cushioned-training-crew-socks-JmgKcT.png", true, false, "Nike Everyday Plus Cushioned Socks", 20.00m, 200, 0 },
                    { 19, 0.0, 2, 4, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9778), "Spacious backpack with modern design.", 39.99m, "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/39aa23ceae624ddcb6eeae8601556df5_9366/Classic_Backpack_Black_FM6876_01_standard.jpg", true, false, "Adidas Classic Backpack", 45.00m, 80, 0 },
                    { 20, 0.0, 1, 6, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9782), "Nike Men’s Essential Grey Hoodie", null, "https://static.nike.com/a/images/c_limit,w_592,f_auto/t_product_v1/fb26574f-572f-463a-a63d-df361188ed62/sportswear-club-fleece-mens-graphic-pullover-hoodie-1WcMnq.png", true, false, "Nike Grey Hoodie", 74.99m, 70, 0 },
                    { 21, 0.0, 3, 2, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9788), "Levi's Men's Relaxed Fit Graphic T-Shirt", 24.99m, "https://lsco.scene7.com/is/image/lsco/161430124-front-pdp.jpg", true, false, "Levi's Black Graphic Tee", 29.99m, 120, 0 },
                    { 22, 0.0, 1, 7, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9793), "Nike Men's Navy Training Shorts", 34.99m, "https://static.nike.com/a/images/c_limit,w_592,f_auto/t_product_v1/62231792-14aa-47c4-b5e4-ef59ec69a4ff/dri-fit-mens-training-shorts-kFv4LN.png", true, false, "Nike Navy Shorts", 39.99m, 90, 0 },
                    { 23, 0.0, 2, 6, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9798), "Adidas Essentials Grey Sweatshirt", null, "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/6b0dfdfd66e24667be28af6400c33688_9366/Essentials_Logo_Fleece_Sweatshirt_Grey_IC6779_01_laydown.jpg", true, false, "Adidas Grey Sweatshirt", 54.99m, 85, 0 },
                    { 24, 0.0, 1, 8, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9802), "Nike Everyday Plus Cushioned Ankle Socks", null, "https://static.nike.com/a/images/t_default/e30dcf40-0325-43aa-bfae-3e8a4cf2cb92/everyday-plus-cushioned-training-ankle-socks-M9CcJm.png", true, false, "Nike Blue Ankle Socks", 14.99m, 300, 0 },
                    { 25, 0.0, 3, 5, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9806), "Levi's Original Trucker Denim Jacket", 79.99m, "https://lsco.scene7.com/is/image/lsco/723340178-front-pdp.jpg", true, false, "Levi's Blue Denim Jacket", 89.99m, 45, 0 },
                    { 26, 0.0, 2, 12, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9811), "Adidas Ultraboost Light White Running Shoes", 159.99m, "https://assets.adidas.com/images/w_600,f_auto,q_auto/bdf5b9ecbb54425aab59af6d0113e5c7_9366/Ultraboost_Light_Shoes_White_GW6723_01_standard.jpg", true, false, "Adidas Sporty White Shoes", 189.99m, 60, 0 },
                    { 27, 0.0, 1, 4, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9859), "Nike Brasilia Small Training Backpack", null, "https://static.nike.com/a/images/t_default/e126b819-b305-40a0-aeb4-36e8c9ee5ce2/brasilia-small-training-backpack-18l-LZgMnm.png", true, false, "Nike Red Sports Backpack", 39.99m, 130, 0 },
                    { 28, 0.0, 1, 13, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9864), "Nike Air Force 1 '07 Men's Shoes", null, "https://static.nike.com/a/images/t_default/1bbebd34-3fe2-4dc7-9b98-80f9932f5a75/air-force-1-07-mens-shoes-WrLlWX.png", true, false, "Nike Lifestyle Black Shoes", 114.99m, 95, 0 },
                    { 29, 0.0, 3, 3, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9870), "Levi's 511 Slim Fit Men's Jeans", 49.99m, "https://lsco.scene7.com/is/image/lsco/045114406-front-pdp.jpg", true, false, "Levi's Slim Fit Jeans", 59.99m, 100, 0 },
                    { 30, 0.0, 1, 2, new DateTime(2025, 7, 14, 11, 5, 14, 974, DateTimeKind.Utc).AddTicks(9874), "Nike Dri-FIT Men’s Training T-Shirt", null, "https://static.nike.com/a/images/c_limit,w_592,f_auto/t_product_v1/1797b3f1-9e7a-42cb-8d92-1c9e7cf9a93c/dri-fit-mens-training-t-shirt-Q7zT4k.png", true, false, "Nike Dri-FIT T-Shirt", 34.99m, 115, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);
        }
    }
}
