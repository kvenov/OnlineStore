using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreationOfTheMainDatabaseStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "AspNetUsers",
                comment: "Users in the store");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                comment: "Registration date of the user");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "NVARCHAR(150)",
                maxLength: 150,
                nullable: true,
                comment: "User Fullname");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "The Address identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The Address Fullname"),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "The Address Street"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The Address City"),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "The Address Country"),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The Address ZipCode"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "The Address PhoneNumber"),
                    IsBillingAddress = table.Column<bool>(type: "bit", nullable: false, comment: "The Address additional identifier"),
                    IsShippingAddress = table.Column<bool>(type: "bit", nullable: false, comment: "The Address additional identifier"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The Addresses of the store");

            migrationBuilder.CreateTable(
                name: "ArticleCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Article category identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Article category name"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Article category description"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleCategories", x => x.Id);
                },
                comment: "Article categories in the store");

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Brand identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Brand name"),
                    LogoUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Brand logo url"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Brand description"),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Brand website url"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Brand short info"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                },
                comment: "Brands in the store");

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Payment method id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Payment method name"),
                    Code = table.Column<int>(type: "int", nullable: true, comment: "Payment method code"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Payment method short admin info"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                },
                comment: "The payment methods in the store");

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Product category identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Product category name"),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Product category description"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                },
                comment: "Product categories in the store");

            migrationBuilder.CreateTable(
                name: "ShoppingCarts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Shopping cart identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Shopping cart creation date"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCarts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Shopping carts in the store");

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "The Wishlist Id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The Wishlist in the store");

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Article identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, comment: "Article title"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Article content"),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Article date of creation"),
                    ImageUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true, comment: "Article image url"),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Article short info"),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_ArticleCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ArticleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                },
                comment: "Articles in the store");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Product identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Product name"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Product description"),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, comment: "Product price"),
                    DiscountPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true, comment: "Product discount price"),
                    StockQuantity = table.Column<int>(type: "int", nullable: false, comment: "Product quantity"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Product short info"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Product creation data"),
                    ImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Product image url"),
                    AverageRating = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false, comment: "Product avarage rating"),
                    TotalRatings = table.Column<int>(type: "int", nullable: false, comment: "Product total ratings"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Products in the store");

            migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Product details identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Material = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Product details material"),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Product details color"),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Product details gender"),
                    SizeGuideUrl = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: false, comment: "Product details size guide url"),
                    CountryOfOrigin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Product details country of origin"),
                    CareInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Product details care instructions"),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", maxLength: 50, nullable: false, comment: "Product details weight"),
                    Fit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, comment: "Product details fit"),
                    Style = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Product details style"),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Details for a certain product");

            migrationBuilder.CreateTable(
                name: "ProductsRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Product rating identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Rating = table.Column<int>(type: "int", maxLength: 5, nullable: false, defaultValue: 0, comment: "Rating value"),
                    Review = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Rating review"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()", comment: "Rating creation date"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductsRatings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The Products ratings in the store");

            migrationBuilder.CreateTable(
                name: "RecentlyViewedProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "The RecentlyViewed Product identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The RecentlyViewed Product creation date")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentlyViewedProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecentlyViewedProducts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecentlyViewedProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The Recently(lastly) viewed products in the store");

            migrationBuilder.CreateTable(
                name: "ShoppingCartsItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Shopping cart item identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Shopping cart item quantity"),
                    Price = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, comment: "Shopping cart item current price"),
                    TotalPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, computedColumnSql: "[Price] * [Quantity]", comment: "Shopping cart item total price"),
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCartsItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCartsItems_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Shopping cart items in the store");

            migrationBuilder.CreateTable(
                name: "WishlistsItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "The WishlistItem Id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "The WishlistItem created date"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "The WishlistItem notes"),
                    Quantity = table.Column<int>(type: "int", nullable: true, comment: "The WishlistItem quantity"),
                    WishlistId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistsItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishlistsItems_Wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The wishlists items in the store");

            migrationBuilder.CreateTable(
                name: "Checkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Checkout id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, comment: "Checkout total price"),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PaymentDetailsId = table.Column<int>(type: "int", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Checkout creation data"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Checkout date of complition"),
                    ShippingAddressId = table.Column<int>(type: "int", nullable: true),
                    BillingAddressId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkouts_Addresses_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkouts_Addresses_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkouts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkouts_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkouts_ShoppingCarts_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCarts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "The checkouts in the store");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Order identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Order creation date"),
                    TotalAmount = table.Column<decimal>(type: "DECIMAL(20,4)", precision: 20, scale: 4, nullable: false, comment: "Order total amount"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "Order status"),
                    ShippingAddressId = table.Column<int>(type: "int", nullable: false),
                    BillingAddressId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CheckoutId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Checkouts_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "Checkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Orders in the store");

            migrationBuilder.CreateTable(
                name: "OrdersItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Order item identifier")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Order item quantity"),
                    UnitPrice = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, comment: "Order item current price"),
                    Subtotal = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, computedColumnSql: "[Quantity] * [UnitPrice]", comment: "Order item price"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdersItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdersItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Order items in the store");

            migrationBuilder.CreateTable(
                name: "PaymentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Payment details id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "Payment details card number"),
                    CardBrand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Payment details card brand"),
                    ExpMonth = table.Column<int>(type: "int", precision: 2, nullable: true, comment: "Payment details card expiry data month"),
                    ExpYear = table.Column<int>(type: "int", precision: 4, nullable: true, comment: "Payment details card expiry data year"),
                    NameOnCard = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Payment details card name"),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Payment details payment date"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "Payment details payment status"),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "The payment details of the store");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategories_Name",
                table: "ArticleCategories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_IsPublished",
                table: "Articles",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_PublishedDate",
                table: "Articles",
                column: "PublishedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Title",
                table: "Articles",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_IsActive",
                table: "Brands",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_BillingAddressId",
                table: "Checkouts",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_PaymentDetailsId",
                table: "Checkouts",
                column: "PaymentDetailsId",
                unique: true,
                filter: "[PaymentDetailsId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_PaymentMethodId",
                table: "Checkouts",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_ShippingAddressId",
                table: "Checkouts",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_ShoppingCartId",
                table: "Checkouts",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_UserId",
                table: "Checkouts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BillingAddressId",
                table: "Orders",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CheckoutId",
                table: "Orders",
                column: "CheckoutId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersItems_OrderId",
                table: "OrdersItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersItems_ProductId",
                table: "OrdersItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_OrderId",
                table: "PaymentDetails",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_PaidAt",
                table: "PaymentDetails",
                column: "PaidAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_Status",
                table: "PaymentDetails",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Name",
                table: "PaymentMethods",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_Name",
                table: "ProductCategories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductDetails",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_AverageRating",
                table: "Products",
                column: "AverageRating");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedAt",
                table: "Products",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price_DiscountPrice",
                table: "Products",
                columns: new[] { "Price", "DiscountPrice" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsRatings_ProductId_UserId",
                table: "ProductsRatings",
                columns: new[] { "ProductId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsRatings_UserId",
                table: "ProductsRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewedProducts_ProductId",
                table: "RecentlyViewedProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewedProducts_UserId",
                table: "RecentlyViewedProducts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_CreatedAt",
                table: "ShoppingCarts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_UserId",
                table: "ShoppingCarts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartsItems_ProductId",
                table: "ShoppingCartsItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartsItems_ShoppingCartId",
                table: "ShoppingCartsItems",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishlistsItems_ProductId",
                table: "WishlistsItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistsItems_WishlistId",
                table: "WishlistsItems",
                column: "WishlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_PaymentDetails_PaymentDetailsId",
                table: "Checkouts",
                column: "PaymentDetailsId",
                principalTable: "PaymentDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Addresses_BillingAddressId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Addresses_ShippingAddressId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_BillingAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_ShippingAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_PaymentDetails_PaymentDetailsId",
                table: "Checkouts");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "OrdersItems");

            migrationBuilder.DropTable(
                name: "ProductDetails");

            migrationBuilder.DropTable(
                name: "ProductsRatings");

            migrationBuilder.DropTable(
                name: "RecentlyViewedProducts");

            migrationBuilder.DropTable(
                name: "ShoppingCartsItems");

            migrationBuilder.DropTable(
                name: "WishlistsItems");

            migrationBuilder.DropTable(
                name: "ArticleCategories");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "PaymentDetails");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Checkouts");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.AlterTable(
                name: "AspNetUsers",
                oldComment: "Users in the store");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
