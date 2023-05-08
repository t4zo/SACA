using ImageMagick;
using SACA.Interfaces;

namespace SACA.Services
{
    public class ImageService : IImageService
    {
        public MagickImage Resize(MagickImage image)
        {
            // image.Resize(110, 150);
            image.Resize(new MagickGeometry(110, 150)
            {
                IgnoreAspectRatio = false,
            });

            return image;
        }
        
        public MagickImage Resize(MagickImage image, int width = 110, int height = 150)
        {
            // image.Resize(width, height);
            image.Resize(new MagickGeometry(width, height)
            {
                IgnoreAspectRatio = false,
            });

            return image;
        }
    }
}