using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Data;
using OnlineStore.Data.Interceptors;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using System.Reflection;

using static OnlineStore.Common.ExceptionMessages;

namespace OnlineStore.Web.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{

		private static readonly string ServiceInterfacePrefix = "I";
		private static readonly string ServiceTypeSuffix = "Service";

		private static readonly string RepositoryInterfacePrefix = "I";
		private static readonly string RepositoryTypeSuffix = "Repository";

		
		public static IServiceCollection AddScopedServices(this IServiceCollection serviceCollection, Assembly serviceAssembly)
		{
			Type[] servicesClasses = serviceAssembly
					.GetTypes()
					.Where(t => t.Name.EndsWith(ServiceTypeSuffix) &&
								!t.IsInterface)
					.ToArray();

			foreach (Type serviceClass in servicesClasses)
			{
				Type? serviceClassInterface = serviceClass
							.GetInterfaces()
							.FirstOrDefault(i => i.Name.StartsWith(ServiceInterfacePrefix) &&
												 i.Name.EndsWith(ServiceTypeSuffix) &&
												 i.IsInterface);

				if (serviceClassInterface != null)
				{
					serviceCollection.AddScoped(serviceClassInterface, serviceClass);
				}
				else
				{
					throw new ArgumentException(string.Format(ServiceInterfaceNotFound, serviceClass.Name));
				}
			}

			return serviceCollection;
		}

		public static IServiceCollection AddScopedRepositories(this IServiceCollection serviceCollection, Assembly repositotyAssembly)
		{

			Type[] repositoriesClasses = repositotyAssembly
					.GetTypes()
					.Where(t => !t.IsInterface && !t.IsAbstract &&
								 t.Name.EndsWith(RepositoryTypeSuffix))
					.ToArray();

			foreach (var repositoryClass in repositoriesClasses)
			{
				Type? repositoryInterface = repositoryClass
							.GetInterfaces()
							.FirstOrDefault(i => i.Name.StartsWith(RepositoryInterfacePrefix) &&
												 i.Name.EndsWith(RepositoryTypeSuffix) &&
												 i.Name.Contains(repositoryClass.Name));

				if (repositoryInterface != null)
				{
					serviceCollection.AddScoped(repositoryInterface, repositoryClass);
				}
				else
				{
					throw new ArgumentException(string.Format(RepositoryInterfaceNotFound, repositoryClass.Name));
				}

			}

			return serviceCollection;
		}
		
		public static IServiceCollection AddScopedGenericRepositories(this IServiceCollection serviceCollection, Type repositoryType)
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
		
		public static IServiceCollection AddApplicationDbContext(this IServiceCollection serviceCollection, string connectionString)
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

		public static IServiceCollection AddCustomIdentity(this IServiceCollection serviceCollection)
		{
			serviceCollection
				.AddIdentity<ApplicationUser, IdentityRole>(options =>
				{
					options.SignIn.RequireConfirmedAccount = false;
					options.Password.RequireDigit = true;
					options.Password.RequireLowercase = true;
					options.Password.RequireUppercase = true;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequiredLength = 6;
				})
				.AddSignInManager<SignInManager<ApplicationUser>>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			return serviceCollection;
		}
	}
}
