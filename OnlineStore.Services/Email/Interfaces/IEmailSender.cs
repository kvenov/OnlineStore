namespace OnlineStore.Services.Core.Email.Interfaces
{
	public interface IEmailSender
	{
		Task SendAsync(string toEmail, string toName, string subject,
					   string htmlBody, string? textBody = null, 
					   CancellationToken ct = default);
	}
}
