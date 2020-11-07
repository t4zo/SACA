﻿using AutoMapper;
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
            CreateMap<Image, ImageResponse>().ReverseMap();
            CreateMap<Image, ImageRequest>().ReverseMap();
            CreateMap<User, UserResponse>();
            CreateMap<UserResponse, AuthenticationRequest>();
        }
    }
}
