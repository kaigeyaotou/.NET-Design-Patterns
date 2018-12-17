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
    public class CreateTheTop
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 顶级分类名称
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
                    return ResponseResult.Error("顶级分类不能为空.");
                }

                request.TypeName = request.TypeName.Trim();

                int maxTopTypeSort = 0;
                string maxCode = string.Empty;

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    maxTopTypeSort = await _producTypeRepository.GetMaxSortOrderValue(string.Empty);

                    if (maxTopTypeSort > 99)
                    {
                        return ResponseResult.Error("分类级别已达到上限.");
                    }

                    bool hasSameName = await _producTypeRepository.CheckTheTopTypeName(request.TypeName);
                    if (hasSameName)
                    {
                        return ResponseResult.Error($"顶级分类: {request.TypeName} 已存在.");
                    }

                    maxCode = await _producTypeRepository.GetMaxCode(string.Empty);
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    QueryStack.Models.ProducType pt = new QueryStack.Models.ProducType
                    {
                        TypeCode = Common.FormatDataCode.GetNewMaxCode(maxCode, string.Empty),
                        TypeName = request.TypeName,
                        LevelCode = 1,
                        SortOrder = maxTopTypeSort + 1,
                    };
                    await _producTypeRepository.AddAsync(pt, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
