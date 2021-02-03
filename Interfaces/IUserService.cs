using OneOf;
using SACA.Models.Identity;
using SACA.Models.Requests;
using SACA.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SACA.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<UserResponse, ArgumentException>> LogInAsync(string email, string password, bool remember = false);
        Task<UserResponse> SignInAsync(SignUpRequest signUpDto);
        Task DeleteAsync(ApplicationUser user);
        Task<ICollection<string>> GetRolesAsync(ApplicationUser user);
    }
}