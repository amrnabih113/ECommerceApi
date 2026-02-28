using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.core.configs
{
    public class EmailSettingsConfig
    {
        public required string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public required string SenderName { get; set; }
        public required string SenderEmail { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}