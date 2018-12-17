using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.PictureResource
{
    public class Create
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 产品Id
            /// </summary>
            public string ProductId { get; set; }

            /// <summary>
            /// 上传图片信息集合
            /// </summary>
            public List<ProductPicture> Pictures { get; set; }

            /// <summary>
            /// 用户GUID
            /// </summary>
            [NSwag.Annotations.SwaggerIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IPictureResourceRepository _repository;

            public Handler(
                IPictureResourceRepository repository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _repository = repository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    if (request.Pictures?.Any() ?? false)
                    {
                        await _repository.AddPictureResourcesAsync(request.ProductId, request.Pictures, request.UserId);
                        scope.SaveChanges();
                    }
                }

                return ResponseResult.Ok();
            }
        }
    }
}
