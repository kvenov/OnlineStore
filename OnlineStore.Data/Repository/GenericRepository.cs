using OnlineStore.Data.Repository.Interfaces;

namespace OnlineStore.Data.Repository
{
	public class GenericRepository<TEntity, TKey> : BaseRepository<TEntity, TKey>, IRepository<TEntity, TKey>, IAsyncRepository<TEntity, TKey>
		where TEntity : class
	{
		public GenericRepository(ApplicationDbContext context) 
					: base(context)
		{
		}
	}
}
