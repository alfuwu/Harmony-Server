namespace Server.Helpers;
public static class FileHelper {
    private static readonly string[] ALLOWED_IMAGE_TYPES = ["image/png", "image/jpeg", "image/webp", "image/gif"];

    public static async Task UploadImage(IFormFile file, string path) {
        if (!ALLOWED_IMAGE_TYPES.Contains(file.ContentType))
            throw new Exception("Unsupported file type");

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext))
            ext = ".png"; // default fallback

        var filePath = path + ext;

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
    }
}
