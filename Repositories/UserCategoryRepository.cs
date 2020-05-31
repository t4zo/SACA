using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Models;
using SACA.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Repositories
{
    public class UserCategoryRepository : IUserCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public UserCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        async Task IUserCategoryRepository.CreateAsync(UserCategory userCategory)
        {
            await _context.UserCategories.AddAsync(userCategory);
        }

        async Task IUserCategoryRepository.CreateForAllAsync(IEnumerable<User> users, int categoryId)
        {
            foreach (var user in users)
            {
                await _context.UserCategories.AddAsync(new UserCategory { UserId = user.Id, CategoryId = categoryId });
            }
        }

        async Task<bool> IUserCategoryRepository.ExistsAsync(UserCategory userCategory)
        {
            return await _context.UserCategories.AnyAsync(uc => uc.CategoryId == userCategory.CategoryId && uc.UserId == userCategory.UserId);
        }
    }
}
