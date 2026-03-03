using ECommerce.DTOs;
using ECommerce.DTOs.Categories;

namespace ECommerce.Interfaces.Services
{
    public interface ICategoriesService
    {
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetRootCategoriesAsync();
        Task<ApiResponse<PageResult<CategoryDto>>> GetAllCategoriesAsync(CategoryQueryDto query);
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(int parentId);
        Task<ApiResponse<CategoryDto>> GetByIdAsync(int id);
        Task<ApiResponse<CategoryDto>> CreateAsync(CategoryCreateDto dto);
        Task<ApiResponse<CategoryDto>> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<PageResult<CategoryDto>>> SearchAsync(string term, int page = 1, int pageSize = 10);
        Task<ApiResponse<IEnumerable<string>>> GetSearchRecommendationsAsync(string term, int size = 5);
    }
}