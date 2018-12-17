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
using Lunz.ProductCenter.Common;
using Lunz.ProductCenter.Common.MoveNode;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProductInfo
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
        public class Response : IPaginationModel<ProductInfoDetails>
        {
            public Response(IEnumerable<ProductInfoDetails> data, long count, int? pageIndex, int? pageSize)
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
            public IEnumerable<ApiService.ProductInfo.Api.ProductInfoDetails> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductInfoRepository _productInfoRepository;
            private readonly IFilterLikeSearch _filterLikeSearch;

            public Handler(
                IProductInfoRepository productInfoRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productInfoRepository = productInfoRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    // var filter = request.Filter.ToSql<ProductInfoDetails>();
                    // var filter = FieldLikeSearch.GetLikeSearchByField<ProductInfoDetails>("ProdName", request.Filter, 3);
                    var filter = FilterLikeSearch.Search<ProductInfoDetails>("1", "ProdName", request.Filter, 3, " ");
                    if (filter.errorMsg != string.Empty && filter.errorMsg != null)
                    {
                        return ResponseResult<Response>.Error($"{filter.errorMsg}.");
                    }

                    var result = await _productInfoRepository.QueryAsync<ProductInfoDetails>(
                        () => filter.Item2,
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
