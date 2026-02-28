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
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductsService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductDto>> CreateAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var createdProduct = await _productRepository.AddAsync(product);
            var productDto = _mapper.Map<ProductDto>(createdProduct);
            return ApiResponse<ProductDto>.Success(productDto, "Product created successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }
            await _productRepository.DeleteAsync(product);
            return ApiResponse.SuccessResponse("Product deleted successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var (products, totalItems) = await _productRepository.GetPagedAsync(page, pageSize);

            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            var pagedResult = new PageResult<ProductDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<ProductDto>>.Success(pagedResult, "Products fetched successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductDto>>> GetByBrandAsync(int brandId, int page = 1, int pageSize = 10)
        {
            var (products, totalItems) = await _productRepository.GetByBrandIdAsync(brandId, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            var pagedResult = new PageResult<ProductDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
            return ApiResponse<PageResult<ProductDto>>.Success(pagedResult, "Products fetched successfully.");
        }

        public async Task<ApiResponse<PageResult<ProductDto>>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10)
        {
            var (products, totalItems) = await _productRepository.GetByCategoryIdAsync(categoryId, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            var pagedResult = new PageResult<ProductDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
            return ApiResponse<PageResult<ProductDto>>.Success(pagedResult, "Products fetched successfully.");
        }

        public async Task<ApiResponse<ProductDto>> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return ApiResponse<ProductDto>.Success(productDto, "Product fetched successfully.");
        }

        public async Task<ApiResponse<ProductDto>> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }
            _mapper.Map(dto, product);
            await _productRepository.UpdateAsync(product);
            var updatedProductDto = _mapper.Map<ProductDto>(product);
            return ApiResponse<ProductDto>.Success(updatedProductDto, "Product updated successfully.");
        }
    }


}