using AutoMapper;
using Lunz.ProductCenter.ApiService.Property.Api;

namespace Lunz.ProductCenter.ApiService.Api.Property
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // INFO: 在这里配置更多的 Mapping 关系。
            CreateMap<QueryStack.Models.Property, PropertyDetails>();
        }
    }
}
