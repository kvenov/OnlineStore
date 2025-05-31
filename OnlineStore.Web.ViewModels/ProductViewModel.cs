using System.ComponentModel.DataAnnotations;
using static OnlineStore.Data.Common.Constants.EntityConstants.Product;

namespace OnlineStore.Web.ViewModels
{
	public class ProductViewModel
	{

		public int Id { get; set; }

		[Required]
		[Display(Name = "Product Name")]
		[StringLength(ProductNameMaxLength, MinimumLength = ProductNameMinLength, ErrorMessage = "{0} must be between {2} and {1} characters long.")]
		public string Name { get; set; } = null!;

		[Required]
		[Display(Name = "Description")]
		[StringLength(ProductDescriptionMaxLength)]
		public string Description { get; set; } = null!;

		[Required]
		[Display(Name = "Price")]
		[DataType(DataType.Currency)]
		public decimal Price { get; set; }

		[Required]
		[Display(Name = "Image Url")]
		[StringLength(ProductImageUrlMaxLength)]
		public string ImageUrl { get; set; } = null!;
	}
}
