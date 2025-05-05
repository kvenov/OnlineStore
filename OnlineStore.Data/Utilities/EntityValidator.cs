using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Data.Utilities
{
    internal static class EntityValidator
    {

		internal static bool IsValid(object entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var context = new ValidationContext(entity, serviceProvider: null, items: null);
			var validationResults = new List<ValidationResult>();

			return Validator.TryValidateObject(entity, context, validationResults, true);
		}
	}
}
