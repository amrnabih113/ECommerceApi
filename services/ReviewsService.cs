using AutoMapper;
using ECommerce.core.Exceptions;
using ECommerce.DTOs;
using ECommerce.DTOs.Reviews;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;

namespace ECommerce.Services
{
    public class ReviewsService : IReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<PageResult<ReviewDto>>> GetByProductIdAsync(int productId, int page = 1, int pageSize = 10)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            var (reviews, totalItems) = await _unitOfWork.Reviews.GetByProductIdAsync(productId, page, pageSize);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDto>>(reviews);

            var pagedResult = new PageResult<ReviewDto>
            {
                Items = reviewDtos,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponse<PageResult<ReviewDto>>.SuccessResponse(pagedResult, "Reviews fetched successfully.");
        }

        public async Task<ApiResponse<ReviewDto>> CreateAsync(string userId, int productId, ReviewCreateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new BadRequestException("Product not found.");
            }

            if (!product.IsActive)
            {
                throw new BadRequestException("Cannot review an inactive product.");
            }

            var existingUserReview = await _unitOfWork.Reviews.GetByProductIdAndUserIdAsync(productId, userId);
            if (existingUserReview != null)
            {
                throw new BadRequestException("You have already reviewed this product.");
            }

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdReview = await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CompleteAsync();

            var loadedReview = await _unitOfWork.Reviews.GetByIdAsync(createdReview.Id);
            var reviewDto = _mapper.Map<ReviewDto>(loadedReview);
            return ApiResponse<ReviewDto>.SuccessResponse(reviewDto, "Review created successfully.");
        }

        public async Task<ApiResponse<ReviewDto>> UpdateAsync(string userId, int reviewId, ReviewUpdateDto dto)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new BadRequestException("Review not found.");
            }

            if (review.UserId != userId)
            {
                throw new UnauthorizedException("You can only update your own review.");
            }

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Reviews.UpdateAsync(review);
            await _unitOfWork.CompleteAsync();

            var updatedReview = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            var reviewDto = _mapper.Map<ReviewDto>(updatedReview);
            return ApiResponse<ReviewDto>.SuccessResponse(reviewDto, "Review updated successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(string userId, int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new BadRequestException("Review not found.");
            }

            if (review.UserId != userId)
            {
                throw new UnauthorizedException("You can only delete your own review.");
            }

            await _unitOfWork.Reviews.DeleteAsync(review);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Review deleted successfully.");
        }

        public async Task<ApiResponse> DeleteAsync(int reviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
            if (review == null)
            {
                throw new BadRequestException("Review not found.");
            }

            await _unitOfWork.Reviews.DeleteAsync(review);
            await _unitOfWork.CompleteAsync();
            return ApiResponse.SuccessResponse("Review deleted successfully.");
        }
    }
}