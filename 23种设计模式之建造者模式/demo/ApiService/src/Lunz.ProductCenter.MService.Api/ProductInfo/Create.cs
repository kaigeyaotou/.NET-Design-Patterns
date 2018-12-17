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
    public class Create
    {
        public class Command : IRequest<ResponseResult<string>>
        {
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
            /// 产品Code
            /// </summary>
            public string ProdTypeCode { get; set; }

            /// <summary>
            /// 产品类型名称
            /// </summary>
            public string ProdTypeName { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string ProductId { get; set; }

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
            public string MaterialId { get; set; }

            public int Quantity { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<string>>
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

            public async Task<ResponseResult<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.ProdName))
                {
                    return ResponseResult<string>.Error("产品名称不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.ProdTypeId))
                {
                    return ResponseResult<string>.Error("产品类型不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.ProdFullName))
                {
                    return ResponseResult<string>.Error("产品全称不能为空.");
                }

                request.ProdName = request.ProdName.Trim();
                request.ProdFullName = request.ProdFullName.Trim();

                // 添加 产品信息
                var productId = await AddProductInfoAfterGetId(request, cancellationToken);
                request.ProductId = productId.productid;
                if (!string.IsNullOrWhiteSpace(productId.errorMsg))
                {
                    return ResponseResult<string>.Error(productId.errorMsg);
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    // 添加 产品-销售渠道关系
                    string tradeMsg = await AddProductTrades(request, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(tradeMsg))
                    {
                        return ResponseResult<string>.Error(tradeMsg);
                    }

                    // 添加产品-物料关系
                    await AddProductMaterials(request, cancellationToken);

                    scope.SaveChanges();
                }

                return ResponseResult<string>.Ok(productId.productid);
            }

            private async Task<(string productid, string errorMsg)> AddProductInfoAfterGetId(Command request, CancellationToken cancellationToken)
            {
                string productId = string.Empty;
                string errorMsg = string.Empty;

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    if (await _productInfoRepository.HasProductFullNameAsync(request.ProdFullName))
                    {
                        errorMsg = $"产品全称：{request.ProdFullName} 已存在.";
                        return (productId, errorMsg);
                    }

                    string currentmaxCode = await _productInfoRepository.GetMaxProductCodeAsync();
                    QueryStack.Models.ProductInfo entity = new QueryStack.Models.ProductInfo()
                    {
                        ProdCode = Lunz.ProductCenter.Common.FormatDataCode.GetNewLoacalCode("CP", currentmaxCode),
                        ProdName = request.ProdName,
                        ProdFullName = request.ProdFullName,
                        ProdTypeId = request.ProdTypeId,
                        ProdTypeCode = request.ProdTypeCode,
                        ProdTypeName = request.ProdTypeName,
                    };
                    await _productInfoRepository.AddAsync(entity, request.UserId);
                    scope.SaveChanges();

                    var newedProductInfo = _productInfoRepository.FindByProductFullNameAsync(request.ProdFullName);
                    if (newedProductInfo == null || newedProductInfo.Result == null)
                    {
                        errorMsg = $"产品：{request.ProdFullName} 保存异常.";
                        return (productId, errorMsg);
                    }

                    productId = newedProductInfo.Result.Id.ToString();
                }

                return (productId, errorMsg);
            }

            private async Task<string> AddProductTrades(Command request, CancellationToken cancellationToken)
            {
                List<MService.QueryStack.Models.Trade> entities = new List<MService.QueryStack.Models.Trade>();
                string errorMsg = string.Empty;

                if (request.Trades == null || request.Trades.Length == 0)
                {
                    return errorMsg;
                }

                await _tradeRepository.DeleteByProductIdAsync(request.ProductId, request.UserId);

                foreach (string trade in request.Trades)
                {
                    int index = trade.IndexOf("|||");
                    string tradeId = trade.Substring(0, index);
                    string tradeName = trade.Substring(index + 3);
                    if (string.IsNullOrWhiteSpace(tradeId) || string.IsNullOrWhiteSpace(tradeName))
                    {
                        errorMsg = $"销售渠道参数：{trade} 不符合规则.";
                        return errorMsg;
                    }

                    MService.QueryStack.Models.Trade entity = new MService.QueryStack.Models.Trade()
                    {
                        TradeId = tradeId,
                        TradeName = tradeName,
                        ProdMaterId = request.ProductId,
                    };
                    entities.Add(entity);
                }

                await _tradeRepository.AddAsync(entities);

                return errorMsg;
            }

            private async Task AddProductMaterials(Command request, CancellationToken cancellationToken)
            {
                string errorMsg = string.Empty;

                List<MaterialItem> handledItems = new List<MaterialItem>();

                foreach (var t in request.Materials)
                {
                    if (!handledItems.Any(q => q.MaterialId == t.MaterialId))
                    {
                        await _productDetailRepository.AddAsync(
                            new MService.QueryStack.Models.ProductDetail()
                            {
                                ProductId = request.ProductId,
                                MaterialId = t.MaterialId,
                                Quantity = t.Quantity,
                            }, request.UserId);
                        handledItems.Add(t);
                    }
                }
            }
        }
    }
}
