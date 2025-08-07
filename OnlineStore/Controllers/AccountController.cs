using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.ViewModels.Account;

using static OnlineStore.Common.ApplicationConstants;

namespace OnlineStore.Web.Controllers
{
	public class AccountController : BaseController
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;
		private readonly ILogger<AccountController> _logger;
		private readonly IShoppingCartService _shoppingCartService;
		private readonly IWishlistService _wishlistService;

		public AccountController(SignInManager<ApplicationUser> signInManager, 
							     UserManager<ApplicationUser> userManager,
								 IUserStore<ApplicationUser> userStore,
								 ILogger<AccountController> logger,
								 IShoppingCartService shoppingCartService,
								 IWishlistService wishlistService)
		{
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._userStore = userStore;
			this._emailStore = GetEmailStore();
			this._logger = logger;
			this._shoppingCartService = shoppingCartService;
			this._wishlistService = wishlistService;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			var model = new LoginViewModel
			{
				ReturnUrl = returnUrl
			};

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			model.ReturnUrl ??= Url.Content("~/");

			if (!ModelState.IsValid)
				return View(model);

			var user = await _userManager.FindByEmailAsync(model.LoginInput)
					   ?? await _userManager.FindByNameAsync(model.LoginInput);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(model);
			}

			var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

			if (result.Succeeded)
			{
				return RedirectToLocal(model.ReturnUrl);
			}
			else if (result.RequiresTwoFactor)
			{
				return RedirectToAction("LoginWith2fa", new { model.ReturnUrl, model.RememberMe });
			}
			else if (result.IsLockedOut)
			{
				return RedirectToAction("Lockout");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(model);
			}
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			var model = new RegisterViewModel()
			{
				ReturnUrl = returnUrl,
			};

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			model.ReturnUrl ??= Url.Content("~/");

			if (ModelState.IsValid)
			{
				ApplicationUser user = CreateUser();

				await _userStore.SetUserNameAsync(user, model.Username, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					if (user.ShoppingCart == null)
					{
						ShoppingCart? userShoppingCart = await this._shoppingCartService
												.AddNewShoppingCartAsync(user);

						if (userShoppingCart == null)
							throw new InvalidOperationException("The shopping cart for the User cannot be created!");

						user.ShoppingCart = userShoppingCart;
					}

					if (user.Wishlist == null)
					{
						Wishlist? userWishlist = await this._wishlistService
										.AddNewWishlistAsync(user);

						if (userWishlist == null)
							throw new InvalidOperationException("The wishlist for the User cannot be created!");

						user.Wishlist = userWishlist;
					}

					string fullName = $"{model.FirstName} {model.LastName}";
					user.FullName = fullName;

					await this._userManager.UpdateAsync(user);

					await this._userManager.AddToRoleAsync(user, UserRoleName);

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = model.ReturnUrl });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(model.ReturnUrl);
					}
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return View(model);
		}


		private ApplicationUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<ApplicationUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
					$"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<ApplicationUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<ApplicationUser>)_userStore;
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);

			return RedirectToAction("Index", "Home");
		}
	}
}