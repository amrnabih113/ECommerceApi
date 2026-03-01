namespace ECommerce.Interfaces.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "ecommerce/products");
        Task<bool> DeleteImageAsync(string publicId);
        string? ExtractPublicIdFromUrl(string? imageUrl);
    }
}
