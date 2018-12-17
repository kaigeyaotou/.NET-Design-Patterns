using AutoMapper;

namespace Lunz.ProductCenter.ApiService.Api.HearFroms
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // INFO: 在这里配置更多的 Mapping 关系。
            CreateMap<Create.Command, Models.Api.HearFromDetails>()
                .ForMember(x => x.Id, o => o.Ignore());
        }
    }
}
