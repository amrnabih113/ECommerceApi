using AutoMapper;
using ECommerce.DTOs.Brands;
using ECommerce.DTOs.Carts;
using ECommerce.DTOs.CartItems;
using ECommerce.DTOs.Categories;
using ECommerce.DTOs.Products;
using ECommerce.DTOs.Reviews;
using ECommerce.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>()
      .ForMember(
          dest => dest.CategoryName,
          opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty)
      )
      .ForMember(
          dest => dest.BrandName,
          opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : string.Empty)
      )
      .ForMember(dest => dest.ImageUrl,
          opt => opt.MapFrom(src => src.Images
                    .Where(pi => pi.IsMain)
                    .Select(pi => pi.ImageUrl)
                    .FirstOrDefault() ?? string.Empty)
      );
        CreateMap<Product, ProductDetailsDto>()
      .ForMember(
          dest => dest.CategoryName,
          opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty)
        )
      .ForMember(
          dest => dest.BrandName,
          opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : string.Empty)
        )
      .ForMember(dest => dest.Images,
          opt => opt.MapFrom(src => src.Images))
        .ForMember(dest => dest.Variants,
                    opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.Reviews,
                    opt => opt.MapFrom(src => src.Reviews
                        .OrderByDescending(r => r.Rating)
                        .ThenByDescending(r => r.CreatedAt)
                        .Take(5)))
                .ForMember(dest => dest.ReviewsCount,
                    opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.Reviews.Any() ? Math.Round(src.Reviews.Average(r => (double)r.Rating), 2) : 0));
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();

        CreateMap<ProductImage, ProductImageDto>()
            .ForMember(dest => dest.Url,
            opt => opt.MapFrom(src => src.ImageUrl));


        // Review mappings
        CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? (src.User.FullName ?? src.User.UserName ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.UserImageUrl,
                opt => opt.MapFrom(src => src.User != null ? src.User.ImageUrl : null));
        CreateMap<ReviewCreateDto, Review>();
        // ProductVariant mappings
        CreateMap<ProductVariant, ProductVariantDto>();
        CreateMap<ProductVariantCreateDto, ProductVariant>();
        CreateMap<ProductVariantUpdateDto, ProductVariant>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Category mappings
        CreateMap<Category, CategoryDto>().ForMember(dest => dest.ProductsCount,
        opt => opt.MapFrom(src =>
            src.Products.Count(p => p.IsActive)
        ));
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();

        // Brand mappings
        CreateMap<Brand, BrandDto>().ForMember(dest => dest.ProductsCount,
        opt => opt.MapFrom(src =>
            src.Products.Count(p => p.IsActive)
        ));
        CreateMap<BrandCreateDto, Brand>();
        CreateMap<BrandUpdateDto, Brand>();

        // Cart mappings
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));


        // CartItem mappings
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.VariantSize, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Size : null))
            .ForMember(dest => dest.VariantColor, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Color : null))
            .ForMember(dest => dest.AdditionalPrice, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.AdditionalPrice : null))
            .ForMember(dest => dest.VariantImageUrl, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.ImageUrl : null));
        CreateMap<CartItemCreateDto, CartItem>();
        CreateMap<CartItemUpdateDto, CartItem>();
    }
}