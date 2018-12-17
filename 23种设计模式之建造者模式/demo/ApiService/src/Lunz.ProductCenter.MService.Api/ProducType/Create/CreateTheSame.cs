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
    public class CreateTheSame
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 父级Id
            /// </summary>
            public string ParentId { get; set; }

            /// <summary>
            /// 父级code
            /// </summary>
            public string ParentCode { get; set; }

            /// <summary>
            /// 当前分类级别
            /// </summary>
            public int CurrentLevelCode { get; set; }

            /// <summary>
            /// 名称
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

                if (string.IsNullOrWhiteSpace(request.ParentId))
                {
                    return ResponseResult.Error("父级分类不能为空.");
                }

                if (request.CurrentLevelCode == 1 && !string.IsNullOrWhiteSpace(request.ParentId))
                {
                    return ResponseResult.Error("此操作暂无顶级分类创建权限.");
                }

                request.TypeName = request.TypeName.Trim();
                request.ParentId = request.ParentId.Trim();

                int maxSortOrder = 0;
                string maxCode = string.Empty;

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    maxSortOrder = await _producTypeRepository.GetMaxSortOrderValue(request.ParentId);

                    if (maxSortOrder > 99)
                    {
                        return ResponseResult.Error("分类级别已达到上限.");
                    }

                    bool hasSameName = await _producTypeRepository.CheckTheSameTypeName(request.TypeName, request.ParentId);
                    if (hasSameName)
                    {
                        return ResponseResult.Error($"同级分类: {request.TypeName} 已存在.");
                    }

                    maxCode = await _producTypeRepository.GetMaxCode(request.ParentId);
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    QueryStack.Models.ProducType pt = new QueryStack.Models.ProducType
                    {
                        TypeCode = Common.FormatDataCode.GetNewMaxCode(maxCode, request.ParentCode),
                        ParentId = request.ParentId,
                        TypeName = request.TypeName,
                        LevelCode = request.CurrentLevelCode,
                        SortOrder = maxSortOrder + 1,
                    };

                    await _producTypeRepository.AddAsync(pt, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
