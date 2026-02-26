using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.core.Exceptions
{
    public class OtpExpiredException : Exception
    {
        public OtpExpiredException() : base("OTP expired") { }
    }

    public class InvalidOtpException : Exception
    {
        public InvalidOtpException() : base("Invalid OTP") { }
    }
}