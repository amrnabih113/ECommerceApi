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
            // Validate product exists and is active
            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            if (!product.IsActive)
            {
                throw new BadRequestException("Product is not available for purchase.");
            }

            if (product.HasVariants && !dto.ProductVariantId.HasValue)
            {
                throw new BadRequestException("This product has variants. You must select a variant (size, color, etc.).");
            }

            //  If HasVariants = false, VariantId must be NULL
            if (!product.HasVariants && dto.ProductVariantId.HasValue)
            {
                throw new BadRequestException("This product does not have variants. VariantId must not be provided.");
            }

            ProductVariant? productVariant = null;
            int availableStock = product.StockQuantity;
            decimal unitPrice = product.Price;

            // Handle products WITH variants
            if (product.HasVariants && dto.ProductVariantId.HasValue)
            {
                productVariant = await _unitOfWork.ProductVariants.GetByIdAsync(dto.ProductVariantId.Value);
                if (productVariant == null || productVariant.ProductId != product.Id)
                {
                    throw new BadRequestException("Product variant not found or does not belong to this product.");
                }

                // Use variant stock
                availableStock = productVariant.StockQuantity;

                // Calculate unit price with variant additional price
                if (productVariant.AdditionalPrice.HasValue)
                {
                    unitPrice += productVariant.AdditionalPrice.Value;
                }
            }

            // Validate stock
            if (availableStock < dto.Quantity)
            {
                throw new BadRequestException($"Insufficient stock. Available: {availableStock}, Requested: {dto.Quantity}");
            }

            // Get or create cart
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

            // Check if item already exists in cart
            CartItem? existingItem = null;
            if (product.HasVariants && dto.ProductVariantId.HasValue)
            {
                existingItem = await _unitOfWork.CartItems.GetByCartIdAndProductVariantIdAsync(cart.Id, dto.ProductVariantId.Value);
            }
            else
            {
                existingItem = await _unitOfWork.CartItems.GetByCartIdAndProductIdAsync(cart.Id, dto.ProductId);
            }

            if (existingItem != null)
            {
                // Validate total quantity won't exceed available stock
                if (availableStock < (existingItem.Quantity + dto.Quantity))
                {
                    throw new BadRequestException($"Insufficient stock. Available: {availableStock}, Current in cart: {existingItem.Quantity}, Requested to add: {dto.Quantity}");
                }

                existingItem.Quantity += dto.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
                await _unitOfWork.CompleteAsync();

                var existingItemDto = _mapper.Map<CartItemDto>(existingItem);
                return ApiResponse<CartItemDto>.SuccessResponse(existingItemDto, "Product quantity updated in cart.");
            }

            // Add new item to cart
            var cartItem = new CartItem
            {
                Quantity = dto.Quantity,
                ProductId = dto.ProductId,
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
            return ApiResponse<CartItemDto>.SuccessResponse(cartItemDto, "Product added to cart successfully.");
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

            // Determine available stock
            int availableStock;
            if (cartItem.ProductVariantId.HasValue)
            {
                var productVariant = cartItem.ProductVariant;
                if (productVariant == null)
                {
                    throw new BadRequestException("Product variant not found.");
                }
                availableStock = productVariant.StockQuantity;
            }
            else
            {
                var product = cartItem.Product;
                if (product == null)
                {
                    throw new BadRequestException("Product not found.");
                }
                availableStock = product.StockQuantity;
            }

            // Validate stock
            if (availableStock < quantity)
            {
                throw new BadRequestException($"Insufficient stock. Available: {availableStock}, Requested: {quantity}");
            }

            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CartItems.UpdateAsync(cartItem);
            await _unitOfWork.CompleteAsync();

            var updatedCartItemDto = _mapper.Map<CartItemDto>(cartItem);
            return ApiResponse<CartItemDto>.SuccessResponse(updatedCartItemDto, "Cart item quantity updated successfully.");
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
            return ApiResponse<PageResult<CartItemDto>>.SuccessResponse(pagedResult, "Cart items fetched successfully.");
        }

        public async Task<ApiResponse<CartItemDto>> GetByIdAsync(int id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null)
            {
                throw new BadRequestException("Cart item not found.");
            }
            var cartItemDto = _mapper.Map<CartItemDto>(cartItem);
            return ApiResponse<CartItemDto>.SuccessResponse(cartItemDto, "Cart item fetched successfully.");
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