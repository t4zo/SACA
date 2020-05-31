using AutoMapper;
using SACA.Models;
using SACA.Models.Dto;

namespace SACA.Utilities
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<User, UserDto>();
            CreateMap<Image, ImageDto>();
            CreateMap<ImageDto, Image>();
            CreateMap<UserDto, AuthenticationDto>();
        }
    }
}
