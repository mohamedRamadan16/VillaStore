using AutoMapper;
using Magic_Villa_MVC.Models;
using Magic_Villa_MVC.Models.DTOs;

namespace Magic_Villa_MVC
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            // this maps the basic same name property :) , if you  want to customize or you changed the names you can make using another method ( search for it :-D )

            CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
            CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();

            CreateMap<VillaNumberDTO, VillaNumberCreatedDTO>().ReverseMap();
            CreateMap<VillaNumberDTO, VillaNumberUpdatedDTO>().ReverseMap();

        }
    }
}
