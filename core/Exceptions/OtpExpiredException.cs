namespace ECommerce.core.Exceptions
{
    public class OtpExpiredException : Exception
    {
        public OtpExpiredException() : base("OTP expired") { }
    }
}