
using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Categories;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiResponse<CategoryDto>> CreateAsync(CategoryCreateDto dto)
        {
            var Category = _mapper.Map<Category>(dto);
            Category.CreatedAt = DateTime.UtcNow;
            var createdCategory = await _unitOfWork.Categories.AddAsync(Category);
            await _unitOfWork.CompleteAsync();
            var CategoryDto = _mapper.Map<CategoryDto>(createdCategory);
            return ApiResponse<CategoryDto>.Success(CategoryDto, "Category created successfully.");

        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                throw new BadRequestException("Category not found.");
            }
            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.CompleteAsync();
            return ApiResponse<bool>.Success(true, "Category deleted successfully.");
        }

        public async Task<ApiResponse<CategoryDto>> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                throw new BadRequestException("Category not found.");
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.Success(categoryDto, "Category retrieved successfully.");


        }

        public async Task<ApiResponse<PageResult<CategoryDto>>> GetAllCategoriesAsync(CategoryQueryDto query)
        {
            var (categories, totalItems) = await _unitOfWork.Categories.GetPagedAsync(query);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            var pagedResult = new PageResult<CategoryDto>
            {
                Items = categoriesDto,
                TotalItems = totalItems,
                Page = query.Page,
                PageSize = query.PageSize
            };
            return ApiResponse<PageResult<CategoryDto>>.Success(pagedResult, "Categories retrieved successfully.");
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetRootCategoriesAsync()
        {
            var rootCategories = await _unitOfWork.Categories.GetRootCategoriesAsync();
            var rootCategoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(rootCategories);
            return ApiResponse<IEnumerable<CategoryDto>>.Success(rootCategoriesDto, "Root categories retrieved successfully.");
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(int parentId)
        {
            var parentCategory = await _unitOfWork.Categories.GetByIdAsync(parentId);
            if (parentCategory == null)
            {
                throw new BadRequestException("Parent category not found.");
            }
            var subCategories = await _unitOfWork.Categories.GetSubCategoriesAsync(parentId);
            var subCategoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(subCategories);
            return ApiResponse<IEnumerable<CategoryDto>>.Success(subCategoriesDto, "Sub categories retrieved successfully.");
        }

        public async Task<ApiResponse<CategoryDto>> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
            {
                throw new BadRequestException("Category not found.");
            }
            _mapper.Map(dto, category);
            category.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.CompleteAsync();
            var updatedCategoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.Success(updatedCategoryDto, "Category updated successfully.");
        }

        public async Task<ApiResponse<PageResult<CategoryDto>>> SearchAsync(string term, int page = 1, int pageSize = 10)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var (categories, totalItems) = await _unitOfWork.Categories.SearchAsync(term, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            var pagedResult = new PageResult<CategoryDto>
            {
                Items = dtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<CategoryDto>>.Success(pagedResult, "Categories search completed successfully.");
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetSearchRecommendationsAsync(string term, int size = 5)
        {
            var recommendations = await _unitOfWork.Categories.GetSearchRecommendationsAsync(term, size);
            return ApiResponse<IEnumerable<string>>.Success(recommendations, "Category recommendations fetched successfully.");
        }
    }
}