using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Properties
    {
        public class Command : IRequest<IEnumerable<MaterialTypeProperty>>
        {
            /// <summary>
            /// 要获取自定义属性的物料 Id
            /// </summary>
            public string MaterialId { get; set; }
        }

        public class Handler : IRequestHandler<Command, IEnumerable<MaterialTypeProperty>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMaterialRepository _materialRepository;

            public Handler(
                IMaterialRepository materialRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _materialRepository = materialRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<IEnumerable<MaterialTypeProperty>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _materialRepository.GetPropertiesAsync(request.MaterialId);
                    return result;
                }
            }
        }
    }
}
