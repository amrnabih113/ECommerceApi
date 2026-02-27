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

            // Generate HTML body
            var htmlBody = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>OTP Verification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 50px auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            font-size: 24px;
            font-weight: bold;
            color: #333333;
        }}
        .content {{
            margin-top: 20px;
            font-size: 16px;
            color: #555555;
            line-height: 1.5;
        }}
        .otp {{
            display: block;
            font-size: 32px;
            font-weight: bold;
            color: #1a73e8;
            text-align: center;
            margin: 20px 0;
            letter-spacing: 5px;
        }}
        .footer {{
            margin-top: 30px;
            font-size: 12px;
            color: #999999;
            text-align: center;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">Your OTP Code</div>
        <div class=""content"">
            <p>Hello,</p>
            <p>Use the following One-Time Password (OTP) to complete your verification:</p>
            <span class=""otp"">{code}</span>
            <p>This OTP is valid for 5 minutes. Do not share it with anyone.</p>
        </div>
        <div class=""footer"">
            &copy; {DateTime.UtcNow.Year} Your Company. All rights reserved.
        </div>
    </div>
</body>
</html>
";

            // Use HTML instead of plain text
            message.Body = new TextPart("html") { Text = htmlBody };

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
