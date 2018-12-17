using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Trade
{
    public class CreateMultiple
    {
        public class Command : IRequest<ResponseResult>
        {
            public List<Item> Trades { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Item
        {
            /// <summary>
            /// 销售渠道id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            ///  销售渠道名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 关联的产品Id
            /// </summary>
            public string ProductId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
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

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!request.Trades.Any())
                {
                    return ResponseResult.Error("请求参数不能为空.");
                }

                var list = new List<QueryStack.Models.Trade>();

                foreach (var t in request.Trades)
                {
                    list.Add(new QueryStack.Models.Trade()
                    {
                        TradeName = t.Name,
                        TradeId = t.Id,
                        ProdMaterId = t.ProductId,
                    });
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _tradeRepository.AddAsync(list);
                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
