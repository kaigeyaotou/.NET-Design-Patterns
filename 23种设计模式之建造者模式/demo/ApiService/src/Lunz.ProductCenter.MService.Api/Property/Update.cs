using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.Property
{
    public class Update
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 属性Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 属性名称
            /// </summary>
            public string PropName { get; set; }

            /// <summary>
            /// 属性描述名称
            /// </summary>
            public string DisplayName { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IPropertyRespository _propertyRespository;

            public Handler(
                IPropertyRespository propertyRespository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _propertyRespository = propertyRespository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return ResponseResult.Error($"属性标识Id: {request.Id} 不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.PropName))
                {
                    return ResponseResult.Error("属性名称不能为空");
                }

                if (string.IsNullOrWhiteSpace(request.PropName))
                {
                    return ResponseResult.Error("属性显示名称不能为空");
                }

                request.Id = request.Id.Trim();
                request.PropName = request.PropName.Trim();
                request.DisplayName = request.DisplayName.Trim();

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    bool has_propName = await _propertyRespository.CheckdPropNameAsync(request.PropName, request.Id);
                    if (has_propName)
                    {
                        return ResponseResult.Error($"属性名称: {request.PropName} 已存在.");
                    }
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _propertyRespository.UpdateAsync(
                        new QueryStack.Models.Property()
                        {
                            Id = request.Id,
                            PropName = request.PropName,
                            DisplayName = request.DisplayName,
                        }, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
