using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace AltinKasap.Web.Services;

public class ImageService : IImageService
{
    private const long MaxBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedMime = { "image/jpeg", "image/png", "image/webp" };
    private static readonly string[] AllowedExt = { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveResizedAsync(IFormFile file, string subfolder, int maxWidth = 800, int maxHeight = 600)
    {
        if (file == null || file.Length == 0)
            throw new InvalidOperationException("Dosya boş veya seçilmedi.");
        if (file.Length > MaxBytes)
            throw new InvalidOperationException("Dosya 5 MB sınırını aşıyor.");
        if (!AllowedMime.Contains(file.ContentType))
            throw new InvalidOperationException("Geçersiz dosya türü. Sadece JPEG, PNG ve WebP kabul edilir.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExt.Contains(ext))
            throw new InvalidOperationException("Geçersiz dosya uzantısı.");

        if (string.IsNullOrWhiteSpace(subfolder))
            throw new InvalidOperationException("Hedef klasör belirtilmedi.");
        var safeSubfolder = string.Join('-', subfolder.Split(Path.GetInvalidFileNameChars()));

        var folderPath = Path.Combine(_env.WebRootPath, "uploads", safeSubfolder);
        Directory.CreateDirectory(folderPath);

        var fileName = $"{Guid.NewGuid():N}.webp";
        var fullPath = Path.Combine(folderPath, fileName);

        await using var stream = file.OpenReadStream();
        using var image = await Image.LoadAsync(stream);
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(maxWidth, maxHeight)
        }));

        var encoder = new WebpEncoder { Quality = 80 };
        await image.SaveAsync(fullPath, encoder);

        return $"/uploads/{safeSubfolder}/{fileName}";
    }

    public void DeleteIfExists(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return;
        var trimmed = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_env.WebRootPath, trimmed);
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }
}
