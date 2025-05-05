namespace OnlineStore.Common
{
	public static class EntityConstants
	{

		public static class ApplicationUser
		{
			/// <summary>
			/// The maximum length of the user's full name.
			/// </summary>
			public const int UserFullNameMaxLength = 150;

			/// <summary>
			/// The maximum length of the user's default address.
			/// </summary>
			public const int UserDefaultAddressMaxLength = 200;
		}

		public static class Article
		{
			/// <summary>
			/// The maximum length of the article's title.
			/// </summary>
			public const int ArticleTitleMaxLength = 30;

			/// <summary>
			/// The maximum length of the article's Url Image.
			/// </summary>
			public const int ArticleImageUrlMaxLenth = 300;

		}

		public static class ArticleCategory
		{
			/// <summary>
			/// The maximum length of the article category's Name.
			/// </summary>
			public const int ArticleCategoryNameMaxLength = 50;

			/// <summary>
			/// The maximum length of the article category's Description.
			/// </summary>
			public const int ArticleCategoryDescriptionMaxLength = 100;
		}

		public static class Brand
		{
			/// <summary>
			/// The maximum length of the brand's Name.
			/// </summary>
			public const int BrandNameMaxLength = 100;

			/// <summary>
			/// The maximum length of the brand's Logo url.
			/// </summary>
			public const int BrandLogoUrlMaxLength = 200;

			/// <summary>
			/// The maximum length of the brand's Description.
			/// </summary>
			public const int BrandDescriptionMaxLength = 500;

			/// <summary>
			/// The maximum length of the brand's Website url.
			/// </summary>
			public const int BrandWebsiteUrlMaxLength = 200;
		}

		public static class Order
		{
			/// <summary>
			/// The maximum length of the order's Shipping address.
			/// </summary>
			public const int OrderShippingAddressMaxLength = 200;
			/// <summary>
			/// The money type for the order's total amount.
			/// </summary>
			public const string OrderTotalAmountType = "DECIMAL(20,4)";
		}

		public static class OrderItem
		{
			/// <summary>
			/// The money type for the order item's unit price.
			/// </summary>
			public const string OrderItemUnitPriceType = "DECIMAL(18,2)";

			/// <summary>
			/// The money type for the order item's subtotal.
			/// </summary>
			public const string OrderItemSubtotalPriceType = "DECIMAL(18,2)";
		}

		public static class Product
		{
			/// <summary>
			/// The maximum length of the product's name.
			/// </summary>
			public const int ProductNameMaxLength = 100;

			/// <summary>
			/// The maximum length of the product's description.
			/// </summary>
			public const int ProductDescriptionMaxLength = 1000;

			/// <summary>
			/// The maximum length of the product's image url.
			/// </summary>
			public const int ProductImageUrlMaxLength = 200;

			/// <summary>
			/// The type of the product's price.
			/// </summary>
			public const string ProductPriceType = "DECIMAL(18,2)";

			/// <summary>
			/// The type of the product's discount price.
			/// </summary>
			public const string ProductDiscountPriceType = "DECIMAL(18,2)";

		}

		public static class ProductCategory
		{
			/// <summary>
			/// The maximum length of the product category's name.
			/// </summary>
			public const int ProductCategoryNameMaxLength = 50;
			/// <summary>
			/// The maximum length of the product category's description.
			/// </summary>
			public const int ProductCategoryDescriptionMaxLength = 100;
		}	

		public static class ShoppingCartItem
		{
			/// <summary>
			/// The type of the shopping cart item's price.
			/// </summary>
			public const string ShoppingCartItemPriceType = "DECIMAL(18,2)";

			/// <summary>
			/// The type of the shopping cart item's total price.
			/// </summary>
			public const string ShoppingCartItemTotalPriceType = "DECIMAL(18,2)";
		}

		public static class PaymentDetails
		{
			/// <summary>
			/// The maximum length of the payment details' name on card.
			/// </summary>
			public const int PaymentDetailsNameOnCardMaxLength = 100;
			/// <summary>
			/// The maximum length of the payment details' card brand.
			/// </summary>
			public const int PaymentDetailsCardBrandMaxLength = 20;
			/// <summary>
			/// The maximum length of the payment details' card number.
			/// </summary>
			public const int PaymentDetailsCardNumberMaxLength = 20;
		}

		public static class PaymentMethod
		{
			/// <summary>
			/// The maximum length of the payment method's name.
			/// </summary>
			public const int PaymentMethodNameMaxLength = 50;
		}

		public static class ProductDetails
		{
			/// <summary>
			/// The maximum length of the product details' material.
			/// </summary>
			public const int MaterialMaxLength = 100;

			/// <summary>
			/// The maximum length of the product details' color.
			/// </summary>
			public const int ColorMaxLength = 50;

			public static readonly string[] AllowedGenders = { "Men", "Women", "Unisex" };

			/// <summary>
			/// The maximum length of the product details' gender.
			/// </summary>
			public const int GenderMaxLength = 20;


			/// <summary>
			/// The maximum length of the product details' size guide URL.
			/// </summary>
			public const int SizeGuideUrlMaxLength = 300;


			/// <summary>
			/// The maximum length of the product details' country of origin.
			/// </summary>
			public const int CountryOfOriginMaxLength = 100;


			/// <summary>
			/// The maximum length of the product details' care instructions.
			/// </summary>
			public const int CareInstructionsMaxLength = 1000;

			/// <summary>
			/// The maximum length of the product details' weight.
			/// </summary>
			public const int MaxWeightKg = 50;

			public static readonly string[] AllowedFits = { "Regular", "Slim", "Loose", "Oversized" };

			/// <summary>
			/// The maximum length of the product details' fit.
			/// </summary>
			public const int FitMaxLength = 30;


			/// <summary>
			/// The maximum length of the product details' style.
			/// </summary>
			public const int StyleMaxLength = 100;
		}

		public static class ProductRating
		{
			/// <summary>
			/// The maximum length of the product rating's review.
			/// </summary>
			public const int ProductRatingReviewMaxLength = 1000;

			/// <summary>
			/// The maximum value for the product rating.
			/// </summary>
			public const int ProductRatingMaxValue = 5;

		}

		public static class Address
		{
			/// <summary>
			/// The maximum length of the address's full name.
			/// </summary>
			public const int AddressFullNameMaxLength = 100;

			/// <summary>
			/// The maximum length of the address's street.
			/// </summary>
			public const int AddressStreetMaxLength = 200;

			/// <summary>
			/// The maximum length of the address's city.
			/// </summary>
			public const int AddressCityMaxLength = 100;

			/// <summary>
			/// The maximum length of the address's country.
			/// </summary>
			public const int AddressCountryMaxLength = 100;

			/// <summary>
			/// The maximum length of the address's zip code.
			/// </summary>
			public const int AddressZipCodeMaxLength = 20;

			/// <summary>
			/// The maximum length of the address's phone number.
			/// </summary>
			public const int AddressPhoneNumberMaxLength = 20;
		}
	}

}
