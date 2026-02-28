using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.DTOs;
using ECommerce.DTOs.Categories;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize(Roles = "Admin,User")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet()]
        [Route("root-categories")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<CategoryDto>>), 200)]
        public async Task<IActionResult> GetRootCategories()
        {
            var categories = await _categoriesService.GetRootCategoriesAsync();
            return Ok(categories);
        }
        [HttpGet()]
        [Route("sub-categories/{parentId:int}")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<CategoryDto>>), 200)]
        public async Task<IActionResult> GetSubCategories(int parentId)
        {
            var categories = await _categoriesService.GetSubCategoriesAsync(parentId);
            return Ok(categories);
        }
        [HttpGet()]
        [ProducesResponseType(typeof(ApiResponse<PageResult<CategoryDto>>), 200)]
        public async Task<IActionResult> GetAllCategories(int page = 1, int pageSize = 10)
        {
            var categories = await _categoriesService.GetAllCategoriesAsync(page, pageSize);
            return Ok(categories);
        }

        [HttpGet()]
        [Route("{id:int}")]

        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoriesService.GetByIdAsync(id);
            return Ok(category);
        }

        [HttpPost()]
        [Authorize(Roles = "Admin")]
        [Route("create")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            var createdCategory = await _categoriesService.CreateAsync(dto);
            return Ok(createdCategory);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
        public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.ErrorResponse(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
            }
            var updatedCategory = await _categoriesService.UpdateAsync(id, dto);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoriesService.DeleteAsync(id);
            return Ok(result);
        }


    }
}