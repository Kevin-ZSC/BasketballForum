using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        var settings = config.GetSection("CloudinarySettings");
        var account = new Account(
            settings["CloudName"],
            settings["ApiKey"],
            settings["ApiSecret"]
        );

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return result.SecureUrl.ToString(); // You can also store result.PublicId
    }
}
