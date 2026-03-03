using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductsService(IUnitOfWork unitOfWork, IProductsRepository productRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductDto>> CreateAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.CreatedAt = DateTime.UtcNow;
            var createdProduct = await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            var productDto = _mapper.Map<ProductDto>(createdProduct);
            return ApiResponse<ProductDto>.Success(productDto, "Product created successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }
            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Product deleted successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductDto>>> GetAllAsync(ProductQueryDto query, string? userId = null)
        {
            var (products, totalItems) = await _unitOfWork.Products.GetPagedAsync(query);

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Set IsFavorite for each product if userId is provided
            if (!string.IsNullOrEmpty(userId))
            {
                foreach (var dto in dtos)
                {
                    var isFavorite = await _unitOfWork.WishList.ExistsAsync(userId, dto.Id);
                    dto.IsFavorite = isFavorite;
                }
            }

            var pagedResult = new PageResult<ProductDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = query.Page,
                PageSize = query.PageSize
            };

            return ApiResponse<PageResult<ProductDto>>.Success(pagedResult, "Products fetched successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductDto>>> GetByBrandAsync(int brandId, ProductQueryDto query, string? userId = null)
        {
            var (products, totalItems) = await _unitOfWork.Products.GetByBrandIdAsync(brandId, query);
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Set IsFavorite for each product if userId is provided
            if (!string.IsNullOrEmpty(userId))
            {
                foreach (var dto in dtos)
                {
                    var isFavorite = await _unitOfWork.WishList.ExistsAsync(userId, dto.Id);
                    dto.IsFavorite = isFavorite;
                }
            }

            var pagedResult = new PageResult<ProductDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = query.Page,
                PageSize = query.PageSize
            };
            return ApiResponse<PageResult<ProductDto>>.Success(pagedResult, "Products fetched successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductDto>>> GetByCategoryAsync(int categoryId, ProductQueryDto query, string? userId = null)
        {
            var (products, totalItems) = await _unitOfWork.Products.GetByCategoryIdAsync(categoryId, query);
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            // Set IsFavorite for each product if userId is provided
            if (!string.IsNullOrEmpty(userId))
            {
                foreach (var dto in dtos)
                {
                    var isFavorite = await _unitOfWork.WishList.ExistsAsync(userId, dto.Id);
                    dto.IsFavorite = isFavorite;
                }
            }

            var pagedResult = new PageResult<ProductDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = query.Page,
                PageSize = query.PageSize
            };
            return ApiResponse<PageResult<ProductDto>>.Success(pagedResult, "Products fetched successfully.");
        }

        public async Task<ApiResponse<ProductDetailsDto>> GetByIdAsync(int id, string? userId = null)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }
            var productDto = _mapper.Map<ProductDetailsDto>(product);

            // Set IsFavorite if userId is provided
            if (!string.IsNullOrEmpty(userId))
            {
                var isFavorite = await _unitOfWork.WishList.ExistsAsync(userId, productDto.Id);
                productDto.IsFavorite = isFavorite;
            }

            return ApiResponse<ProductDetailsDto>.Success(productDto, "Product fetched successfully.");
        }

        public async Task<ApiResponse<ProductDetailsDto>> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }
            _mapper.Map(dto, product);
            product.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CompleteAsync();
            var updatedProductDto = _mapper.Map<ProductDetailsDto>(product);
            return ApiResponse<ProductDetailsDto>.Success(updatedProductDto, "Product updated successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductDto>>> SearchAsync(string term, int page = 1, int pageSize = 10, string? userId = null)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var (products, totalItems) = await _unitOfWork.Products.SearchAsync(term, page, pageSize);

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            if (!string.IsNullOrEmpty(userId))
            {
                foreach (var dto in dtos)
                {
                    var isFavorite = await _unitOfWork.WishList.ExistsAsync(userId, dto.Id);
                    dto.IsFavorite = isFavorite;
                }
            }

            var pagedResult = new PageResult<ProductDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<ProductDto>>.Success(pagedResult, "Products search completed successfully.");
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetSearchRecommendationsAsync(string term, int size = 5)
        {
            var recommendations = await _unitOfWork.Products.GetSearchRecommendationsAsync(term, size);
            return ApiResponse<IEnumerable<string>>.Success(recommendations, "Product recommendations fetched successfully.");
        }


    }


}