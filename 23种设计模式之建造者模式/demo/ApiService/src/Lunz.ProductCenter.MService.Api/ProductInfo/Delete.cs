using System.Collections.Generic;
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
    public class Delete
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 选项主键
            /// </summary>
            public string Id { get; set; }

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
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return ResponseResult.Error($"标识Id: {request.Id} 不能为空.");
                }

                request.Id = request.Id.Trim();

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    if (await _productInfoRepository.IsReleaseAsync(request.Id))
                    {
                        return ResponseResult.Error($"发布状态不不允许删除.");
                    }

                    await _productInfoRepository.DeleteAsync(request.Id, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
