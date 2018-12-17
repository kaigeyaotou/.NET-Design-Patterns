using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.MaterialType
{
    public class Properties
    {
        public class Command : IRequest<IEnumerable<MaterialTypePropertyDetails>>
        {
            /// <summary>
            /// 要获取自定义属性的物料类型 Id
            /// </summary>
            public string MaterialTypeId { get; set; }
        }

        public class List : IRequest<IEnumerable<MaterialTypeProperty>>
        {
            /// <summary>
            /// 要获取自定义属性的物料类型 Id
            /// </summary>
            public string MaterialTypeId { get; set; }
        }

        public class Handler : IRequestHandler<Command, IEnumerable<MaterialTypePropertyDetails>>,
            IRequestHandler<List, IEnumerable<MaterialTypeProperty>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMaterialTypeRepository _materialTypeRepository;

            public Handler(
                IMaterialTypeRepository materialTypeRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _materialTypeRepository = materialTypeRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<IEnumerable<MaterialTypePropertyDetails>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _materialTypeRepository.GetPropertiesAsync<MaterialTypePropertyDetails>(request.MaterialTypeId);
                    return result;
                }
            }

            public async Task<IEnumerable<MaterialTypeProperty>> Handle(List request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _materialTypeRepository.GetPropertiesAsync(request.MaterialTypeId);
                    return result;
                }
            }
        }
    }
}
