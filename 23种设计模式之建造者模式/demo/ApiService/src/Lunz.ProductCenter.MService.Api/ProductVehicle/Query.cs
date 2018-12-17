using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProductInfo.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Models;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProductVehicle
{
    public class Query
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            public Command()
            {
                Sort = new List<PagingSort>();
                Filter = new QueryGroup() { Op = "and" };
            }

            /// <summary>
            /// 过滤条件
            /// </summary>
            public QueryGroup Filter { get; set; }

            /// <summary>
            /// 页号，如 1
            /// </summary>
            public int? PageIndex { get; set; }

            /// <summary>
            /// 每页行数，如 10
            /// </summary>
            public int? PageSize { get; set; }

            /// <summary>
            /// 排序规则
            /// </summary>
            public List<PagingSort> Sort { get; private set; }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public class Response : IPaginationModel<ApiService.ProductVehicle.Api.ProductVehicleDetails>
        {
            public Response(IEnumerable<ApiService.ProductVehicle.Api.ProductVehicleDetails> data, long count, int? pageIndex, int? pageSize)
            {
                Data = data;
                Count = count;
                PageIndex = pageIndex;
                PageSize = pageSize;
            }

            /// <summary>
            /// 总行数
            /// </summary>
            public long Count { get; }

            /// <summary>
            /// 当前页号
            /// </summary>
            public int? PageIndex { get; }

            /// <summary>
            /// 每页行数
            /// </summary>
            public int? PageSize { get; }

            /// <summary>
            /// 列表
            /// </summary>
            public IEnumerable<ApiService.ProductVehicle.Api.ProductVehicleDetails> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductVehicleRepository _productVehicleRepository;

            public Handler(
                IProductVehicleRepository productVehicleRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productVehicleRepository = productVehicleRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var filter = request.Filter.ToSql<ApiService.ProductVehicle.Api.ProductVehicleDetails>();
                    var result = await _productVehicleRepository.QueryAsync<ApiService.ProductVehicle.Api.ProductVehicleDetails>(
                        () => filter.ToTuble(),
                        request.PageIndex,
                        request.PageSize,
                        request.Sort.ToSql(),
                        true);
                    return ResponseResult<Response>.Ok(
                       new Response(result.Data, result.Count, request.PageIndex, request.PageSize));
                }
            }
        }
    }
}
