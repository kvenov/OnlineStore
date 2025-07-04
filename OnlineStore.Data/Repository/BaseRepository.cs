using OnlineStore.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace OnlineStore.Data.Repository
{
	public abstract class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>, IAsyncRepository<TEntity, TKey>
		where TEntity : class
	{
		protected readonly ApplicationDbContext DbContext;
		protected readonly DbSet<TEntity> DbSet;

		protected BaseRepository(ApplicationDbContext dbContext)
		{
			this.DbContext = dbContext;
			this.DbSet = this.DbContext.Set<TEntity>();
		}

		public void Add(TEntity item)
		{
			DbSet.Add(item);
			DbContext.SaveChanges();
		}

		public async Task AddAsync(TEntity item)
		{
			await DbSet.AddAsync(item);
			await DbContext.SaveChangesAsync();
		}

		public void AddRange(IEnumerable<TEntity> items)
		{
			DbSet.AddRange(items);
			DbContext.SaveChanges();
		}

		public async Task AddRangeAsync(IEnumerable<TEntity> items)
		{
			await DbSet.AddRangeAsync(items);
			await DbContext.SaveChangesAsync();
		}

		public int Count()
		{
			return DbSet
					.Count();
		}

		public async Task<int> CountAsync()
		{
			return await DbSet
					.CountAsync();
		}

		public bool Delete(TEntity entity)
		{
			this.ExecuteSoftDelete(entity);
			return this.Update(entity);

		}

		public Task<bool> DeleteAsync(TEntity entity)
		{
			this.ExecuteSoftDelete(entity);
			return this.UpdateAsync(entity);
		}

		public TEntity? FirstOrDefault(Func<TEntity, bool> predicate)
		{
			return DbSet.FirstOrDefault(predicate);
		}

		public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
		{
			TEntity? entity = await DbSet
					.FirstOrDefaultAsync(predicate);

			return entity;
		}

		public IEnumerable<TEntity> GetAll()
		{
			return DbSet
					.ToArray();
		}

		public async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			TEntity[] entities = await DbSet
					.ToArrayAsync();

			return entities;
		}

		public IQueryable<TEntity> GetAllAttached()
		{
			return DbSet
					.AsQueryable();
		}

		public TEntity? GetById(TKey id)
		{
			return DbSet
					.Find(id);
		}

		public async Task<TEntity?> GetByIdAsync(TKey id)
		{
			TEntity? entity = await DbSet
					.FindAsync(id);

			return entity;
		}

		public bool HardDelete(TEntity entity)
		{
			DbSet.Remove(entity);
			int affectedRows = DbContext.SaveChanges();
			return affectedRows > 0;
		}

		public async Task<bool> HardDeleteAsync(TEntity entity)
		{
			DbSet.Remove(entity);
			int affectedRows = await DbContext.SaveChangesAsync();
			return affectedRows > 0;
		}

		public void SaveChanges()
		{
			DbContext.SaveChanges();
		}

		public async Task SaveChangesAsync()
		{
			await DbContext.SaveChangesAsync();
		}

		public TEntity? SingleOrDefault(Func<TEntity, bool> predicate)
		{
			return DbSet.SingleOrDefault(predicate);
		}

		public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
		{
			TEntity? entity = await DbSet
					.SingleOrDefaultAsync(predicate);

			return entity;
		}

		public bool Update(TEntity item)
		{
			try
			{
				DbSet.Attach(item);
				DbSet.Entry(item).State = EntityState.Modified;
				DbContext.SaveChanges();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> UpdateAsync(TEntity item)
		{
			try
			{
				DbSet.Attach(item);
				DbSet.Entry(item).State = EntityState.Modified;
				await DbContext.SaveChangesAsync();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void ExecuteSoftDelete(TEntity entity)
		{
			PropertyInfo? propertyInfo = GetIsDeletedProperty();

			if (propertyInfo == null)
			{
				throw new InvalidOperationException();
			}

			propertyInfo.SetValue(entity, true);
		}

		private PropertyInfo? GetIsDeletedProperty()
		{
			return typeof(TEntity)
					.GetProperties()
					.FirstOrDefault(pi => pi.PropertyType == typeof(bool) &&
										  pi.Name == "IsDeleted");
		}
	}
}
