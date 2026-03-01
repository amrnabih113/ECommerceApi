using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.CartItems;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class CartItemsService : ICartItemsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CartItemDto>> AddToCartAsync(string userId, CartItemCreateDto dto)
        {
            // Validate product variant exists
            var productVariant = await _unitOfWork.ProductVariants.GetByIdAsync(dto.ProductVariantId);
            if (productVariant == null)
            {
                throw new BadRequestException("Product variant not found.");
            }

            // Validate product exists and is active
            var product = productVariant.Product;
            if (product == null || !product.IsActive)
            {
                throw new BadRequestException("Product is not available for purchase.");
            }

            // Validate stock
            if (productVariant.StockQuantity < dto.Quantity)
            {
                throw new BadRequestException($"Insufficient stock. Available: {productVariant.StockQuantity}, Requested: {dto.Quantity}");
            }

    
            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                cart = await _unitOfWork.Carts.AddAsync(cart);
            }

            // Check if variant already exists in cart
            var existingItem = await _unitOfWork.CartItems.GetByCartIdAndProductVariantIdAsync(cart.Id, dto.ProductVariantId);
            if (existingItem != null)
            {
                // Validate total quantity won't exceed available stock
                if (productVariant.StockQuantity < (existingItem.Quantity + dto.Quantity))
                {
                    throw new BadRequestException($"Insufficient stock. Available: {productVariant.StockQuantity}, Current in cart: {existingItem.Quantity}, Requested to add: {dto.Quantity}");
                }

                existingItem.Quantity += dto.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
                await _unitOfWork.CompleteAsync();
                var existingItemDto = _mapper.Map<CartItemDto>(existingItem);
                return ApiResponse<CartItemDto>.Success(existingItemDto, "Product quantity updated in cart.");
            }

            // Calculate unit price (snapshot at time of adding to cart)
            decimal unitPrice = product.Price;
            if (productVariant.AdditionalPrice.HasValue)
            {
                unitPrice += productVariant.AdditionalPrice.Value;
            }

            // Add new item to cart
            var cartItem = new CartItem
            {
                Quantity = dto.Quantity,
                ProductVariantId = dto.ProductVariantId,
                UnitPrice = unitPrice,
                CartId = cart.Id,
                CreatedAt = DateTime.UtcNow
            };
            var createdCartItem = await _unitOfWork.CartItems.AddAsync(cartItem);
            await _unitOfWork.CompleteAsync();

            // Reload the cart item with relations for mapping
            var item = await _unitOfWork.CartItems.GetByIdAsync(createdCartItem.Id);
            var cartItemDto = _mapper.Map<CartItemDto>(item);
            return ApiResponse<CartItemDto>.Success(cartItemDto, "Product added to cart successfully.");
        }

        public async Task<ApiResponse<CartItemDto>> UpdateQuantityAsync(string userId, int cartItemId, int quantity)
        {
            if (quantity < 1 || quantity > 999)
            {
                throw new BadRequestException("Quantity must be between 1 and 999.");
            }

            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem == null)
            {
                throw new BadRequestException("Cart item not found.");
            }

            // Verify the cart item belongs to the user's cart
            var userCart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
            if (userCart == null || cartItem.CartId != userCart.Id)
            {
                throw new BadRequestException("Cart item does not belong to your cart.");
            }

            // Validate stock before updating
            var productVariant = cartItem.ProductVariant;
            if (productVariant == null)
            {
                throw new BadRequestException("Product variant not found.");
            }

            if (productVariant.StockQuantity < quantity)
            {
                throw new BadRequestException($"Insufficient stock. Available: {productVariant.StockQuantity}, Requested: {quantity}");
            }

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CartItems.UpdateAsync(cartItem);
            await _unitOfWork.CompleteAsync();
            var updatedCartItemDto = _mapper.Map<CartItemDto>(cartItem);
            return ApiResponse<CartItemDto>.Success(updatedCartItemDto, "Cart item quantity updated successfully.");
        }

        public async Task<ApiResponse> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem == null)
            {
                throw new BadRequestException("Cart item not found.");
            }

            // Verify the cart item belongs to the user's cart
            var userCart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
            if (userCart == null || cartItem.CartId != userCart.Id)
            {
                throw new BadRequestException("Cart item does not belong to your cart.");
            }

            await _unitOfWork.CartItems.DeleteAsync(cartItem);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Product removed from cart successfully.");
        }

        // Admin methods
        public async Task<ApiResponse<PageResult<CartItemDto>>> GetByCartIdAsync(int cartId, int page = 1, int pageSize = 10)
        {
            var (cartItems, totalItems) = await _unitOfWork.CartItems.GetByCartIdAsync(cartId, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<CartItemDto>>(cartItems);
            var pagedResult = new PageResult<CartItemDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
            return ApiResponse<PageResult<CartItemDto>>.Success(pagedResult, "Cart items fetched successfully.");
        }

        public async Task<ApiResponse<CartItemDto>> GetByIdAsync(int id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null)
            {
                throw new BadRequestException("Cart item not found.");
            }
            var cartItemDto = _mapper.Map<CartItemDto>(cartItem);
            return ApiResponse<CartItemDto>.Success(cartItemDto, "Cart item fetched successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null)
            {
                throw new BadRequestException("Cart item not found.");
            }
            await _unitOfWork.CartItems.DeleteAsync(cartItem);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Cart item deleted successfully.");
        }
    }
}