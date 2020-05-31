using SACA.Models;
using System.Threading.Tasks;

namespace SACA.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task CreateAsync(Image image, int? userId);
        Task<Image> GetAsync(int imageId);
        Task UpdateAsync(Image image);
        void Remove(Image image);
    }
}
