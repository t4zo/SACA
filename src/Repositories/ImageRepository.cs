using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public ImageRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Image> GetAsync(int imageId)
        {
            return await _context.Images.FindAsync(imageId);
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
            return await _context.Images
                .AsNoTracking()
                .Include(x => x.User.Id == userId)
                .Where(x => x.UserId == userId)
                .ProjectTo<ImageResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<ImageResponse> GetUserImageAsync(int userId, int imageId)
        {
            return await _context.Images
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.Id == imageId && x.UserId == userId)
                .ProjectTo<ImageResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }
    }
}
