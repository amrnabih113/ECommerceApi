using AutoMapper;
using ECommerce.DTOs;
using ECommerce.DTOs.Profile;
using ECommerce.Interfaces.Services;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            ICloudinaryService cloudinaryService,
            IMapper mapper)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProfileDto>> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<ProfileDto>.Error("User not found.");

            var profileDto = _mapper.Map<ProfileDto>(user);
            return ApiResponse<ProfileDto>.Success(profileDto, "Profile retrieved successfully.");
        }

        public async Task<ApiResponse<ProfileDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<ProfileDto>.Error("User not found.");

            var profileDto = _mapper.Map<ProfileDto>(user);
            return ApiResponse<ProfileDto>.Success(profileDto, "User retrieved successfully.");
        }

        public async Task<ApiResponse<ProfileDto>> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<ProfileDto>.Error("User not found.");

            // Update user properties
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.Address = dto.Address;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                // Check if phone number is already taken by another user
                var existingUserWithPhone = await _userManager.Users
                    .Where(u => u.PhoneNumber == dto.PhoneNumber && u.Id != userId)
                    .FirstOrDefaultAsync();

                if (existingUserWithPhone != null)
                    return ApiResponse<ProfileDto>.Error("Phone number is already in use.");

                user.PhoneNumber = dto.PhoneNumber;
                user.PhoneNumberConfirmed = false; // Reset confirmation when changed
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ApiResponse<ProfileDto>.Error(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );

            var profileDto = _mapper.Map<ProfileDto>(user);
            return ApiResponse<ProfileDto>.Success(profileDto, "Profile updated successfully.");
        }

        public async Task<ApiResponse> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse.ErrorResponse("User not found.");

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
                return ApiResponse.ErrorResponse(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );

            return ApiResponse.SuccessResponse("Password changed successfully.");
        }

        public async Task<ApiResponse<ProfileDto>> UploadProfileImageAsync(string userId, IFormFile file)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<ProfileDto>.Error("User not found.");

            // Delete old image if exists
            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                var publicId = _cloudinaryService.ExtractPublicIdFromUrl(user.ImageUrl);
                if (publicId != null)
                {
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }
            }

            // Upload new image
            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(file, "ecommerce/profiles");
                user.ImageUrl = imageUrl;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return ApiResponse<ProfileDto>.Error(
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );

                var profileDto = _mapper.Map<ProfileDto>(user);
                return ApiResponse<ProfileDto>.Success(profileDto, "Profile image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfileDto>.Error($"Failed to upload image: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteProfileImageAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse.ErrorResponse("User not found.");

            if (string.IsNullOrEmpty(user.ImageUrl))
                return ApiResponse.ErrorResponse("No profile image to delete.");

            // Delete image from Cloudinary
            var publicId = _cloudinaryService.ExtractPublicIdFromUrl(user.ImageUrl);
            if (publicId != null)
            {
                try
                {
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }
                catch (Exception ex)
                {
                    return ApiResponse.ErrorResponse($"Failed to delete image: {ex.Message}");
                }
            }

            // Remove image URL from user
            user.ImageUrl = null;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return ApiResponse.ErrorResponse(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );

            return ApiResponse.SuccessResponse("Profile image deleted successfully.");
        }

        public async Task<ApiResponse> DeleteAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse.ErrorResponse("User not found.");

            // Delete profile image if exists
            if (!string.IsNullOrEmpty(user.ImageUrl))
            {
                var publicId = _cloudinaryService.ExtractPublicIdFromUrl(user.ImageUrl);
                if (publicId != null)
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageAsync(publicId);
                    }
                    catch
                    {
                        // Continue with account deletion even if image deletion fails
                    }
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ApiResponse.ErrorResponse(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );

            return ApiResponse.SuccessResponse("Account deleted successfully.");
        }
    }
}
