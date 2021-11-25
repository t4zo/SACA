using SACA.Entities;
using SACA.Entities.Responses;

namespace SACA.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task<Image> GetAsync(int imageId);
        Task AddAsync(Image image);
        void Remove(Image image);
        Task<ImageResponse> GetUserImageAsync(int userId);
        Task<ImageResponse> GetUserImageAsync(int userId, int imageId);
    }
}
