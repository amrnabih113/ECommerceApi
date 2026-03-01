using ECommerce.DTOs;
using ECommerce.DTOs.CartItems;

namespace ECommerce.Interfaces.Services
{
    public interface ICartItemsService
    {
        // User methods
        Task<ApiResponse<CartItemDto>> AddToCartAsync(string userId, CartItemCreateDto dto);
        Task<ApiResponse<CartItemDto>> UpdateQuantityAsync(string userId, int cartItemId, int quantity);
        Task<ApiResponse> RemoveFromCartAsync(string userId, int cartItemId);

        // Admin methods
        Task<ApiResponse> DeleteAsync(int id);
    }
}
