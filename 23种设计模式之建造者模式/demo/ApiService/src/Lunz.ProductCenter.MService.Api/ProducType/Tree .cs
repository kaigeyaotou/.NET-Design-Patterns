using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProducType.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProducType
{
    public class Tree
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            public string ParentId { get; set; }
        }

        /// <summary>
        /// 类型树
        /// </summary>
        public class Response
        {
            public Response(IEnumerable<ProducTypeDetials> data)
            {
                Data = data;
            }

            /// <summary>
            /// 类型树.
            /// </summary>
            public IEnumerable<ProducTypeDetials> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProducTypeRepository _producTypeRepository;

            public Handler(
                IProducTypeRepository producTypeRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _producTypeRepository = producTypeRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _producTypeRepository.QueryAsync<ProducTypeDetials>(request.ParentId);

                    foreach (var r in result)
                    {
                        r.HasChildren = await _producTypeRepository.HasChildren(r.Id);
                    }

                    return ResponseResult<Response>.Ok(new Response(result));
                }
            }
        }
    }
}
