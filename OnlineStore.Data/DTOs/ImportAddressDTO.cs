using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using static OnlineStore.Data.Common.Constants.EntityConstants.Address;

namespace OnlineStore.Data.DTOs
{
	[XmlType("Address")]
	public class ImportAddressDTO
	{

		[Required]
		[XmlElement(nameof(Street))]
		[MaxLength(AddressStreetMaxLength)]
		public string Street { get; set; } = null!;

		[Required]
		[XmlElement(nameof(City))]
		[MaxLength(AddressCityMaxLength)]
		public string City { get; set; } = null!;

		[Required]
		[XmlElement(nameof(Country))]
		[MaxLength(AddressCountryMaxLength)]
		public string Country { get; set; } = null!;

		[Required]
		[XmlElement(nameof(ZipCode))]
		[MaxLength(AddressZipCodeMaxLength)]
		public string ZipCode { get; set; } = null!;

		[Required]
		[XmlElement(nameof(PhoneNumber))]
		[MaxLength(AddressPhoneNumberMaxLength)]
		public string PhoneNumber { get; set; } = null!;

		[Required]
		[XmlElement(nameof(IsBillingAddress))]
		public string IsBillingAddress { get; set; } = null!;

		[Required]
		[XmlElement(nameof(IsShippingAddress))]
		public string IsShippingAddress { get; set; } = null!;

		[Required]
		[XmlElement(nameof(UserId))]
		public string UserId { get; set; } = null!;

		[Required]
		[XmlElement(nameof(IsDeleted))]
		public string IsDeleted { get; set; } = null!;
	}
}
