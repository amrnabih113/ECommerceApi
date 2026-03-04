using ECommerce.DTOs;
using ECommerce.DTOs.Orders;
using ECommerce.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{

    [ApiController]
    [Route("api/addresses")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressesService _addressesService;

        public AddressesController(IAddressesService addressesService)
        {
            _addressesService = addressesService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<AddressDto>>), 200)]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _addressesService.GetMyAddressesAsync(userId);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AddressDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _addressesService.GetByIdAsync(id, userId);
            if (!((ApiResponse)response).Success)
                return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// Create a new address
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AddressDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _addressesService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = response.Data?.Id }, response);
        }

        /// <summary>
        /// Update an existing address
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AddressDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse.ErrorResponse(
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                ));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _addressesService.UpdateAsync(id, dto, userId);
            if (!((ApiResponse)response).Success)
                return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// Delete an address
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse), 204)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.ErrorResponse("User not authenticated."));

            var response = await _addressesService.DeleteAsync(id, userId);
            if (!response.Success)
                return NotFound(response);

            return NoContent();
        }
    }
}
