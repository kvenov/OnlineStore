﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Web.Areas.Admin.Controllers
{
	public class DashboardController : Controller
	{

		[Area("Admin")]
		[Authorize(Roles = "Admin")]
		public IActionResult ProductIndex()
		{
			return View();
		}
	}
}
