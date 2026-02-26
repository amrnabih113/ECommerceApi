using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string code);
    }
}
