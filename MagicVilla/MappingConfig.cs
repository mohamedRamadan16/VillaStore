using AutoMapper;
using MagicVilla.Models;
using MagicVilla.Models.DTOs;

namespace MagicVilla
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            // this maps the basic same name property :) , if you  want to customize or you changed the names you can make using another method ( search for it :-D )

            CreateMap<Villa, VillaDTO>();
            CreateMap<VillaDTO, Villa>();

            CreateMap<Villa, VillaCreateDTO>().ReverseMap();

            CreateMap<VillaUpdateDTO, Villa>().ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
            CreateMap<Villa, VillaUpdateDTO>();

            CreateMap<VillaNumber, VillaNumberDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberCreatedDTO>().ReverseMap();
            CreateMap<VillaNumber, VillaNumberUpdatedDTO>().ReverseMap();

        }
    }
}
