using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Web.Validation
{
	public class CardExpirationAttribute : ValidationAttribute, IClientModelValidator
	{
		private readonly string _monthProperty;

		public CardExpirationAttribute(string monthProperty)
		{
			_monthProperty = monthProperty;
			ErrorMessage = "Card has expired.";
		}

		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var expYear = (int?)value;
			var monthProperty = validationContext.ObjectType.GetProperty(_monthProperty);

			if (monthProperty == null || expYear == null)
				return ValidationResult.Success;

			var expMonth = (int?)monthProperty.GetValue(validationContext.ObjectInstance);

			if (expMonth == null)
				return ValidationResult.Success;

			if (expYear is >= 0 and < 100)
				expYear += 2000;

			var now = DateTime.UtcNow;
			var current = new DateTime(now.Year, now.Month, 1);
			var expiration = new DateTime(expYear.Value, expMonth.Value, 1);

			return expiration >= current
				? ValidationResult.Success
				: new ValidationResult(ErrorMessage);
		}

		public void AddValidation(ClientModelValidationContext context)
		{
			MergeAttribute(context.Attributes, "data-val", "true");
			MergeAttribute(context.Attributes, "data-val-cardexp", ErrorMessage ?? "Card expiration date is invalid.");

			// Use the parent field name (e.g., Payment.CreditCardDetails.ExpYear)
			var fullFieldName = context.Attributes["name"]; // e.g., Payment.CreditCardDetails.ExpYear

			if (!string.IsNullOrEmpty(fullFieldName))
			{
				var prefix = fullFieldName.Substring(0, fullFieldName.LastIndexOf('.') + 1);
				var monthFieldFullName = prefix + _monthProperty;

				MergeAttribute(context.Attributes, "data-val-cardexp-month", monthFieldFullName);
			}
		}


		private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
		{
			if (attributes.ContainsKey(key)) return false;
			attributes.Add(key, value);
			return true;
		}
	}
}
