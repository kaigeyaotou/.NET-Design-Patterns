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
    public class Update
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 父级Id
            /// </summary>
            public string ParentId { get; set; }

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
                QueryStack.Models.ProducType pt = new QueryStack.Models.ProducType
                {
                    TypeName = request.TypeName,
                    ParentId = request.ParentId,
                    Id = request.Id,
                };

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    bool hasSameName = false;
                    if (string.IsNullOrWhiteSpace(request.ParentId))
                    {// 顶级分类查重
                        hasSameName = await _producTypeRepository.CheckTheTopTypeName(pt.TypeName, pt.Id);
                    }
                    else
                    {// 分类查重
                        hasSameName = await _producTypeRepository.CheckTheSameTypeName(pt.TypeName, pt.ParentId, pt.Id);
                    }

                    if (hasSameName)
                    {
                        return ResponseResult.Error($"分类: {pt.TypeName} 已存在.");
                    }
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _producTypeRepository.UpdateAsync(pt, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
