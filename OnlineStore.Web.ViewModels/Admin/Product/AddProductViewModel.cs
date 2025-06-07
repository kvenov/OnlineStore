using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using static OnlineStore.Data.Common.Constants.EntityConstants.Product;
using static OnlineStore.Data.Common.Constants.EntityConstants.ProductDetails;


namespace OnlineStore.Web.ViewModels.Admin.Product
{
	public class AddProductViewModel
	{

		// Product information
		[Required]
		[Display(Name = "Product Name")]
		[StringLength(ProductNameMaxLength, MinimumLength = ProductNameMinLength, ErrorMessage = "{0} length must be between {2} and {1}.")]
		public string Name { get; set; } = null!;

		[Required]
		[Display(Name = "Product Description")]
		[StringLength(ProductDescriptionMaxLength, MinimumLength = ProductDescriptionMinLength, ErrorMessage = "{0} length must be between {2} and {1}.")]
		public string Description { get; set; } = null!;

		[Required]
		[Display(Name = "Product ImageUrl")]
		[StringLength(ProductImageUrlMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string ImageUrl { get; set; } = null!;

		[Required]
		[Display(Name = "Product Price")]
		[DataType(DataType.Currency)]
		public decimal Price { get; set; }

		[Display(Name = "Product DiscountPrice")]
		[DataType(DataType.Currency)]
		public decimal? DiscountPrice { get; set; }

		[Required]
		[Display(Name = "Product Is Active")]
		public bool IsActive { get; set; }

		[Required]
		[Display(Name = "Product Stock Quantity")]
		public int StockQuantity { get; set; }

		[Required]
		[Display(Name = "Product AverageRating")]
		public double AverageRating { get; set; }

		[Required]
		[Display(Name = "Product Total Ratings")]
		public int TotalRatings { get; set; }

		// Category and Brand information
		[Required]
		public int CategoryId { get; set; }

		public int? BrandId { get; set; }

		public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
		public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();

		// ProductDetails

		[Required]
		[Display(Name = "ProductDetails Material")]
		[StringLength(MaterialMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string Material { get; set; } = null!;

		[Required]
		[Display(Name = "ProductDetails Color")]
		[StringLength(ColorMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string Color { get; set; } = null!;

		[Required]
		[Display(Name = "ProductDetails Gender")]
		[AllowedValues("Men", "Women", "Unisex")]
		[StringLength(GenderMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string Gender { get; set; } = null!;

		[Required]
		[Display(Name = "ProductDetails SizeGuideUrl")]
		[StringLength(SizeGuideUrlMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string SizeGuideUrl { get; set; } = null!;

		[Required]
		[Display(Name = "ProductDetails CountryOfOrigin")]
		[StringLength(CountryOfOriginMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string CountryOfOrigin { get; set; } = null!;

		[Required]
		[Display(Name = "ProductDetails CareInstructions")]
		[StringLength(CareInstructionsMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string CareInstructions { get; set; } = null!;

		[Required]
		[Display(Name = "ProductDetails Weight")]
		public decimal Weight { get; set; }

		[Required]
		[Display(Name = "ProductDetails Fit")]
		[StringLength(FitMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string Fit { get; set; } = null!;

		[Required]
		[Display(Name = "ProductDetails Style")]
		[StringLength(StyleMaxLength, ErrorMessage = "{0} max length is {1}.")]
		public string Style { get; set; } = null!;
	}
}
