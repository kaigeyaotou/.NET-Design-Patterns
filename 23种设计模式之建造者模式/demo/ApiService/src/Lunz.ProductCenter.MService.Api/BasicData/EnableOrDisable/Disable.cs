using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.BasicData
{
    public class Disable
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 父级id
            /// </summary>
            public string ParentId { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IBasicDataRepository _basicDataRepository;

            public Handler(
                IBasicDataRepository basicDataRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _basicDataRepository = basicDataRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return ResponseResult.Error($"属性标识Id: {request.Id} 不能为空.");
                }

                request.Id = request.Id.Trim();

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    if (!string.IsNullOrEmpty(request.ParentId))
                    {
                        if (!await _basicDataRepository.AllowDisabledOfThisOptionAsync(request.ParentId, request.Id))
                        {
                            return ResponseResult.Error($"字典选项至少保留一项启用值.");
                        }
                    }

                    await _basicDataRepository.DisableAsync(request.Id, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
