namespace ECommerce.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string code);
    }
}
