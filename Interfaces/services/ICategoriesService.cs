using ECommerce.DTOs;
using ECommerce.DTOs.Categories;

namespace ECommerce.Interfaces.Services
{
    public interface ICategoriesService
    {
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetRootCategoriesAsync();
        Task<ApiResponse<PageResult<CategoryDto>>> GetAllCategoriesAsync(int page = 1, int pageSize = 20);
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(int parentId);
        Task<ApiResponse<CategoryDto>> GetByIdAsync(int id);
        Task<ApiResponse<CategoryDto>> CreateAsync(CategoryCreateDto dto);
        Task<ApiResponse<CategoryDto>> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}