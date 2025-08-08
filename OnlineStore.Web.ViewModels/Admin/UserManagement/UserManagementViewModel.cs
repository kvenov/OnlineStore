namespace OnlineStore.Web.ViewModels.Admin.UserManagement
{
	public class UserManagementViewModel
	{

		public string Id { get; set; } = null!;

		public string Username { get; set; } = null!;

		public string? FullName { get; set; }

		public string Email { get; set; } = null!;

		public IEnumerable<string> Roles { get; set; } = new List<string>();

		public bool IsDeleted { get; set; }
	}
}
