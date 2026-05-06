namespace AltinKasap.Web.Services;

public interface IImageService
{
    Task<string> SaveResizedAsync(IFormFile file, string subfolder, int maxWidth = 800, int maxHeight = 600);
    void DeleteIfExists(string? relativePath);
}
