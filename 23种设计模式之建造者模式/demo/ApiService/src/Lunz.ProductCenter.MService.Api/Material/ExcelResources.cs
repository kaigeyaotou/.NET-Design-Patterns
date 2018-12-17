using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.Core.Models;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class ExcelResources
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
            public List<PagingSort> Sort { get; set; }
        }

        /// <summary>
        /// 导出资源列表
        /// </summary>
        public class Response : IPaginationModel<ResourceItem>
        {
            public Response(IEnumerable<ResourceItem> data, long count, int? pageIndex, int? pageSize)
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
            /// 导出资源列表
            /// </summary>
            public IEnumerable<ResourceItem> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMaterialRepository _repository;

            public Handler(
                IMaterialRepository repository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _repository = repository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var filter = request.Filter.ToSql<ResourceItem>();

                    var result = await _repository.FindExcelResourcesAsync<ResourceItem>(
                        () => filter.ToTuble(),
                        request.PageIndex,
                        request.PageSize,
                        request.Sort.ToSql(),
                        true);

                    return ResponseResult<Response>.Ok(new Response(
                        result.Data,
                        result.Count,
                        request.PageIndex,
                        request.PageSize));
                }
            }
        }
    }
}
