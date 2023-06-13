using ImageMagick;

namespace SACA.Interfaces
{
    public interface IImageService
    {
        Task Resize(IFormFile file);
        Task Resize(IFormFile file, int width, int height);
        MagickImage Resize(MagickImage image);
        MagickImage Resize(MagickImage image, int width, int height);
        Task Compress(IFormFile file);
    }
}