using AutoMapper;
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
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();

    }
}