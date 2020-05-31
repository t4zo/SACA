using ImageMagick;
using SACA.Models;
using SACA.Models.Dto;
using System.Threading.Tasks;

namespace SACA.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadToCloudinaryAsync(ImageDto model, int? userId);
        Task RemoveFolderFromCloudinaryAsync(int userId);
        Task<bool> RemoveImageFromCloudinaryAsync(Image model, User user);
        MagickImage Resize(MagickImage image, int width, int height);
    }
}
