using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Banners;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class BannersService : IBannersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BannersService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<BannerDto>> CreateAsync(BannerCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ImageUrl))
                throw new BadRequestException("Image URL is required.");

            var banner = _mapper.Map<Banner>(dto);
            banner.CreatedAt = DateTime.UtcNow;
            
            var createdBanner = await _unitOfWork.Banners.AddAsync(banner);
            await _unitOfWork.CompleteAsync();
            
            var bannerDto = _mapper.Map<BannerDto>(createdBanner);
            return ApiResponse<BannerDto>.SuccessResponse(bannerDto, "Banner created successfully.");
        }

        public async Task<ApiResponse<PageResult<BannerDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var (banners, totalItems) = await _unitOfWork.Banners.GetPagedAsync(page, pageSize);

            var dtos = _mapper.Map<IEnumerable<BannerDto>>(banners);

            var pagedResult = new PageResult<BannerDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<BannerDto>>.SuccessResponse(pagedResult, "Banners fetched successfully.");
        }

        public async Task<ApiResponse<BannerDto>> GetByIdAsync(int id)
        {
            var banner = await _unitOfWork.Banners.GetByIdAsync(id);
            if (banner == null)
                return ApiResponse<BannerDto>.Error("Banner not found.");

            var bannerDto = _mapper.Map<BannerDto>(banner);
            return ApiResponse<BannerDto>.SuccessResponse(bannerDto, "Banner fetched successfully.");
        }

        public async Task<ApiResponse<BannerDto>> UpdateAsync(int id, BannerUpdateDto dto)
        {
            var banner = await _unitOfWork.Banners.GetByIdAsync(id);
            if (banner == null)
                return ApiResponse<BannerDto>.Error("Banner not found.");

            // Only update fields that are provided
            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
                banner.ImageUrl = dto.ImageUrl;

            if (dto.IsActive.HasValue)
                banner.IsActive = dto.IsActive.Value;

            if (dto.Title != null)
                banner.Title = dto.Title;

            if (dto.Description != null)
                banner.Description = dto.Description;

            if (dto.Link != null)
                banner.Link = dto.Link;

            if (dto.DisplayOrder.HasValue)
                banner.DisplayOrder = dto.DisplayOrder.Value;

            banner.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Banners.UpdateAsync(banner);
            await _unitOfWork.CompleteAsync();

            var bannerDto = _mapper.Map<BannerDto>(banner);
            return ApiResponse<BannerDto>.SuccessResponse(bannerDto, "Banner updated successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var banner = await _unitOfWork.Banners.GetByIdAsync(id);
            if (banner == null)
                return ApiResponse.ErrorResponse("Banner not found.");

            await _unitOfWork.Banners.DeleteAsync(banner);
            await _unitOfWork.CompleteAsync();

            return ApiResponse.SuccessResponse("Banner deleted successfully.");
        }

        public async Task<ApiResponse<IEnumerable<BannerDto>>> GetActiveBannersAsync()
        {
            var banners = await _unitOfWork.Banners.GetActiveBannersOrderedAsync();
            var dtos = _mapper.Map<IEnumerable<BannerDto>>(banners);
            
            return ApiResponse<IEnumerable<BannerDto>>.SuccessResponse(dtos, "Active banners fetched successfully.");
        }
    }
}
