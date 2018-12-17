using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProducType
{
    public class CreateTheChild
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 当前分类code
            /// </summary>
            public string CurrentCode { get; set; }

            /// <summary>
            /// 当前分类Id
            /// </summary>
            public string CurrentId { get; set; }

            /// <summary>
            /// 当前分类等级
            /// </summary>
            public int CurrentLevelCode { get; set; }

            /// <summary>
            /// 子级分类名称
            /// </summary>
            public string TypeName { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
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

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.TypeName))
                {
                    return ResponseResult.Error("分类名称不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.CurrentId))
                {
                    return ResponseResult.Error("当前分类Id不能为空.");
                }

                request.TypeName = request.TypeName.Trim();
                request.CurrentId = request.CurrentId.Trim();

                int maxChildrenTypeSort = 0;
                string maxCode = string.Empty;

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    maxChildrenTypeSort = await _producTypeRepository.GetMaxSortOrderValue(request.CurrentId);

                    if (maxChildrenTypeSort > 99)
                    {
                        return ResponseResult.Error("分类级别已达到上限.");
                    }

                    bool hasSameName = await _producTypeRepository.CheckTheSameTypeName(request.TypeName, request.CurrentId);
                    if (hasSameName)
                    {
                        return ResponseResult.Error($"子级分类: {request.TypeName} 已存在.");
                    }

                    maxCode = await _producTypeRepository.GetMaxCode(request.CurrentId);
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    QueryStack.Models.ProducType pt = new QueryStack.Models.ProducType
                    {
                        TypeCode = Common.FormatDataCode.GetNewMaxCode(maxCode, request.CurrentCode),
                        ParentId = request.CurrentId,
                        TypeName = request.TypeName,
                        LevelCode = request.CurrentLevelCode + 1,
                        SortOrder = maxChildrenTypeSort + 1,
                    };
                    await _producTypeRepository.AddAsync(pt, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
