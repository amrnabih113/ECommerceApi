using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Products;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class ProductImagesService : IProductImagesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public ProductImagesService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        private void ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadRequestException("File is required.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new BadRequestException($"File type not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
            }
        }

        public async Task<ApiResponse<PageResult<ProductImageDto>>> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var (images, totalItems) = await _unitOfWork.ProductImages.GetByProductIdAsync(productId, page, pageSize);
            var imageDtos = _mapper.Map<IEnumerable<ProductImageDto>>(images);

            var pagedResult = new PageResult<ProductImageDto>
            {
                Items = imageDtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<ProductImageDto>>.SuccessResponse(pagedResult, "Product images fetched successfully.");
        }

        public async Task<ApiResponse<ProductImageDto>> GetByIdAsync(int productId, int imageId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var image = await _unitOfWork.ProductImages.GetByProductIdAndImageIdAsync(productId, imageId);
            if (image == null)
            {
                throw new BadRequestException("Product image not found.");
            }

            var imageDto = _mapper.Map<ProductImageDto>(image);
            return ApiResponse<ProductImageDto>.SuccessResponse(imageDto, "Product image fetched successfully.");
        }

        public async Task<ApiResponse<ProductImageDto>> UploadAsync(int productId, ProductImageUploadDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            ValidateImageFile(dto.File);

            var cloudinaryUrl = await _cloudinaryService.UploadImageAsync(dto.File);

            var currentImages = (await _unitOfWork.ProductImages.GetAllByProductIdAsync(productId)).ToList();
            var shouldBeMain = dto.IsMain || !currentImages.Any();

            if (shouldBeMain)
            {
                foreach (var existingImage in currentImages.Where(i => i.IsMain))
                {
                    existingImage.IsMain = false;
                    existingImage.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.ProductImages.UpdateAsync(existingImage);
                }
            }

            var newImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = cloudinaryUrl,
                IsMain = shouldBeMain,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdImage = await _unitOfWork.ProductImages.AddAsync(newImage);
            await _unitOfWork.CompleteAsync();

            var imageDto = _mapper.Map<ProductImageDto>(createdImage);
            return ApiResponse<ProductImageDto>.SuccessResponse(imageDto, "Product image uploaded successfully.");
        }

        public async Task<ApiResponse<ProductImageDto>> UpdateAsync(int productId, int imageId, ProductImageUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var image = await _unitOfWork.ProductImages.GetByProductIdAndImageIdAsync(productId, imageId);
            if (image == null)
            {
                throw new BadRequestException("Product image not found.");
            }

            ValidateImageFile(dto.File);

            // Upload new image to Cloudinary
            var newCloudinaryUrl = await _cloudinaryService.UploadImageAsync(dto.File);

            // Delete old image from Cloudinary
            var oldPublicId = _cloudinaryService.ExtractPublicIdFromUrl(image.ImageUrl);
            if (!string.IsNullOrEmpty(oldPublicId))
            {
                await _cloudinaryService.DeleteImageAsync(oldPublicId);
            }

            var allImages = (await _unitOfWork.ProductImages.GetAllByProductIdAsync(productId)).ToList();

            // Handle main image assignment
            if (dto.IsMain)
            {
                foreach (var otherImage in allImages.Where(i => i.Id != imageId && i.IsMain))
                {
                    otherImage.IsMain = false;
                    otherImage.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.ProductImages.UpdateAsync(otherImage);
                }
            }
            else if (image.IsMain && allImages.Count == 1)
            {
                throw new BadRequestException("The only product image must remain main.");
            }
            else if (image.IsMain)
            {
                var anotherImage = allImages.FirstOrDefault(i => i.Id != imageId);
                if (anotherImage != null)
                {
                    anotherImage.IsMain = true;
                    anotherImage.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.ProductImages.UpdateAsync(anotherImage);
                }
            }

            image.ImageUrl = newCloudinaryUrl;
            image.IsMain = dto.IsMain;
            image.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.ProductImages.UpdateAsync(image);
            await _unitOfWork.CompleteAsync();

            var imageDto = _mapper.Map<ProductImageDto>(image);
            return ApiResponse<ProductImageDto>.SuccessResponse(imageDto, "Product image updated successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int productId, int imageId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var image = await _unitOfWork.ProductImages.GetByProductIdAndImageIdAsync(productId, imageId);
            if (image == null)
            {
                throw new BadRequestException("Product image not found.");
            }

            var allImages = (await _unitOfWork.ProductImages.GetAllByProductIdAsync(productId)).ToList();

            // Delete from Cloudinary first
            var publicId = _cloudinaryService.ExtractPublicIdFromUrl(image.ImageUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                try
                {
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }
                catch (Exception ex)
                {
                    throw new BadRequestException($"Failed to delete image from Cloudinary: {ex.Message}");
                }
            }

            // Delete from database
            await _unitOfWork.ProductImages.DeleteAsync(image);

            // Update main image if needed
            if (image.IsMain && allImages.Count > 1)
            {
                var remainingImages = allImages.Where(i => i.Id != imageId).ToList();
                var newMain = remainingImages.First();
                newMain.IsMain = true;
                newMain.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.ProductImages.UpdateAsync(newMain);
            }

            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Product image deleted successfully.");
        }
    }
}