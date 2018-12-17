using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProductDetail.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.Trade
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
            public Response(IEnumerable<ApiService.Trade.Api.TradeDetails> data)
            {
                Data = data;
            }

            /// <summary>
            /// 列表
            /// </summary>
            public IEnumerable<ApiService.Trade.Api.TradeDetails> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly ITradeRepository _tradeRepository;

            public Handler(
                ITradeRepository tradeRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _tradeRepository = tradeRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _tradeRepository.QueryAsync<ApiService.Trade.Api.TradeDetails>(request.ProductId);
                    return ResponseResult<Response>.Ok(
                       new Response(result));
                }
            }
        }
    }
}
