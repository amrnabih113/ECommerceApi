using AutoMapper;
using ECommerce.DTOs.Brands;
using ECommerce.DTOs.Carts;
using ECommerce.DTOs.CartItems;
using ECommerce.DTOs.Categories;
using ECommerce.DTOs.Products;
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
          opt => opt.MapFrom(src => src.Variants));
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();

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
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null ? src.ProductVariant.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.ProductId : 0))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null ? src.ProductVariant.Product.Price : 0))
            .ForMember(dest => dest.VariantSize, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Size : string.Empty))
            .ForMember(dest => dest.VariantColor, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.Color : string.Empty))
            .ForMember(dest => dest.AdditionalPrice, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.AdditionalPrice : 0))
            .ForMember(dest => dest.VariantImageUrl, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.ImageUrl : string.Empty));
        CreateMap<CartItemCreateDto, CartItem>();
        CreateMap<CartItemUpdateDto, CartItem>();
    }
}