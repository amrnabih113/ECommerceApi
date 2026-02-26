namespace ECommerce.core.Exceptions
{

    public class InvalidOtpException : Exception
    {
        public InvalidOtpException() : base("Invalid OTP") { }
    }
}