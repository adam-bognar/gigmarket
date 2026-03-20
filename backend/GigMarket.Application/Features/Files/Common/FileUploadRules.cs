namespace GigMarket.Application.Features.Files.Common;

public static class FileUploadRules
{
    public static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png"];
    public static readonly string[] AllowedVideoExtensions = [".mp4", ".webm"];

    public const long MaxImageSize = 5 * 1024 * 1024;
    public const long MaxVideoSize = 100 * 1024 * 1024;

    public static bool IsImageExtension(string extension)
        => AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);

    public static bool IsVideoExtension(string extension)
        => AllowedVideoExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
}

