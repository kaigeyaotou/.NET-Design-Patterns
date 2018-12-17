using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.Common;
using Lunz.ProductCenter.Core.Models;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class QueryForProduct
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            public Command()
            {
                Sort = new List<PagingSort>();
                Filter = new QueryGroup() { Op = "AND" };
            }

            /// <summary>
            /// 过滤条件
            /// </summary>
            public QueryGroup Filter { get; set; }

            /// <summary>
            /// 要获取物料列表的物料类型 Id
            /// </summary>
            public string MaterialTypeId { get; set; }

            /// <summary>
            /// 采购主体
            /// </summary>
            public List<string> TradeNames { get; set; }

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
        /// 物料数据列表
        /// </summary>
        public class Response : IPaginationModel<MaterialViewDetails>
        {
            public Response(IEnumerable<MaterialViewDetails> data, long count, int? pageIndex, int? pageSize)
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
            /// 物料列表
            /// </summary>
            public IEnumerable<MaterialViewDetails> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
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

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                request.MaterialTypeId = request.MaterialTypeId ?? request.MaterialTypeId;
                request.TradeNames = request.TradeNames ?? request.TradeNames;

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var filter = FieldLikeSearch.GetLikeSearchByField<MaterialViewDetails>("MateName", request.Filter, 3);
                    if (filter.errorMsg != string.Empty)
                    {
                        return ResponseResult<Response>.Error($"{filter.errorMsg}.");
                    }

                    var result = await _materialRepository.QueryForProductAsync<MaterialViewDetails>(
                        request.MaterialTypeId,
                        request.TradeNames,
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
