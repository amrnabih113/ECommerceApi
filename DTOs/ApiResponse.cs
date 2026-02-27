namespace ECommerce.DTOs
{
    public class ApiResponse
    {
        public bool Success { get; }
        public string Message { get; }

        protected ApiResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public static ApiResponse SuccessResponse(string message = "")
            => new ApiResponse(true, message);

        public static ApiResponse ErrorResponse(string message)
            => new ApiResponse(false, message);
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; }

        private ApiResponse(bool success, string message, T? data)
            : base(success, message)
        {
            Data = data;
        }

        public static new ApiResponse<T> Success(T data, string message = "")
            => new ApiResponse<T>(true, message, data);

        public static ApiResponse<T> Error(string message)
            => new ApiResponse<T>(false, message, default);
    }
}