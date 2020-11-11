using ImageMagick;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Models.Identity;
using System.Threading.Tasks;

namespace SACA.Interfaces
{
    public interface IImageService
    {
        Task<(string FullyQualifiedPublicId, string PublicId)> UploadToCloudinaryAsync(ImageRequest model, int? userId);
        Task RemoveFolderFromCloudinaryAsync(int userId);
        Task<bool> RemoveImageFromCloudinaryAsync(Image model, ApplicationUser user);
        MagickImage Resize(MagickImage image, int width, int height);
    }
}
