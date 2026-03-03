using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.DTOs;
using ECommerce.DTOs.Brands;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/brands")]
    [Authorize(Roles = "Admin,User")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandsService _brandsService;
        public BrandsController(IBrandsService brandsService)
        {
            _brandsService = brandsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PageResult<BrandDto>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] BrandQueryDto query)
        {
            if (query.Page < 1 || query.PageSize < 1)
                return BadRequest(ApiResponse.ErrorResponse("Page and pageSize must be greater than 0."));

            if (query.MinProductsCount.HasValue && query.MinProductsCount.Value < 0)
                return BadRequest(ApiResponse.ErrorResponse("minProductsCount cannot be negative."));

            var response = await _brandsService.GetAllAsync(query);
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<BrandDto>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _brandsService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<BrandDto>), 200)]
        public async Task<IActionResult> Create(BrandCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _brandsService.CreateAsync(dto);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<BrandDto>), 200)]
        public async Task<IActionResult> Update(int id, BrandUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));

            var response = await _brandsService.UpdateAsync(id, dto);
            return Ok(response);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            var response = await _brandsService.DeleteAsync(id);
            return Ok(response);
        }
    }
}