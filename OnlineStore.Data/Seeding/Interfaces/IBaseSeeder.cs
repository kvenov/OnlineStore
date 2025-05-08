using Microsoft.Extensions.Logging;

namespace OnlineStore.Data.Seeding.Interfaces
{
    public interface IBaseSeeder<T>
    {
		string FilePath { get; }

		ILogger<T> Logger { get; }

		string BuildEntityValidatorWarningMessage(string entity);

	}
}
