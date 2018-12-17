using AutoMapper;
using Lunz.ProductCenter.Core.Models.BasicProducts;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // INFO: 在这里配置更多的 Mapping 关系。
            CreateMap<QueryStack.Models.Material, Models.Api.MaterialDetails>()
                 .ForMember(d => d.Properties, o => o.MapFrom(s => s.Properties))
                 .ForMember(d => d.Trades, o => o.MapFrom(s => s.Trades));
            CreateMap<QueryStack.Models.MaterialProperty, MaterialPropertyDetails>();
            CreateMap<QueryStack.Models.Trade, TradeDetails>();
        }
    }
}
