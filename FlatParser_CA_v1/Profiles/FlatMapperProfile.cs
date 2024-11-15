using AutoMapper;
using DataAccess.Entity;
using FlatParser_CA_v1.Models;

namespace FlatParser_CA_v1.Profiles
{
    public class FlatMapperProfile : Profile
    {
        public FlatMapperProfile()
        {
            CreateMap<FlatInfo, FlatEntity>().ReverseMap();
        }
    }
}
