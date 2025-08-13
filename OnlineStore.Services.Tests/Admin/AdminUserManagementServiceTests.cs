using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using OnlineStore.Data.Models;
using OnlineStore.Services.Core.Admin;
using OnlineStore.Services.Core.Admin.Interfaces;

namespace OnlineStore.Services.Tests.Admin
{
	[TestFixture]
	public class AdminUserManagementServiceTests
	{
		private Mock<UserManager<ApplicationUser>> _mockUserManager;
		private Mock<RoleManager<IdentityRole>> _mockRoleManager;
		private IAdminUserManagementService _service;

		[SetUp]
		public void SetUp()
		{
			_mockUserManager = new Mock<UserManager<ApplicationUser>>(
				Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

			_mockRoleManager = new Mock<RoleManager<IdentityRole>>(
				Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

			_service = new AdminUserManagementService(_mockUserManager.Object, _mockRoleManager.Object);
		}

		[Test]
		public void AssignUserToRoleAsync_Throws_IfUserIdOrRoleIsNullOrWhitespace()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _service.AssignUserToRoleAsync(null, "Admin"));
			Assert.ThrowsAsync<ArgumentNullException>(() => _service.AssignUserToRoleAsync("user1", null));
			Assert.ThrowsAsync<ArgumentException>(() => _service.AssignUserToRoleAsync("", "Admin"));
			Assert.ThrowsAsync<ArgumentException>(() => _service.AssignUserToRoleAsync("user1", ""));
			Assert.ThrowsAsync<ArgumentException>(() => _service.AssignUserToRoleAsync(" ", "Admin"));
			Assert.ThrowsAsync<ArgumentException>(() => _service.AssignUserToRoleAsync("user1", " "));
		}

