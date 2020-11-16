using OneOf;
using SACA.Models.Requests;
using SACA.Models.Responses;
using System;
using System.Threading.Tasks;

namespace SACA.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<UserResponse, ArgumentException>> SignInAsync(string email, string password, bool remember = false);
        Task<UserResponse> CreateAsync(SignUpRequest signUpDto);
    }
}
