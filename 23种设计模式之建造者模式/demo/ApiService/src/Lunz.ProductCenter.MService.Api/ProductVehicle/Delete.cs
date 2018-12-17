using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProductInfo.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.ProductVehicle
{
    public class Delete
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 选项主键
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 用户id
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductVehicleRepository _productVehicleRepository;

            public Handler(
                IProductVehicleRepository productVehicleRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productVehicleRepository = productVehicleRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return ResponseResult.Error("车型Id不能为空.");
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _productVehicleRepository.DeleteAsync(request.Id, request.UserId);
                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
