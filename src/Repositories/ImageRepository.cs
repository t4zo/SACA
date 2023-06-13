using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Entities;
using SACA.Entities.Responses;
using SACA.Repositories.Interfaces;

namespace SACA.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly MapperlyMapper _mapper;

        public ImageRepository(ApplicationDbContext context, MapperlyMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Image> GetAsync(int imageId)
        {
            return await _context.Images.FirstOrDefaultAsync(i => i.Id == imageId);
        }
        
        public async Task<Image> GetAsync(int userId, int imageId)
        {
            var image = await _context.Images
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.Id == imageId)
                .FirstOrDefaultAsync();

            return image;
        }

        public async Task AddAsync(Image image)
        {
            await _context.Images.AddAsync(image);
        }

        public void Remove(Image image)
        {
            _context.Images.Remove(image);
        }

        public async Task<ImageResponse> GetUserImageAsync(int userId)
        {
            var image = await _context.Images
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            return _mapper.MapToImageResponse(image);
        }

        public async Task<ImageResponse> GetUserImageAsync(int userId, int imageId)
        {
            var image = await _context.Images
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.Id == imageId)
                .FirstOrDefaultAsync();

            return _mapper.MapToImageResponse(image);
        }
    }
}
