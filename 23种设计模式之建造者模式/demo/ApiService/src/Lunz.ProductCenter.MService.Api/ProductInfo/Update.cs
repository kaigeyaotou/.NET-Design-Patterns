using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProductDetail.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProductInfo
{
    public class Update
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// Id
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 产品名称
            /// </summary>
            public string ProdName { get; set; }

            /// <summary>
            /// 产品全称
            /// </summary>
            public string ProdFullName { get; set; }

            /// <summary>
            /// 产品类型Id
            /// </summary>
            public string ProdTypeId { get; set; }

            /// <summary>
            /// 产品类型Code
            /// </summary>
            public string ProdTypeCode { get; set; }

            /// <summary>
            /// 产品类型名称
            /// </summary>
            public string ProdTypeName { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }

            /// <summary>
            /// 销售渠道
            /// </summary>
            public string[] Trades { get; set; }

            /// <summary>
            /// 物料
            /// </summary>
            public List<MaterialItem> Materials { get; set; }
        }

        public class MaterialItem
        {
            public string Id { get; set; }

            public string MaterialId { get; set; }

            public int Quantity { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductInfoRepository _productInfoRepository;
            private readonly ITradeRepository _tradeRepository;
            private readonly IProductDetailRepository _productDetailRepository;

            public Handler(
                IProductInfoRepository productInfoRepository,
                ITradeRepository tradeRepository,
                IProductDetailRepository productDetailRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productInfoRepository = productInfoRepository;
                _tradeRepository = tradeRepository;
                _productDetailRepository = productDetailRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.ProdName))
                {
                    return ResponseResult.Error("产品名称不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.ProdTypeId))
                {
                    return ResponseResult.Error("产品类型不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.ProdFullName))
                {
                    return ResponseResult.Error("产品全称不能为空.");
                }

                request.ProdName = request.ProdName.Trim();
                request.ProdFullName = request.ProdFullName.Trim();

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    // 产品信息
                    string errorMsg = await UpdateProduct(request);
                    if (!string.IsNullOrWhiteSpace(errorMsg))
                    {
                        return ResponseResult.Error(errorMsg);
                    }

                    // 产品-销售渠道关系
                    string tradeMsg = await DeleteAndAddProductTrades(request);
                    if (!string.IsNullOrWhiteSpace(tradeMsg))
                    {
                        return ResponseResult.Error(tradeMsg);
                    }

                    // 产品-物料  编辑
                    await UpdateProductMaterials(request, cancellationToken);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }

            private async Task<string> UpdateProduct(Command request)
            {
                string errorMsg = string.Empty;

                if (await _productInfoRepository.HasProductFullNameAsync(request.ProdFullName, request.Id))
                {
                    errorMsg = $"产品全称：{request.ProdFullName} 已存在.";
                    return errorMsg;
                }

                QueryStack.Models.ProductInfo entity = new QueryStack.Models.ProductInfo()
                {
                    Id = request.Id,
                    ProdName = request.ProdName,
                    ProdFullName = request.ProdFullName,
                    ProdTypeId = request.ProdTypeId,
                    ProdTypeCode = request.ProdTypeCode,
                    ProdTypeName = request.ProdTypeName,
                };
                await _productInfoRepository.UpdateAsync(entity, request.UserId);
                return errorMsg;
            }

            private async Task<string> DeleteAndAddProductTrades(Command request)
            {
                string errorMsg = string.Empty;

                if (request.Trades == null || request.Trades.Length == 0)
                {
                    return errorMsg;
                }

                request.Trades = request.Trades.Distinct().ToArray<string>();

                await _tradeRepository.DeleteByProductIdAsync(request.Id, request.UserId);

                List<MService.QueryStack.Models.Trade> entities = new List<MService.QueryStack.Models.Trade>();
                foreach (string trade in request.Trades)
                {
                    int index = trade.IndexOf("|||");
                    string tradeId = trade.Substring(0, index);
                    string tradeName = trade.Substring(index + 3);
                    if (string.IsNullOrWhiteSpace(tradeId) || string.IsNullOrWhiteSpace(tradeName))
                    {
                        errorMsg = errorMsg = $"销售渠道参数：{trade} 不符合规则.";
                        return errorMsg;
                    }

                    MService.QueryStack.Models.Trade entity = new MService.QueryStack.Models.Trade()
                    {
                        TradeId = tradeId,
                        TradeName = tradeName,
                        ProdMaterId = request.Id,
                    };
                    entities.Add(entity);
                }

                await _tradeRepository.AddAsync(entities);
                return errorMsg;
            }

            private async Task UpdateProductMaterials(Command request, CancellationToken cancellationToken)
            {
                string errorMsg = string.Empty;

                request.Materials = request.Materials.Distinct().ToList();

                foreach (var m in request.Materials)
                {
                    MService.QueryStack.Models.ProductDetail entity = new MService.QueryStack.Models.ProductDetail()
                    {
                        Id = m.Id,
                        MaterialId = m.MaterialId,
                        ProductId = request.Id,
                        Quantity = m.Quantity,
                    };
                    if (string.IsNullOrEmpty(entity.Id))
                    {
                        await _productDetailRepository.AddAsync(entity, request.UserId);
                    }
                    else
                    {
                        await _productDetailRepository.UpdateAsync(entity, request.UserId);
                    }
                }
            }
        }
    }
}
