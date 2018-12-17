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
    public class Enable
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 属性Id
            /// </summary>
            public string Id { get; set; }

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

                request.Id = request.Id.Trim();

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _propertyRespository.EnableAsync(request.Id, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
