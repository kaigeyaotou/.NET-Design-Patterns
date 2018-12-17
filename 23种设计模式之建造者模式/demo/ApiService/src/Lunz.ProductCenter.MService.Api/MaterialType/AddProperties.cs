using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.MaterialType
{
    public class AddProperties
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 要添加属性的物料类型Id
            /// </summary>
            public string MaterialTypeId { get; set; }

            /// <summary>
            /// 要添加属性的 Id 集合
            /// </summary>
            public string[] PropIds { get; set; }

            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMaterialTypeRepository _materialTypeRepository;

            public Handler(
                IMaterialTypeRepository materialTypeRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _materialTypeRepository = materialTypeRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.MaterialTypeId))
                {
                    return ResponseResult.Error("物料类型 Id 不能为空。");
                }

                if (request.PropIds == null || request.PropIds.Length <= 0)
                {
                    return ResponseResult.Error("属性 Ids 不能为空");
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var materialType = await _materialTypeRepository.FindAsync(request.MaterialTypeId);
                    if (materialType == null || materialType.Deleted == true)
                    {
                        return ResponseResult.Error($"未找到 Id ={request.MaterialTypeId} 的符合要求的物料类型。");
                    }

                    await _materialTypeRepository.AddPropertiesAsync(
                        request.MaterialTypeId,
                        request.PropIds,
                        string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
