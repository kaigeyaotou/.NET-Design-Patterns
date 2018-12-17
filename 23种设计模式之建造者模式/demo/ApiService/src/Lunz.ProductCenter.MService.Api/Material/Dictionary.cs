using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Dictionary
    {
        public class Command : IRequest<IEnumerable<DataDictionary>>
        {
            /// <summary>
            /// 是否取得禁用的数据
            /// </summary>
            public bool NeedAll { get; set; }
        }

        public class Handler : IRequestHandler<Command, IEnumerable<DataDictionary>>
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

            public async Task<IEnumerable<DataDictionary>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _materialRepository.DataDicAsync(request.NeedAll);
                    return result;
                }
            }
        }
    }
}
