using SACA.Models;
using SACA.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> AuthenticateAsync(string email, string password, bool remember);
        Task<UserDto> Create(SignUpDto signUpDto);
        Task<UserDto> FindByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string role);
        Task<bool> IsInRole(User user, string role);
        Task Remove(User user);
    }
}
