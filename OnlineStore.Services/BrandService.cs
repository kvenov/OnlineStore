﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Services.Core.Interfaces;

namespace OnlineStore.Services.Core
{
	public class BrandService : IBrandService
	{
		private readonly IRepository<Brand, int> _repository;

		public BrandService(IRepository<Brand, int> repository)
		{
			this._repository = repository;
		}

		public async Task<IEnumerable<SelectListItem>> GetAllBrandsIdsAndNamesAsync()
		{
			IEnumerable<SelectListItem> brands = await this._repository
				.GetAllAttached()
				.AsNoTracking()
				.Select(c => new SelectListItem
				{
					Value = c.Id.ToString(),
					Text = c.Name
				})
				.ToListAsync();

			return brands;
		}
	}
}