		[Test]
		public void AssignUserToRoleAsync_Throws_IfUserNotFound()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser)null);

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.AssignUserToRoleAsync("user1", "Admin"));
			Assert.That(ex.Message, Does.Contain("cannot be found"));
		}

		[Test]
		public void AssignUserToRoleAsync_Throws_IfRoleDoesNotExist()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockRoleManager.Setup(m => m.RoleExistsAsync("Admin")).ReturnsAsync(false);

			var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.AssignUserToRoleAsync("user1", "Admin"));
			Assert.That(ex.Message, Does.Contain("not valid app role"));
		}

		[Test]
		public async Task AssignUserToRoleAsync_ReturnsTrue_WhenSuccess()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockRoleManager.Setup(m => m.RoleExistsAsync("Admin")).ReturnsAsync(true);
			_mockUserManager.Setup(m => m.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

			var result = await _service.AssignUserToRoleAsync("user1", "Admin");
			Assert.That(result, Is.True);
		}

		[Test]
		public void AssignUserToRoleAsync_Throws_IfAddToRoleFails()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockRoleManager.Setup(m => m.RoleExistsAsync("Admin")).ReturnsAsync(true);
			_mockUserManager.Setup(m => m.AddToRoleAsync(user, "Admin")).ThrowsAsync(new Exception("fail"));

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.AssignUserToRoleAsync("user1", "Admin"));
			Assert.That(ex.Message, Does.Contain("assigning role"));
		}

		[Test]
		public void RemoveRoleFromUserAsync_Throws_IfUserIdOrRoleIsNullOrWhitespace()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemoveRoleFromUserAsync(null, "Admin"));
			Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemoveRoleFromUserAsync("user1", null));
			Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveRoleFromUserAsync("", "Admin"));
			Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveRoleFromUserAsync("user1", ""));
			Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveRoleFromUserAsync(" ", "Admin"));
			Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveRoleFromUserAsync("user1", " "));
		}

		[Test]
		public void RemoveRoleFromUserAsync_Throws_IfUserNotFound()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser)null);

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoveRoleFromUserAsync("user1", "Admin"));
			Assert.That(ex.Message, Does.Contain("cannot be found"));
		}

		[Test]
		public void RemoveRoleFromUserAsync_Throws_IfRoleDoesNotExist()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockRoleManager.Setup(m => m.RoleExistsAsync("Admin")).ReturnsAsync(false);

			var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveRoleFromUserAsync("user1", "Admin"));
			Assert.That(ex.Message, Does.Contain("not valid app role"));
		}

		[Test]
		public async Task RemoveRoleFromUserAsync_ReturnsTrue_WhenSuccess()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockRoleManager.Setup(m => m.RoleExistsAsync("Admin")).ReturnsAsync(true);
			_mockUserManager.Setup(m => m.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

			var result = await _service.RemoveRoleFromUserAsync("user1", "Admin");
			Assert.That(result, Is.True);
		}

		[Test]
		public void RemoveRoleFromUserAsync_Throws_IfRemoveFromRoleFails()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockRoleManager.Setup(m => m.RoleExistsAsync("Admin")).ReturnsAsync(true);
			_mockUserManager.Setup(m => m.RemoveFromRoleAsync(user, "Admin")).ThrowsAsync(new Exception("fail"));

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoveRoleFromUserAsync("user1", "Admin"));
			Assert.That(ex.Message, Does.Contain("removing role"));
		}

		[Test]
		public void SoftDeleteUserAsync_Throws_IfUserIdIsNullOrWhitespace()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _service.SoftDeleteUserAsync(null));
			Assert.ThrowsAsync<ArgumentException>(() => _service.SoftDeleteUserAsync(""));
			Assert.ThrowsAsync<ArgumentException>(() => _service.SoftDeleteUserAsync(" "));
		}

		[Test]
		public void SoftDeleteUserAsync_Throws_IfUserNotFound()
		{
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync((ApplicationUser)null);

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDeleteUserAsync("user1"));
			Assert.That(ex.Message, Does.Contain("cannot be found"));
		}

		[Test]
		public async Task SoftDeleteUserAsync_ReturnsTrue_WhenSuccess()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

			var result = await _service.SoftDeleteUserAsync("user1");
			Assert.That(result, Is.True);
		}

		[Test]
		public void SoftDeleteUserAsync_Throws_IfDeleteFails()
		{
			var user = new ApplicationUser { Id = "user1" };
			_mockUserManager.Setup(m => m.FindByIdAsync("user1")).ReturnsAsync(user);
			_mockUserManager.Setup(m => m.DeleteAsync(user)).ThrowsAsync(new Exception("fail"));

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.SoftDeleteUserAsync("user1"));
			Assert.That(ex.Message, Does.Contain("deleting the User"));
		}

		[Test]
		public void RenewUserAsync_Throws_IfUserIdIsNullOrWhitespace()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _service.RenewUserAsync(null));
			Assert.ThrowsAsync<ArgumentException>(() => _service.RenewUserAsync(""));
			Assert.ThrowsAsync<ArgumentException>(() => _service.RenewUserAsync("   "));
		}

		[Test]
		public void RenewUserAsync_Throws_IfUserNotFound()
		{
			var usersQueryable = new List<ApplicationUser>().BuildMockDbSet();
			_mockUserManager.Setup(m => m.Users).Returns(usersQueryable.Object);

			InvalidOperationException? ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.RenewUserAsync("user1"));
			string expectedExMessage = "User with the provided credentials cannot be found!";

			Assert.That(expectedExMessage, Is.EqualTo(ex.Message));
		}

		[Test]
		public async Task RenewUserAsync_ReturnsTrue_WhenSuccess()
		{
			var user = new ApplicationUser { Id = "user1", IsDeleted = true };
			var usersQueryable = new List<ApplicationUser> { user }.BuildMockDbSet();
			_mockUserManager.Setup(m => m.Users).Returns(usersQueryable.Object);
			_mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

			var result = await _service.RenewUserAsync("user1");
			Assert.That(result, Is.True);
			Assert.That(user.IsDeleted, Is.False);
		}

		[Test]
		public void RenewUserAsync_Throws_IfUpdateFails()
		{
			var user = new ApplicationUser { Id = "user1", IsDeleted = true };
			var usersQueryable = new List<ApplicationUser> { user }.BuildMockDbSet();
			_mockUserManager.Setup(m => m.Users).Returns(usersQueryable.Object);
			_mockUserManager.Setup(m => m.UpdateAsync(user)).ThrowsAsync(new Exception("fail"));

			var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _service.RenewUserAsync("user1"));
			Assert.That(ex.Message, Does.Contain("renewed the User"));
		}

		[Test]
		public async Task GetAllUsersAsync_ReturnsEmpty_WhenNoUsers()
		{
			var usersQueryable = new List<ApplicationUser>().BuildMockDbSet();
			_mockUserManager.Setup(m => m.Users).Returns(usersQueryable.Object);

			var result = await _service.GetAllUsersAsync("admin");
			Assert.That(result, Is.Empty);
		}

		[Test]
		public async Task GetAllUsersAsync_ReturnsUserViewModels_WithRoles()
		{
			var user1 = new ApplicationUser { Id = "user1", UserName = "User1", FullName = "User One", Email = "user1@email.com", IsDeleted = false };
			var user2 = new ApplicationUser { Id = "user2", UserName = "User2", FullName = "User Two", Email = "user2@email.com", IsDeleted = true };
			var usersQueryable = new List<ApplicationUser> { user1, user2 }.BuildMockDbSet();

			_mockUserManager.Setup(m => m.Users).Returns(usersQueryable.Object);
			_mockUserManager.Setup(m => m.GetRolesAsync(user1)).ReturnsAsync(new List<string> { "User" });
			_mockUserManager.Setup(m => m.GetRolesAsync(user2)).ReturnsAsync(new List<string> { "Admin", "Manager" });

			var result = await _service.GetAllUsersAsync("admin");

			Assert.That(result.Count(), Is.EqualTo(2));
			var vm1 = result.First(r => r.Id == "user1");
			var vm2 = result.First(r => r.Id == "user2");
			Assert.That(vm1.Username, Is.EqualTo("User1"));
			Assert.That(vm1.FullName, Is.EqualTo("User One"));
			Assert.That(vm1.Email, Is.EqualTo("user1@email.com"));
			Assert.That(vm1.Roles, Contains.Item("User"));
			Assert.That(vm1.IsDeleted, Is.False);

			Assert.That(vm2.Username, Is.EqualTo("User2"));
			Assert.That(vm2.FullName, Is.EqualTo("User Two"));
			Assert.That(vm2.Email, Is.EqualTo("user2@email.com"));
			Assert.That(vm2.Roles, Contains.Item("Admin"));
			Assert.That(vm2.Roles, Contains.Item("Manager"));
			Assert.That(vm2.IsDeleted, Is.True);
		}
	}
}