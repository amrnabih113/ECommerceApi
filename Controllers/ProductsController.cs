using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize(Roles = "Admin,User")]
    [EnableRateLimiting("general")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
        {
            if (query.Page < 1 || query.PageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
                return BadRequest(ApiResponse.ErrorResponse("minPrice cannot be greater than maxPrice."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _productsService.GetAllAsync(query, userId);
            return Ok(response);
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailsDto>), 200)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _productsService.GetByIdAsync(id, userId);
            return Ok(response);
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productsService.CreateAsync(dto);
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailsDto>), 200)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productsService.UpdateAsync(id, dto);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productsService.DeleteAsync(id);
            return Ok(response);
        }

        [HttpGet("by-brand/{brandId:int}")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetByBrand(int brandId, [FromQuery] ProductQueryDto query)
        {
            if (query.Page < 1 || query.PageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
                return BadRequest(ApiResponse.ErrorResponse("minPrice cannot be greater than maxPrice."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _productsService.GetByBrandAsync(brandId, query, userId);
            return Ok(response);
        }

        [HttpGet("by-category/{categoryId:int}")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] ProductQueryDto query)
        {
            if (query.Page < 1 || query.PageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
                return BadRequest(ApiResponse.ErrorResponse("minPrice cannot be greater than maxPrice."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _productsService.GetByCategoryAsync(categoryId, query, userId);
            return Ok(response);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Search([FromQuery] string term, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(ApiResponse.ErrorResponse("Search term is required."));

            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _productsService.SearchAsync(term, page, pageSize, userId);
            return Ok(response);
        }

        [HttpGet("search/recommendations")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetSearchRecommendations([FromQuery] string term, int size = 5)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(ApiResponse.ErrorResponse("Search term is required."));

            if (size < 1)
                return BadRequest(ApiResponse.ErrorResponse("size must be greater than 0."));

            var response = await _productsService.GetSearchRecommendationsAsync(term, size);
            return Ok(response);
        }
        [HttpGet("sales")]
        
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetSalesProducts([FromQuery] ProductQueryDto query)
        {
            if (query.Page < 1 || query.PageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
                return BadRequest(ApiResponse.ErrorResponse("minPrice cannot be greater than maxPrice."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _productsService.GetSalesProductsAsync(query, userId);
            return Ok(response);
        }

     
        [HttpGet("best-sales")]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> GetBestSalesProducts([FromQuery] ProductQueryDto query)
        {
            if (query.Page < 1 || query.PageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            if (query.MinPrice.HasValue && query.MaxPrice.HasValue && query.MinPrice > query.MaxPrice)
                return BadRequest(ApiResponse.ErrorResponse("minPrice cannot be greater than maxPrice."));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _productsService.GetBestSalesProductsAsync(query, userId);
            return Ok(response);
        }
    }
}