using AutoMapper;
using Services.DataProcessAPI.Models;
using WebUI.Models;

namespace Services.DataProcessAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<UserDto, User>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
