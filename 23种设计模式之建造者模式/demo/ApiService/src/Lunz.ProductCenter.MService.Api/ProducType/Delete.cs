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
    public class Delete
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// Id
            /// </summary>
            public string Id { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProducTypeRepository _producTypeRepository;
            private readonly IProductInfoRepository _productInfoRepository;

            public Handler(
                IProducTypeRepository producTypeRepository,
                IProductInfoRepository productInfoRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _producTypeRepository = producTypeRepository;
                _productInfoRepository = productInfoRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return ResponseResult.Error("分类Id不能为空.");
                }

                request.Id = request.Id.Trim();

                QueryStack.Models.ProducType pt = new QueryStack.Models.ProducType();
                pt.Id = request.Id;
                pt.Deleted = true;

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    if (await _producTypeRepository.HasChildren(pt.Id))
                    {
                        return ResponseResult.Error("存在子级数据 , 本级不允许删除.");
                    }

                    if (await _productInfoRepository.HasProductOfThisTypeAsync(pt.Id))
                    {
                        return ResponseResult.Error("此分类下存在产品数据 , 不允许删除.");
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
