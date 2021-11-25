using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Entities.Identity;
using SACA.Entities.Responses;
using SACA.Repositories.Interfaces;

namespace SACA.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserResponse>> GetUsersAsync()
        {
            return await _context.Users.ProjectTo<UserResponse>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ApplicationUser> GetUserCategoryAsync(int userId)
        {
            return await _context.Users
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<ApplicationUser> GetUserAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
