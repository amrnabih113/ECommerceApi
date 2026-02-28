using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Brands;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class BrandsService : IBrandsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<BrandDto>> CreateAsync(BrandCreateDto dto)
        {
            var brand = _mapper.Map<Brand>(dto);
            brand.CreatedAt = DateTime.UtcNow;
            var createdBrand = await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.CompleteAsync();
            var brandDto = _mapper.Map<BrandDto>(createdBrand);
            return ApiResponse<BrandDto>.Success(brandDto, "Brand created successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                throw new BadRequestException("Brand not found.");
            }

            await _unitOfWork.Brands.DeleteAsync(brand);
            await _unitOfWork.CompleteAsync();
            return ApiResponse<bool>.Success(true, "Brand deleted successfully.");
        }

        public async Task<ApiResponse<PageResult<BrandDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var (brands, totalItems) = await _unitOfWork.Brands.GetPagedAsync(page, pageSize);
            var dtos = _mapper.Map<IEnumerable<BrandDto>>(brands);
            var pagedResult = new PageResult<BrandDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };
            return ApiResponse<PageResult<BrandDto>>.Success(pagedResult, "Brands fetched successfully.");
        }

        public async Task<ApiResponse<BrandDto>> GetByIdAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                throw new BadRequestException("Brand not found.");
            }
            var brandDto = _mapper.Map<BrandDto>(brand);
            return ApiResponse<BrandDto>.Success(brandDto, "Brand fetched successfully.");
        }

        public async Task<ApiResponse<BrandDto>> UpdateAsync(int id, BrandUpdateDto dto)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                throw new BadRequestException("Brand not found.");
            }
            _mapper.Map(dto, brand);
            brand.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Brands.UpdateAsync(brand);
            await _unitOfWork.CompleteAsync();
            var updatedBrandDto = _mapper.Map<BrandDto>(brand);
            return ApiResponse<BrandDto>.Success(updatedBrandDto, "Brand updated successfully.");
        }
    }
}