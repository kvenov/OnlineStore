namespace OnlineStore.Common
{
	public static class ExceptionMessages
	{

		public const string RepositoryInterfaceNotFound = 
				"The interface for the {0} class was not found.The application convention for the repository interface is: I{0}";

		public const string ServiceInterfaceNotFound =
				"The service interface for the {0} class was not found.The application convention for the service interface is: I{0}";
	}
}
