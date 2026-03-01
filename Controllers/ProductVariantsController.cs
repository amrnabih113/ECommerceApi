using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/products/{productId:int}/variants")]
    [Authorize(Roles = "Admin")]
    public class ProductVariantsController : ControllerBase
    {
        private readonly IProductVariantsService _productVariantsService;

        public ProductVariantsController(IProductVariantsService productVariantsService)
        {
            _productVariantsService = productVariantsService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductVariantDto>>), 200)]
        public async Task<IActionResult> GetByProduct([FromRoute] int productId, int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            var response = await _productVariantsService.GetByProductIdAsync(productId, page, pageSize);
            return Ok(response);
        }

        [HttpGet("{variantId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ProductVariantDto>), 200)]
        public async Task<IActionResult> GetById([FromRoute] int productId, [FromRoute] int variantId)
        {
            var response = await _productVariantsService.GetByIdAsync(productId, variantId);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductVariantDto>), 200)]
        public async Task<IActionResult> Create([FromRoute] int productId, [FromBody] ProductVariantCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productVariantsService.CreateAsync(productId, dto);
            return Ok(response);
        }

        [HttpPut("{variantId:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<ProductVariantDto>), 200)]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromRoute] int variantId, [FromBody] ProductVariantUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productVariantsService.UpdateAsync(productId, variantId, dto);
            return Ok(response);
        }

        [HttpDelete("{variantId:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> Delete([FromRoute] int productId, [FromRoute] int variantId)
        {
            var response = await _productVariantsService.DeleteAsync(productId, variantId);
            return Ok(response);
        }
    }
}
