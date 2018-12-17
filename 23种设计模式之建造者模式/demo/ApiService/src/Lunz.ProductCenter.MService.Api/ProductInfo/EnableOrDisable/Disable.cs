using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProductInfo.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProductInfo
{
    public class Disable
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 选项主键
            /// </summary>
            public List<string> Ids { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductInfoRepository _productInfoRepository;

            public Handler(
                IProductInfoRepository productInfoRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productInfoRepository = productInfoRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Ids?.Count == 0)
                {
                    return ResponseResult.Error("暂无需要更新的数据.");
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _productInfoRepository.DisableAsync(request.Ids, request.UserId);

                    scope.SaveChanges();
                }
                return ResponseResult.Ok();
            }
        }
    }
}