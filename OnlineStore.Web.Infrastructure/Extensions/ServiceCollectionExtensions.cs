using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OnlineStore.Web.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{

		private static readonly string ServiceInterfacePrefix = "I";
		private static readonly string ServiceTypeSuffix = "Service";


		public static IServiceCollection AddUserDefinedScopedServices(this IServiceCollection serviceCollection, Assembly serviceAssembly)
		{
			Type[] servicesClasses = serviceAssembly
					.GetTypes()
					.Where(t => t.Name.EndsWith(ServiceTypeSuffix) &&
								!t.IsInterface)
					.ToArray();

			foreach (Type serviceClass in servicesClasses)
			{
				Type[] serviceClassInterfaces = serviceClass
							.GetInterfaces();

				if (serviceClassInterfaces.Length == 1 &&
					serviceClassInterfaces.First().Name.StartsWith(ServiceInterfacePrefix) &&
					serviceClassInterfaces.First().Name.EndsWith(ServiceTypeSuffix) &&
					serviceClassInterfaces.First().IsInterface)
				{
					Type serviceClassInterface = serviceClassInterfaces.First();

					serviceCollection.AddScoped(serviceClassInterface, serviceClass);
				}
			}

			return serviceCollection;
		}
	}
}
