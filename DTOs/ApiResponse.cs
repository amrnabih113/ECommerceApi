namespace ECommerce.DTOs
{
    public class ApiResponse<T> : ApiResponse
    {

        public T? Data { get; set; }
    }

    public class ApiResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}