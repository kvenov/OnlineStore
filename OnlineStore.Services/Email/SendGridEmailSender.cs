using Microsoft.Extensions.Configuration;
using OnlineStore.Services.Core.Email.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace OnlineStore.Services.Core.Email
{
	public class SendGridEmailSender : IEmailSender
	{
		private readonly string _apiKey;
		private readonly string _fromEmail;
		private readonly string _fromName;

		public SendGridEmailSender(IConfiguration config)
		{
			_apiKey = config["SendGrid:ApiKey"] ?? "";
			_fromEmail = config["SendGrid:FromEmail"] ?? "no-reply@yourstore.test";
			_fromName = config["SendGrid:FromName"] ?? "OnlineStore";
		}

		public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, string? textBody = null, CancellationToken ct = default)
		{
			var client = new SendGridClient(_apiKey);
			var from = new EmailAddress(_fromEmail, _fromName);
			var to = new EmailAddress(toEmail, toName);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, textBody, htmlBody);
			var response = await client.SendEmailAsync(msg, ct);
		}
	}
}
