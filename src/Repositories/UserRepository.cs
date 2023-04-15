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
        private readonly MapperlyMapper _mapper;

        public UserRepository(ApplicationDbContext context, MapperlyMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserResponse>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.MapToUsersResponse(users).ToList();
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
