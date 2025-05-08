using OnlineStore.Data.Seeding.Interfaces;
using Microsoft.Extensions.Logging;
using static OnlineStore.Common.OutputMessages.ErrorMessages;

namespace OnlineStore.Data.Seeding
{
    public abstract class BaseSeeder<T> : IBaseSeeder<T>
	{
		private readonly ILogger<T> _logger;

		protected BaseSeeder(ILogger<T> logger)
		{
			this._logger = logger;
			
			this.FilePath = string.Empty;
		}

		public ILogger<T> Logger => 
				this._logger;

		public virtual string FilePath { get; }

		public string BuildEntityValidatorWarningMessage(string entity)
		{
			string logMessage = string
									.Format(EntityImportError, nameof(entity));

			return logMessage;
		}
	}
}
