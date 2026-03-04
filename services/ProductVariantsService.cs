using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class ProductVariantsService : IProductVariantsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductVariantsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductVariantDto>> CreateAsync(int productId, ProductVariantCreateDto dto)
        {
            // Verify product exists and is active
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            if (!product.IsActive)
            {
                throw new BadRequestException("Cannot add variants to an inactive product.");
            }

            // Create and map variant
            var variant = _mapper.Map<ProductVariant>(dto);
            variant.ProductId = productId;
            variant.CreatedAt = DateTime.UtcNow;

            var createdVariant = await _unitOfWork.ProductVariants.AddAsync(variant);

            // Mark product as having variants and recalculate stock
            product.HasVariants = true;
            await RecalculateProductStockAsync(productId);

            await _unitOfWork.CompleteAsync();

            var variantDto = _mapper.Map<ProductVariantDto>(createdVariant);
            return ApiResponse<ProductVariantDto>.SuccessResponse(variantDto, "Product variant created successfully.");
        }

        public async Task<ApiResponse<ProductVariantDto>> UpdateAsync(int productId, int variantId, ProductVariantUpdateDto dto)
        {
            // Verify product exists
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            // Get variant
            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(variantId);
            if (variant == null || variant.ProductId != productId)
            {
                throw new BadRequestException("Variant not found for this product.");
            }

            // Update only non-null fields
            _mapper.Map(dto, variant);
            variant.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ProductVariants.UpdateAsync(variant);

            // Recalculate product stock if stock was updated
            if (dto.StockQuantity.HasValue)
            {
                await RecalculateProductStockAsync(productId);
            }

            await _unitOfWork.CompleteAsync();

            var updatedVariantDto = _mapper.Map<ProductVariantDto>(variant);
            return ApiResponse<ProductVariantDto>.SuccessResponse(updatedVariantDto, "Product variant updated successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int productId, int variantId)
        {
            // Verify product exists
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            // Get variant
            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(variantId);
            if (variant == null || variant.ProductId != productId)
            {
                throw new BadRequestException("Variant not found for this product.");
            }

            // Prevent deletion if variant has active cart items
            var cartItemsCount = await _unitOfWork.CartItems.CountByProductVariantAsync(variantId);
            if (cartItemsCount > 0)
            {
                throw new BadRequestException("Cannot delete variant that has active cart items. Please remove from carts first.");
            }

            await _unitOfWork.ProductVariants.DeleteAsync(variant);

            // Recalculate product stock and check if any variants remain
            var remainingVariantsCount = await _unitOfWork.ProductVariants.CountByProductAsync(productId);
            if (remainingVariantsCount == 0)
            {
                // No more variants, set HasVariants to false and stock to 0
                product.HasVariants = false;
                product.StockQuantity = 0;
                product.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Products.UpdateAsync(product);
            }
            else
            {
                // Recalculate stock from remaining variants
                await RecalculateProductStockAsync(productId);
            }

            await _unitOfWork.CompleteAsync();

            return ApiResponse.SuccessResponse("Product variant deleted successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductVariantDto>>> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10)
        {
            // Verify product exists
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var (variants, totalItems) = await _unitOfWork.ProductVariants.GetByProductIdAsync(productId, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<ProductVariantDto>>(variants);

            var pagedResult = new PageResult<ProductVariantDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<ProductVariantDto>>.SuccessResponse(pagedResult, "Product variants fetched successfully.");
        }

        public async Task<ApiResponse<ProductVariantDto>> GetByIdAsync(int productId, int variantId)
        {
            // Verify product exists
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var variant = await _unitOfWork.ProductVariants.GetByIdAsync(variantId);
            if (variant == null || variant.ProductId != productId)
            {
                throw new BadRequestException("Variant not found for this product.");
            }

            var variantDto = _mapper.Map<ProductVariantDto>(variant);
            return ApiResponse<ProductVariantDto>.SuccessResponse(variantDto, "Product variant fetched successfully.");
        }

        // Helper method to recalculate product stock from all variants
        private async Task RecalculateProductStockAsync(int productId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return;

            var totalStock = await _unitOfWork.ProductVariants.GetTotalStockByProductAsync(productId);
            product.StockQuantity = totalStock;
            product.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Products.UpdateAsync(product);
        }
    }
}
