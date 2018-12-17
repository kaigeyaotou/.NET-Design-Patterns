using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models.BasicProducts;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.ProductDetail
{
    public class Delete
    {
        public class Command : IRequest<ResponseResult>
        {
            public string[] Ids { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductDetailRepository _productDetailRepository;

            public Handler(
                IProductDetailRepository productDetailRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productDetailRepository = productDetailRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _productDetailRepository.DeleteAsync(request.Ids, request.UserId);
                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
