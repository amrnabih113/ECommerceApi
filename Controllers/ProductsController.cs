using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.core.utils;
using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize(Roles = "Admin,User")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));
            var response = await _productsService.GetAllAsync(page, pageSize);
            return Ok(response);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var response = await _productsService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productsService.CreateAsync(dto);
            return Ok(response);
        }

        [HttpPut("{id}")]

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productsService.UpdateAsync(id, dto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productsService.DeleteAsync(id);
            return Ok(response);
        }

        [HttpGet("by-brand/{brandId}")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetByBrand(int brandId, int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));
            var response = await _productsService.GetByBrandAsync(brandId, page, pageSize);
            return Ok(response);
        }

        [HttpGet("by-category/{categoryId}")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetByCategory(int categoryId, int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));
            var response = await _productsService.GetByCategoryAsync(categoryId, page, pageSize);
            return Ok(response);
        }
    }
}