using ImageMagick;
using SACA.Interfaces;

namespace SACA.Services
{
    public class ImageService : IImageService
    {
        public MagickImage Resize(MagickImage image, int width, int height)
        {
            var size = new MagickGeometry(width, height)
            {
                IgnoreAspectRatio = true
            };

            image.Resize(size);

            return image;
        }
    }
}