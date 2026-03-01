using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.core.configs;
using ECommerce.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinaryConfig> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "ecommerce/products")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");

                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentException("PublicId is required.");

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
        }

        public string? ExtractPublicIdFromUrl(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return null;

            try
            {
                // Cloudinary URL format: https://res.cloudinary.com/{cloud-name}/image/upload/{transformations}/{type}/{folder}/{public-id}.{ext}
                var uri = new Uri(imageUrl);
                var pathSegments = uri.AbsolutePath.Split(new[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);

                if (pathSegments.Length > 0)
                {
                    var lastSegment = pathSegments[pathSegments.Length - 1];
                    var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(lastSegment);

                    // Find the index of 'upload' and get everything after it
                    var uploadIndex = Array.IndexOf(pathSegments, "upload");
                    if (uploadIndex >= 0 && uploadIndex < pathSegments.Length - 1)
                    {
                        // Reconstruct public ID from folder and filename
                        var parts = new List<string>();
                        for (int i = uploadIndex + 1; i < pathSegments.Length; i++)
                        {
                            if (i == pathSegments.Length - 1)
                            {
                                parts.Add(Path.GetFileNameWithoutExtension(pathSegments[i]));
                            }
                            else
                            {
                                parts.Add(pathSegments[i]);
                            }
                        }
                        return string.Join("/", parts);
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
