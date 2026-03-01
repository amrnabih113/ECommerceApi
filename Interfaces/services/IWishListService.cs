using ECommerce.DTOs;
using ECommerce.DTOs.WishLists;

namespace ECommerce.Interfaces.Services
{
    public interface IWishListService
    {
        Task<ApiResponse<WishListItemDto>> AddToWishListAsync(string userId, int productId);
        Task<ApiResponse> RemoveFromWishListAsync(string userId, int productId);
        Task<ApiResponse<PageResult<WishListItemDto>>> GetUserWishListAsync(string userId, int page = 1, int pageSize = 10);
        Task<ApiResponse<bool>> IsProductInWishListAsync(string userId, int productId);
    }
}
