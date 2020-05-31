using SACA.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
