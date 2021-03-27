using AutoMapper;
using SACA.Entities;
using SACA.Entities.Identity;
using SACA.Entities.Requests;
using SACA.Entities.Responses;

namespace SACA
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Image, ImageResponse>().ReverseMap();
            CreateMap<Image, ImageRequest>().ReverseMap();
            CreateMap<ApplicationUser, UserResponse>().ReverseMap();
            CreateMap<UserResponse, SignInRequest>().ReverseMap();
        }
    }
}