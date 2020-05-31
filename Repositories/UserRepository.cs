using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SACA.Models;
using SACA.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        async Task<User> IUserRepository.GetAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        async Task<IEnumerable<User>> IUserRepository.GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}
