using ImageMagick;
using SACA.Interfaces;

namespace SACA.Services
{
    public class ImageService : IImageService
    {
        public async Task Resize(IFormFile file)
        {
            await using var input = file.OpenReadStream();
            using var image = new MagickImage(input);

            image.Resize(110, 150);
        }
        
        public async Task Resize(IFormFile file, int width, int height)
        {
            await using var input = file.OpenReadStream();
            using var image = new MagickImage(input);

            image.Resize(width, height);
        }

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

        public async Task Compress(IFormFile file)
        {
            await using var input = file.OpenReadStream();
            
            var optimizer = new ImageOptimizer
            {
                IgnoreUnsupportedFormats = true,
            };

            optimizer.Compress(input);
        }
    }
}