using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/products/{productId:int}/images")]
    [Authorize(Roles = "Admin")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImagesService _productImagesService;

        public ProductImagesController(IProductImagesService productImagesService)
        {
            _productImagesService = productImagesService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PageResult<ProductImageDto>>), 200)]
        public async Task<IActionResult> GetByProduct([FromRoute] int productId, int page = 1, int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            var response = await _productImagesService.GetByProductIdAsync(productId, page, pageSize);
            return Ok(response);
        }

        [HttpGet("{imageId:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), 200)]
        public async Task<IActionResult> GetById([FromRoute] int productId, [FromRoute] int imageId)
        {
            var response = await _productImagesService.GetByIdAsync(productId, imageId);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), 200)]
        public async Task<IActionResult> Upload([FromRoute] int productId, [FromForm] ProductImageUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productImagesService.UploadAsync(productId, dto);
            return Ok(response);
        }

        [HttpPut("{imageId:int}")]
        [ProducesResponseType(typeof(ApiResponse<ProductImageDto>), 200)]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromRoute] int imageId, [FromForm] ProductImageUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _productImagesService.UpdateAsync(productId, imageId, dto);
            return Ok(response);
        }

        [HttpDelete("{imageId:int}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        public async Task<IActionResult> Delete([FromRoute] int productId, [FromRoute] int imageId)
        {
            var response = await _productImagesService.DeleteAsync(productId, imageId);
            return Ok(response);
        }
    }
}