using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProductDetail.Api;
using Lunz.ProductCenter.ApiService.ProductInfo.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProductDetail
{
    public class Query
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            /// <summary>
            /// 关联产品的Id
            /// </summary>
            public string ProductId { get; set; }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public class Response
        {
            public Response(IEnumerable<ProductMaterialDetails> data)
            {
                Data = data;
            }

            /// <summary>
            /// 列表
            /// </summary>
            public IEnumerable<ProductMaterialDetails> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
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

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _productDetailRepository.QueryAsync<ProductMaterialDetails>(request.ProductId);
                    return ResponseResult<Response>.Ok(
                       new Response(result));
                }
            }
        }
    }
}
