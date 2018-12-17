using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.PropertiesOption
{
    public class Create
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 关联属性Id
            /// </summary>
            public string PropId { get; set; }

            /// <summary>
            /// 属性选项名称
            /// </summary>
            public string OptionName { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IPropertiesOptionResponsitory _propertiesOptionResponsitory;

            public Handler(
                IPropertiesOptionResponsitory propertiesOptionResponsitory,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _propertiesOptionResponsitory = propertiesOptionResponsitory;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.PropId))
                {
                    return ResponseResult.Error("关联属性Id不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.OptionName))
                {
                    return ResponseResult.Error("属性选项名称不能为空.");
                }

                request.PropId = request.PropId.Trim();
                request.OptionName = request.OptionName.Trim();

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var checkData = await _propertiesOptionResponsitory.FindByOptionNameAsync(request.PropId, request.OptionName);
                    if (checkData != null)
                    {
                        return ResponseResult.Error($"属性选项名称: {request.OptionName} 已存在.");
                    }
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var maxOrder = await _propertiesOptionResponsitory.MaxSortOrderValue(request.PropId);

                    await _propertiesOptionResponsitory.AddAsync(
                        new QueryStack.Models.PropertiesOption()
                        {
                            PropId = request.PropId,
                            OptionName = request.OptionName,
                            IsDisable = false,
                            SortOrder = maxOrder + 1,
                        }, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
