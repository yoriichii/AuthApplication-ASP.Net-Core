using AutoMapper;
using AuthApplication.Dto;
using AuthApplication.Models;

namespace AuthApplication.MapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterDto, User>();
        }
    }
}


