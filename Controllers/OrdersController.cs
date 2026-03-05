using ECommerce.DTOs;
using ECommerce.DTOs.Orders;
using ECommerce.DTOs.Payments;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    /// <summary>
    /// Orders management endpoints

    [ApiController]
    [Route("api/orders")]
    [Authorize(Roles = "Admin,User")]
    [EnableRateLimiting("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        /// Get all orders (Admin only)
    
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<OrderDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _ordersService.GetAllOrdersAsync(page, pageSize);
            return Ok(response);
        }

        /// Get current user's orders
    
        [HttpGet("my-orders")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<OrderDto>>), 200)]
        public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _ordersService.GetUserOrdersAsync(userId, page, pageSize);
            return Ok(response);
        }

        /// Get order by ID (Admin can view any, users can view their own)
    
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");

            if (isAdmin)
            {
                var response = await _ordersService.GetOrderByIdAsync(id);
                return Ok(response);
            }
            else
            {
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

                var response = await _ordersService.GetUserOrderByIdAsync(id, userId);
                return Ok(response);
            }
        }

        /// Create a new order
    
        [HttpPost("create")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _ordersService.CreateOrderAsync(dto, userId);

            if (!response.Success)
                return BadRequest(response);

            if (response.Data == null)
                return BadRequest(ApiResponse<OrderDto>.ErrorResponse("Order created but data is null."));

            return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
        }

        /// Update order status (Admin only)
    
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var response = await _ordersService.UpdateOrderStatusAsync(id, dto);
            return Ok(response);
        }

        /// Cancel an order
    
        [HttpPost("{id:int}/cancel")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _ordersService.CancelOrderAsync(id, userId);
            return Ok(response);
        }

        /// Create payment intent for an order
    
        [HttpPost("{id:int}/payment")]
        [ProducesResponseType(typeof(ApiResponse<PaymentIntentResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> CreatePayment(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _ordersService.CreatePaymentIntentAsync(id, userId);
            return Ok(response);
        }
    }
}
