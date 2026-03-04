using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.DTOs.WishLists;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class WishListService : IWishListService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishListService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<WishListItemDto>> AddToWishListAsync(string userId, int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var existingWishListItem = await _unitOfWork.WishList.GetByUserAndProductAsync(userId, productId);
            if (existingWishListItem != null)
            {
                throw new BadRequestException("Product is already in your wishlist.");
            }

            var wishListItem = new WishList
            {
                UserId = userId,
                ProductId = productId
            };

            await _unitOfWork.WishList.AddAsync(wishListItem);
            await _unitOfWork.CompleteAsync();

            var dto = _mapper.Map<WishListItemDto>(wishListItem);
            if (dto.Product != null)
            {
                dto.Product.IsFavorite = true; // Set to true since it's added to wishlist
            }
            return ApiResponse<WishListItemDto>.SuccessResponse(dto, "Product added to wishlist successfully.");
        }

        public async Task<ApiResponse> RemoveFromWishListAsync(string userId, int productId)
        {
            var wishListItem = await _unitOfWork.WishList.GetByUserAndProductAsync(userId, productId);
            if (wishListItem == null)
            {
                throw new BadRequestException("Product is not in your wishlist.");
            }

            await _unitOfWork.WishList.DeleteAsync(wishListItem);
            await _unitOfWork.CompleteAsync();

            return ApiResponse.SuccessResponse("Product removed from wishlist successfully.");
        }

        public async Task<ApiResponse<PageResult<WishListItemDto>>> GetUserWishListAsync(string userId, int page = 1, int pageSize = 20)
        {
            var (items, totalItems) = await _unitOfWork.WishList.GetByUserIdAsync(userId, page, pageSize);
            var dtos = new List<WishListItemDto>();

            foreach (var item in items)
            {
                var dto = _mapper.Map<WishListItemDto>(item);
                if (dto.Product != null)
                {
                    dto.Product.IsFavorite = true;
                }
                dtos.Add(dto);
            }

            var pagedResult = new PageResult<WishListItemDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<WishListItemDto>>.SuccessResponse(pagedResult, "Wishlist fetched successfully.");
        }

        public async Task<ApiResponse<bool>> IsProductInWishListAsync(string userId, int productId)
        {
            var exists = await _unitOfWork.WishList.ExistsAsync(userId, productId);
            return ApiResponse<bool>.SuccessResponse(exists, "Wishlist item check completed.");
        }
    }
}
