using AutoMapper;
using SACA.Models;
using SACA.Models.Dto;
using SACA.Models.Requests;
using SACA.Models.Responses;

namespace SACA
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<Image, ImageResponse>();
            CreateMap<ImageResponse, Image>();
            CreateMap<Image, ImageRequest>();
            CreateMap<ImageRequest, Image>();
            CreateMap<UserResponse, AuthenticationRequest>();
        }
    }
}
