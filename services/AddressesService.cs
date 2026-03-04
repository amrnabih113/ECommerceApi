using AutoMapper;
using ECommerce.DTOs;
using ECommerce.DTOs.Orders;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class AddressesService : IAddressesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddressesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<AddressDto>>> GetMyAddressesAsync(string userId)
        {
            var addresses = await _unitOfWork.Addresses.GetByUserIdAsync(userId);

            var addressDtos = _mapper.Map<List<AddressDto>>(addresses);
            return ApiResponse<List<AddressDto>>.SuccessResponse(addressDtos);
        }

        public async Task<ApiResponse<AddressDto>> GetByIdAsync(int id, string userId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAndUserIdAsync(id, userId);

            if (address == null)
                return ApiResponse<AddressDto>.Error("Address not found.");

            var addressDto = _mapper.Map<AddressDto>(address);
            return ApiResponse<AddressDto>.SuccessResponse(addressDto);
        }

        public async Task<ApiResponse<AddressDto>> CreateAsync(CreateAddressDto dto, string userId)
        {
            var address = new Address
            {
                UserId = userId,
                Country = dto.Country,
                City = dto.City,
                Street = dto.Street,
                PostalCode = dto.PostalCode
            };

            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.CompleteAsync();

            var addressDto = _mapper.Map<AddressDto>(address);
            return ApiResponse<AddressDto>.SuccessResponse(addressDto, "Address created successfully.");
        }

        public async Task<ApiResponse<AddressDto>> UpdateAsync(int id, UpdateAddressDto dto, string userId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAndUserIdAsync(id, userId);

            if (address == null)
                return ApiResponse<AddressDto>.Error("Address not found.");

            address.Country = dto.Country;
            address.City = dto.City;
            address.Street = dto.Street;
            address.PostalCode = dto.PostalCode;

            await _unitOfWork.Addresses.UpdateAsync(address);
            await _unitOfWork.CompleteAsync();

            var addressDto = _mapper.Map<AddressDto>(address);
            return ApiResponse<AddressDto>.SuccessResponse(addressDto, "Address updated successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int id, string userId)
        {
            var address = await _unitOfWork.Addresses.GetByIdAndUserIdAsync(id, userId);

            if (address == null)
                return ApiResponse.ErrorResponse("Address not found.");

            await _unitOfWork.Addresses.DeleteAsync(address);
            await _unitOfWork.CompleteAsync();

            return ApiResponse.SuccessResponse("Address deleted successfully.");
        }
    }
}
