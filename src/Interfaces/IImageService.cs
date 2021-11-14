using ImageMagick;

namespace SACA.Interfaces
{
    public interface IImageService
    {
        MagickImage Resize(MagickImage image, int width, int height);
    }
}