using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.DTOs;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Seeding.Interfaces;
using System.Text.Json;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;
using static OnlineStore.Data.Utilities.EntityValidator;

namespace OnlineStore.Data.Seeding
{
	public class PaymentMethodSeeder : BaseSeeder<PaymentMethodSeeder>, IEntitySeeder
	{
		private readonly ApplicationDbContext _context;

		public PaymentMethodSeeder(ILogger<PaymentMethodSeeder> logger, ApplicationDbContext context) :
					base(logger)
		{
			this._context = context;
		}

		public override string FilePath =>
					Path.Combine(AppContext.BaseDirectory, "Files", "paymentMethods.json");

		public async Task SeedEntityData()
		{
			await this.ImportPaymentMethodsFromJson();
		}

		private async Task ImportPaymentMethodsFromJson()
		{
			string jsonString = await File.ReadAllTextAsync(this.FilePath);

			try
			{

				ImportPaymentMethodDTO[]? paymentMethodsDTOs = JsonSerializer.Deserialize<ImportPaymentMethodDTO[]>(jsonString);

				if (paymentMethodsDTOs != null && paymentMethodsDTOs.Length > 0)
				{
					ICollection<PaymentMethod> validPaymentMethods = new List<PaymentMethod>();
					HashSet<string> existingPaymentMethodsNames = ( await this._context
							.PaymentMethods
							.AsNoTracking()
							.Select(pm => pm.Name)
							.ToListAsync()).ToHashSet();

					this.Logger.LogInformation($"Found {paymentMethodsDTOs.Length} PaymentMethods DTO's to process.");

					foreach (var paymentMethodDto in paymentMethodsDTOs)
					{
						if (!IsValid(paymentMethodDto))
						{
							string warningMessage = this.BuildEntityValidatorWarningMessage(nameof(PaymentMethod));
							this.Logger.LogWarning(warningMessage);
							continue;
						}

						if (existingPaymentMethodsNames.Contains(paymentMethodDto.Name))
						{
							this.Logger.LogWarning(EntityInstanceAlreadyExists);
							continue;
						}

						if (validPaymentMethods.Any(vpm => vpm.Name == paymentMethodDto.Name))
						{
							string warningMessage = this.BuildEntityValidatorWarningMessage(nameof(PaymentMethod));
							this.Logger.LogWarning(warningMessage);
							continue;
						}

						bool isCodeValid = Enum.TryParse<PaymentMethodCode>(paymentMethodDto.Code, out var paymentMethodCode);

						if (!isCodeValid)
						{
							this.Logger.LogWarning(EntityDataParseError);
							continue;
						}

						PaymentMethod paymentMethod = new PaymentMethod()
						{
							Name = paymentMethodDto.Name,
							Code = paymentMethodCode,
							IsActive = paymentMethodDto.IsActive,
							IsDeleted = paymentMethodDto.IsDeleted
						};

						validPaymentMethods.Add(paymentMethod);
					}

					if (validPaymentMethods.Count > 0)
					{
						await this._context.PaymentMethods.AddRangeAsync(validPaymentMethods);
						await this._context.SaveChangesAsync();
						this.Logger.LogInformation($"{validPaymentMethods.Count} payment methods imported.");
					}
					else
					{
						this.Logger.LogWarning(NoNewEntityDataToAdd);
					}
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex.Message);
			}
		}
	}
}
