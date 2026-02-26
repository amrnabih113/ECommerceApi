using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.DTOs
{
    public class ApiResponse<T> where T : class
    {
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}