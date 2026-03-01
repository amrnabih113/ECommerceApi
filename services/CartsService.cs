using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Carts;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class CartsService : ICartsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // User methods
        public async Task<ApiResponse<CartDto>> GetByUserIdAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
            if (cart == null)
            {
                throw new BadRequestException("Cart not found for this user.");
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            return ApiResponse<CartDto>.Success(cartDto, "Cart fetched successfully.");
        }

        public async Task<ApiResponse> ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
            if (cart == null)
            {
                throw new BadRequestException("Cart not found for this user.");
            }

            foreach (var item in cart.Items)
            {
                await _unitOfWork.CartItems.DeleteAsync(item);
            }
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Cart cleared successfully.");
        }

        public async Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
            if (cart == null)
            {
                throw new BadRequestException("Cart not found for this user.");
            }

            // Calculate totals
            decimal subtotal = 0;
            int totalItems = 0;

            if (cart.Items != null && cart.Items.Any())
            {
                subtotal = cart.Items.Sum(item => item.UnitPrice * item.Quantity);
                totalItems = cart.Items.Sum(item => item.Quantity);
            }

            // Tax rate: 14% (configurable)
            decimal taxRate = 0.14m;
            decimal tax = subtotal * taxRate;

            // Discount: 0 for now (can be extended with coupon system)
            decimal discount = 0;

            decimal total = subtotal + tax - discount;

            var summary = new CartSummaryDto
            {
                CartId = cart.Id,
                TotalItems = totalItems,
                Subtotal = subtotal,
                Tax = tax,
                Discount = discount,
                Total = total
            };

            return ApiResponse<CartSummaryDto>.Success(summary, "Cart summary calculated successfully.");
        }

        // Admin methods
        public async Task<ApiResponse<PageResult<CartDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var (carts, totalItems) = await _unitOfWork.Carts.GetPagedAsync(page, pageSize);
            var dtos = _mapper.Map<IEnumerable<CartDto>>(carts);
            var pagedResult = new PageResult<CartDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
            return ApiResponse<PageResult<CartDto>>.Success(pagedResult, "Carts fetched successfully.");
        }

        public async Task<ApiResponse<CartDto>> GetByIdAsync(int id)
        {
            var cart = await _unitOfWork.Carts.GetByIdAsync(id);
            if (cart == null)
            {
                throw new BadRequestException("Cart not found.");
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            return ApiResponse<CartDto>.Success(cartDto, "Cart fetched successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var cart = await _unitOfWork.Carts.GetByIdAsync(id);
            if (cart == null)
            {
                throw new BadRequestException("Cart not found.");
            }

            // Delete all items in the cart first
            foreach (var item in cart.Items)
            {
                await _unitOfWork.CartItems.DeleteAsync(item);
            }

            await _unitOfWork.Carts.DeleteAsync(cart);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Cart deleted successfully.");
        }
    }
}
