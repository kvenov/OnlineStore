using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Data;
using OnlineStore.Data.Interceptors;
using OnlineStore.Data.Repository.Interfaces;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace OnlineStore.Web.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{

		private static readonly string ServiceInterfacePrefix = "I";
		private static readonly string ServiceTypeSuffix = "Service";

		private static readonly string RepositoryInterfacePrefix = "I";
		private static readonly string RepositoryTypeSuffix = "Repository";


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

		public static IServiceCollection AddUserDefinedScopedRepositories(this IServiceCollection serviceCollection, Assembly repositotyAssembly)
		{

			Type[] repositoriesClasses = repositotyAssembly
					.GetTypes()
					.Where(t => !t.IsInterface && !t.IsAbstract &&
								 t.Name.EndsWith(RepositoryTypeSuffix))
					.ToArray();

			foreach (var repositoryClass in repositoriesClasses)
			{
				string repositotyClassName = repositoryClass.Name;
				Type[] repositoryInterfaces = repositoryClass
							.GetInterfaces()
							.Where(i => i.Name.ToLower().Contains(repositotyClassName.ToLower()))
							.ToArray();

				if (repositoryInterfaces.Length == 1 && 
					repositoryInterfaces.First().Name.StartsWith(RepositoryInterfacePrefix) &&
					repositoryInterfaces.First().Name.EndsWith(RepositoryTypeSuffix) &&
					repositoryInterfaces.First().IsInterface)
				{
					Type repositoryInterface = repositoryInterfaces.First();

					serviceCollection.AddScoped(repositoryInterface, repositoryClass);
				}

			}

			return serviceCollection;
		}

		public static IServiceCollection AddUserDefinedScopedGenericRepositories(this IServiceCollection serviceCollection, Type repositoryType)
		{
			Type[] interfaceTypes = repositoryType.GetInterfaces()
				.Where(i => i.IsGenericType &&
							(i.GetGenericTypeDefinition() == typeof(IRepository<,>) ||
							 i.GetGenericTypeDefinition() == typeof(IAsyncRepository<,>)))
				.ToArray();

			foreach (var interfaceType in interfaceTypes)
			{
				var openInterfaceType = interfaceType.GetGenericTypeDefinition();
				serviceCollection.AddScoped(openInterfaceType, repositoryType);
			}


			return serviceCollection;
		}

		public static IServiceCollection AddUserDefinedApplicationDbContext(this IServiceCollection serviceCollection, string connectionString)
		{
			serviceCollection.AddDbContext<ApplicationDbContext>((sp, options) =>
			{
				var interceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
				options
					.UseLazyLoadingProxies()
					.UseSqlServer(connectionString)
					.AddInterceptors(interceptor);
			});

			return serviceCollection;
		}
	}
}
