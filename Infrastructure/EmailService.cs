using ECommerce.Interfaces.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace ECommerce.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config) => _config = config;

        public async Task SendOtpAsync(string toEmail, string code)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"])
            );
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Your OTP Code";
            message.Body = new TextPart("plain") { Text = $"Your OTP code is: {code}" };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                emailSettings["SmtpServer"],
                int.Parse(emailSettings["SmtpPort"]),
                MailKit.Security.SecureSocketOptions.StartTls
            );
            await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
