using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.BasicData.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.BasicData
{
    public class QueryByParentId
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            /// <summary>
            /// 全部：空；顶级：0；下级：父级id
            /// </summary>
            public string ParentId { get; set; }

            public bool? Enable { get; set; }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public class Response
        {
            public Response(IEnumerable<BasicDataDetails> data)
            {
                Data = data;
            }

            /// <summary>
            /// 列表
            /// </summary>
            public IEnumerable<BasicDataDetails> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IBasicDataRepository _basicDataRepository;

            public Handler(
                IBasicDataRepository basicDataRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _basicDataRepository = basicDataRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var resultData = await _basicDataRepository.QueryByParentIdAsync<BasicDataDetails>(request.ParentId);
                    if (request.Enable.HasValue)
                    {
                        resultData = resultData.Where(q => q.Enabled == request.Enable);
                    }

                    return ResponseResult<Response>.Ok(new Response(resultData));
                }
            }
        }
    }
}
