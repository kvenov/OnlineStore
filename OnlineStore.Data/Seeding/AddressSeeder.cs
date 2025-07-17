using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.DTOs;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using OnlineStore.Data.Utilities.Interfaces;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;
using static OnlineStore.Data.Utilities.EntityValidator;

namespace OnlineStore.Data.Seeding
{
	public class AddressSeeder : BaseSeeder<AddressSeeder>, IEntitySeeder, IXmlSeeder
	{
		private readonly ApplicationDbContext _context;
		private readonly IXmlHelper _xmlHelper;

		public AddressSeeder(ILogger<AddressSeeder> logger, ApplicationDbContext context, IXmlHelper xmlHelper) : 
					base(logger)
		{
			this._context = context ?? throw new ArgumentNullException(nameof(context));
			this._xmlHelper = xmlHelper ?? throw new ArgumentNullException(nameof(xmlHelper));
		}

		public override string FilePath => 
							Path.Combine(AppContext.BaseDirectory, "Files", "addresses.xml");

		public string RootName => "Addresses";

		public IXmlHelper XmlHelper => this._xmlHelper;

		public async Task SeedEntityData()
		{
			await this.ImportAddressesFromXml();
		}

		private async Task ImportAddressesFromXml()
		{
			string stringXml = await File.ReadAllTextAsync(this.FilePath);

			try
			{

				ImportAddressDTO[]? addressDTOs = this.XmlHelper.Deserialize<ImportAddressDTO[]>(stringXml, this.RootName);

				if (addressDTOs != null && addressDTOs.Length > 0)
				{
					ICollection<Address> validAddresses = new List<Address>();

					HashSet<string> validUsersIds = (await this._context
							.Users
							.AsNoTracking()
							.Select(u => u.Id)
							.ToListAsync()).ToHashSet();

					var existingAddresses = (await this._context
							.Addresses
							.AsNoTracking()
							.Select(a => new
							{
								a.Street,
								a.City,
								a.Country,
								a.ZipCode,
								a.PhoneNumber,
								a.UserId
							})
							.ToListAsync()).ToHashSet();

					this.Logger.LogInformation($"Found {addressDTOs.Length} Address DTO's to process.");

					foreach (var addressDto in addressDTOs)
					{

						if (!IsValid(addressDto))
						{
							string warningMessage = this.BuildEntityValidatorWarningMessage(nameof(Address));
							this.Logger.LogWarning(warningMessage);
							continue;
						}

						bool isBillingAddressValid = bool.TryParse(addressDto.IsBillingAddress, out bool isBillingAddress);

						bool isShippingAddressValid = bool.TryParse(addressDto.IsShippingAddress, out bool isShippingAddress);

						bool isDeletedValid = bool.TryParse(addressDto.IsDeleted, out bool isDeleted);

						if (!isBillingAddressValid || !isShippingAddressValid || !isDeletedValid)
						{
							this.Logger.LogWarning(EntityDataParseError);
							continue;
						}

						if (!validUsersIds.Contains(addressDto.UserId))
						{
							this.Logger.LogWarning(ReferencedEntityMissing);
							continue;
						}

						bool isDuplicateInDb = existingAddresses.Any(a =>
									a.UserId == addressDto.UserId &&
									a.Street.Equals(addressDto.Street, StringComparison.OrdinalIgnoreCase) &&
									a.City.Equals(addressDto.City, StringComparison.OrdinalIgnoreCase) &&
									a.Country.Equals(addressDto.Country, StringComparison.OrdinalIgnoreCase) &&
									a.ZipCode.Equals(addressDto.ZipCode.Trim(), StringComparison.OrdinalIgnoreCase) &&
									a.PhoneNumber.Equals(addressDto.PhoneNumber.Trim(), StringComparison.OrdinalIgnoreCase));

						bool isDuplicateInValidAddresses = validAddresses.Any(a =>
									a.UserId == addressDto.UserId &&
									a.Street.Equals(addressDto.Street, StringComparison.OrdinalIgnoreCase) &&
									a.City.Equals(addressDto.City, StringComparison.OrdinalIgnoreCase) &&
									a.Country.Equals(addressDto.Country, StringComparison.OrdinalIgnoreCase) &&
									a.ZipCode.Equals(addressDto.ZipCode.Trim(), StringComparison.OrdinalIgnoreCase) &&
									a.PhoneNumber.Equals(addressDto.PhoneNumber.Trim(), StringComparison.OrdinalIgnoreCase));

						if (isDuplicateInDb || isDuplicateInValidAddresses)
						{
							this.Logger.LogWarning(EntityInstanceAlreadyExists);
							continue;
						}

						if (!isBillingAddress && !isShippingAddress)
						{
							this.Logger.LogWarning(EntityInstanceNotValid);
							continue;
						}

						Address address = new Address()
						{
							Street = addressDto.Street,
							City = addressDto.City,
							Country = addressDto.Country,
							ZipCode = addressDto.ZipCode,
							PhoneNumber = addressDto.PhoneNumber,
							IsBillingAddress = isBillingAddress,
							IsShippingAddress = isShippingAddress,
							UserId = addressDto.UserId,
							IsDeleted = isDeleted
						};

						validAddresses.Add(address);
					}

					if (validAddresses.Count > 0)
					{
						await this._context.AddRangeAsync(validAddresses);
						await this._context.SaveChangesAsync();
						this.Logger.LogInformation($"Successfully imported {validAddresses.Count} Address entities.");
					}
					else
					{
						this.Logger.LogWarning(NoNewEntityDataToAdd);
					}
				}
				else
				{
					this.Logger.LogWarning($"No Address DTO's found in the XML file at {this.FilePath}.");
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex.Message);
			}
		}
	}
}
