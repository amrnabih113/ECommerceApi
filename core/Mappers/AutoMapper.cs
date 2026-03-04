using AutoMapper;
using ECommerce.DTOs.Brands;
using ECommerce.DTOs.Carts;
using ECommerce.DTOs.CartItems;
using ECommerce.DTOs.Categories;
using ECommerce.DTOs.Coupons;
using ECommerce.DTOs.Orders;
using ECommerce.DTOs.Payments;
using ECommerce.DTOs.Products;
using ECommerce.DTOs.Profile;
using ECommerce.DTOs.Reviews;
using ECommerce.DTOs.WishLists;
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

        // WishList mappings
        CreateMap<WishList, WishListItemDto>()
            .ForMember(dest => dest.Product,
                opt => opt.MapFrom(src => src.Product));

        // Profile mappings
        CreateMap<ApplicationUser, ProfileDto>();
        CreateMap<UpdateProfileDto, ApplicationUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.Coupon, opt => opt.MapFrom(src => src.Coupon))
            .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null && src.Product.Images.Any()
                ? src.Product.Images.FirstOrDefault(img => img.IsMain)!.ImageUrl
                : null))
            .ForMember(dest => dest.VariantSize, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Size : null))
            .ForMember(dest => dest.VariantColor, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Color : null));

        // Address mappings
        CreateMap<Address, AddressDto>();

        // Coupon mappings
        CreateMap<Coupon, CouponDto>();
        CreateMap<CreateCouponDto, Coupon>();
        CreateMap<UpdateCouponDto, Coupon>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Payment mappings
        CreateMap<Payment, PaymentDto>();
    }
}