using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Create
    {
        public class Command : Core.Models.BasicProducts.MaterialDetails, IRequest<ResponseResult>
        {
            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
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
                request.UserId = string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId;

                var material = new QueryStack.Models.Material()
                {
                    MateName = request.MateName,
                    MateTypeId = request.MateTypeId,
                    MateTypeCode = string.IsNullOrWhiteSpace(request.MateTypeCode) ? null : request.MateTypeCode,
                    MateTypeName = string.IsNullOrWhiteSpace(request.MateTypeName) ? null : request.MateTypeName,
                    MaterialSpecId = string.IsNullOrWhiteSpace(request.MaterialSpecId) ? null : request.MaterialSpecId,
                    MaterialSpec = string.IsNullOrWhiteSpace(request.MaterialSpec) ? null : request.MaterialSpec,
                    MaterialUnitId = request.MaterialUnitId,
                    MaterialUnits = string.IsNullOrWhiteSpace(request.MaterialUnits) ? null : request.MaterialUnits,
                    MaterialPrice = request.MaterialPrice,
                    IdCodeSingle = request.IdCodeSingle,
                    IsSelfBuild = request.IsSelfBuild,
                };
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var result = await _materialRepository.GetIdAndMaxCodeAsync();
                    material.Id = result.Id;
                    var c = 1;
                    if (result.Data != null)
                    {
                        c = int.Parse(result.Data.MateCode.Substring(2)) + 1;
                    }

                    material.MateCode = "WL" + c.ToString().PadLeft(8, '0');

                    await _materialRepository.AddAsync(material, request.UserId);

                    request.Properties = request.Properties
                        .Where(x => (!string.IsNullOrWhiteSpace(x.PropId) && !string.IsNullOrWhiteSpace(x.OptionId)))
                    .Distinct().ToList();

                    if (request.Properties.Count > 0)
                    {
                        var pops = new List<QueryStack.Models.MaterialProperty>();
                        foreach (var one in request.Properties)
                        {
                            pops.Add(new QueryStack.Models.MaterialProperty()
                            {
                                MaterialId = material.Id,
                                PropId = one.PropId,
                                PropName = one.PropName,
                                OptionId = one.OptionId,
                            });
                        }

                        await _materialRepository.AddPropertiesAsync(pops, request.UserId);
                    }

                    request.Trades = request.Trades.Where(x => !string.IsNullOrWhiteSpace(x.TradeId)).Distinct().ToList();
                    if (request.Trades.Count > 0)
                    {
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

                        await _materialRepository.AddTradeAsync(trade, request.UserId);
                    }

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
