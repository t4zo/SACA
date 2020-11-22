using System.Threading.Tasks;
using ImageMagick;
using SACA.Models;
using SACA.Models.Requests;

namespace SACA.Interfaces
{
    public interface IImageService
    {
        Task<(string FullyQualifiedPublicId, string PublicId)> UploadToCloudinaryAsync(ImageRequest model);
        Task<(string FullyQualifiedPublicId, string PublicId)> UploadToCloudinaryAsync(ImageRequest model, int userId);
        Task RemoveFolderFromCloudinaryAsync(int userId);
        Task<bool> RemoveImageFromCloudinaryAsync(Image model);
        MagickImage Resize(MagickImage image, int width, int height);
    }
}