using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Models;
using SACA.Repositories.Interfaces;
using System.Threading.Tasks;

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

        async Task IImageRepository.CreateAsync(Image image, int? userId)
        {
            if (userId.HasValue)
            {
                image.UserId = userId.Value;
            }

            await _context.Images.AddAsync(image);
        }

        async Task<Image> IImageRepository.GetAsync(int imageId)
        {
            return await _context.Images.FirstOrDefaultAsync(c => c.Id == imageId);
        }

        async Task IImageRepository.UpdateAsync(Image image)
        {
            var _image = await _context.Images.FirstOrDefaultAsync(i => i.Id == image.Id);

            _image.Name = image.Name;
            _image.Url = image.Url;

            _context.Entry(image).State = EntityState.Modified;
        }

        public void Remove(Image image)
        {
            _context.Remove(image);
        }
    }
}
