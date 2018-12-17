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
    public class Create
    {
        public class Command : IRequest<ResponseResult>
        {
            public List<Item> Items { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Item
        {
            /// <summary>
            /// 产品 Id
            /// </summary>
            public string ProductId { get; set; }

            /// <summary>
            /// 物料 Id
            /// </summary>
            public string MaterialId { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            public int Quantity { get; set; }
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
                if (request.Items == null || request.Items.Count() == 0)
                {
                    return ResponseResult.Error("暂无请求参数数据.");
                }

                request.Items = request.Items.Distinct().ToList();

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    List<Item> handledItems = new List<Item>();

                    foreach (var t in request.Items)
                    {
                        if (!handledItems.Any(q => q.MaterialId == t.MaterialId && q.ProductId == t.ProductId))
                        {
                            await _productDetailRepository.AddAsync(
                                new QueryStack.Models.ProductDetail()
                                {
                                    ProductId = t.ProductId,
                                    MaterialId = t.MaterialId,
                                    Quantity = t.Quantity,
                                }, request.UserId);
                            scope.SaveChanges();
                            handledItems.Add(t);
                        }
                    }
                }

                return ResponseResult.Ok();
            }
        }
    }
}
