using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Edit
    {
        public class Command : IRequest<ResponseResult>
        {
            public Command()
            {
                Trades = new List<Core.Models.BasicProducts.TradeDetails>();
            }

            /// <summary>
            /// 要编辑的物料Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 要编辑的物料参考采购价
            /// </summary>
            public decimal MaterialPrice { get; set; }

            /// <summary>
            /// 要编辑物料的类型编码
            /// </summary>
            public string MateTypeCode { get; set; }

            /// <summary>
            /// 要添加的物料采购主体
            /// </summary>
            public List<Core.Models.BasicProducts.TradeDetails> Trades { get; set; }

            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private const string _tradCode = "01";
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMaterialRepository _materialRepository;

            public Handler(
                IMaterialRepository materialRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _materialRepository = materialRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return ResponseResult.Error("物料 Id 不能为空");
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var material = await _materialRepository.FindAsync(request.Id);
                    if (material == null)
                    {
                        return ResponseResult.Error($"未找到 Id = {request.Id} 的符合要求的物料");
                    }

                    if (material.MaterialPrice != request.MaterialPrice)
                    {
                        await _materialRepository.UpdateAsync(
                            request.Id,
                            request.MaterialPrice,
                            string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);
                    }

                    var ids = material.Trades.Select(x => x.TradeId).ToList();

                    request.Trades = request.Trades
                        .Where(x => (!string.IsNullOrWhiteSpace(x.TradeId)) && (!ids.Contains(x.TradeId))).ToList();

                    bool hasTradesValues = false;
                    if (request.Trades.Count > 0)
                    {
                        hasTradesValues = true;
                        var trade = new List<QueryStack.Models.Trade>();
                        foreach (var one in request.Trades)
                        {
                            trade.Add(new QueryStack.Models.Trade()
                            {
                                ProdMaterId = material.Id,
                                TradeId = one.TradeId,
                                TradeName = one.TradeName,
                            });
                        }

                        await _materialRepository.AddTradeAsync(
                            trade,
                            string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);
                    }

                    if (!string.IsNullOrEmpty(request.MateTypeCode) && request.MateTypeCode.StartsWith(_tradCode))
                    {
                        if (!hasTradesValues)
                        {
                           return ResponseResult.Error("采购主体不能为空");
                        }
                    }

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
