using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.PropertiesOption.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.PropertiesOption
{
    public class Tree
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            /// <summary>
            /// 关联属性Id
            /// </summary>
            public string PropId { get; set; }
        }

        /// <summary>
        /// 属性列表
        /// </summary>
        public class Response
        {
            public Response(IEnumerable<PropertiesOptionDetials> data)
            {
                Data = data;
            }

            /// <summary>
            /// 属性选项数据集合
            /// </summary>
            public IEnumerable<PropertiesOptionDetials> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IPropertiesOptionResponsitory _propertiesOptionResponsitory;

            public Handler(
                IPropertiesOptionResponsitory propertiesOptionResponsitory,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _propertiesOptionResponsitory = propertiesOptionResponsitory;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _propertiesOptionResponsitory.QueryAsync<PropertiesOptionDetials>(request.PropId);
                    return ResponseResult<Response>.Ok(
                       new Response(result));
                }
            }
        }
    }
}
