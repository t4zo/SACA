using System;
using System.Threading.Tasks;
using OneOf;
using SACA.Models.Requests;
using SACA.Models.Responses;

namespace SACA.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<UserResponse, ArgumentException>> SignInAsync(string email, string password, bool remember = false);
        Task<UserResponse> CreateAsync(SignUpRequest signUpDto);
    }
}