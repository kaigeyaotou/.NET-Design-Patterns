using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Update
    {
        public class Command : MaterialDetails, IRequest<ResponseResult>
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
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var material = await _materialRepository.FindAsync(request.Id);
                    if (material == null)
                    {
                        return ResponseResult.Error($"未找到 Id={request.Id} 的符合要求的物料数据");
                    }

                    request.UserId = string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId;

                    material.MateName = request.MateName;
                    material.MateTypeId = request.MateTypeId;
                    material.MateTypeCode = string.IsNullOrWhiteSpace(request.MateTypeCode) ? null : request.MateTypeCode;
                    material.MateTypeName = string.IsNullOrWhiteSpace(request.MateTypeName) ? null : request.MateTypeName;
                    material.MaterialSpecId = string.IsNullOrWhiteSpace(request.MaterialSpecId) ? null : request.MaterialSpecId;
                    material.MaterialSpec = string.IsNullOrWhiteSpace(request.MaterialSpec) ? null : request.MaterialSpec;
                    material.MaterialUnitId = request.MaterialUnitId;
                    material.MaterialUnits = string.IsNullOrWhiteSpace(request.MaterialUnits) ? null : request.MaterialUnits;
                    material.MaterialPrice = request.MaterialPrice;
                    material.IdCodeSingle = request.IdCodeSingle;
                    material.IsSelfBuild = request.IsSelfBuild;

                    request.Properties = request.Properties.Where(x => (!string.IsNullOrWhiteSpace(x.PropId)) &&
                    (!string.IsNullOrWhiteSpace(x.OptionId))).Distinct().ToList();
                    if (request.Properties.Count > 0)
                    {
                        // 添加
                        var add = request.Properties.Where(x => !material.Properties.Any(y => y.PropId == x.PropId)).ToList();
                        if (add.Count > 0)
                        {
                            var pops = new List<QueryStack.Models.MaterialProperty>();
                            foreach (var one in add)
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

                        // 编辑
                        var edit = request.Properties.Where(x => material.Properties.Any(y => y.PropId == x.PropId && y.OptionId != x.OptionId)).ToList();
                        if (edit.Count > 0)
                        {
                            var pops_2 = new List<QueryStack.Models.MaterialProperty>();
                            foreach (var one in edit)
                            {
                                var mp = material.Properties.FirstOrDefault(x => x.PropId == one.PropId);
                                pops_2.Add(new QueryStack.Models.MaterialProperty()
                                {
                                    Id = mp.Id,
                                    MaterialId = material.Id,
                                    PropId = one.PropId,
                                    PropName = one.PropName,
                                    OptionId = one.OptionId,
                                });
                            }

                            await _materialRepository.UpdatePropertiesAsync(pops_2, request.UserId);
                        }

                        // 删除
                        var delete = material.Properties.Where(x => !request.Properties.Any(y => y.PropId == x.PropId)).ToList();
                        if (delete.Count > 0)
                        {
                            await _materialRepository.DeletePropertiesAsync(delete.Select(x => x.Id).ToArray());
                        }
                    }
                    else
                    {
                        if (material.Properties.Count > 0)
                        {
                            await _materialRepository.DeletePropertiesAsync(material.Properties.Select(x => x.Id).ToArray());
                        }
                    }

                    request.Trades = request.Trades.Where(x => !string.IsNullOrWhiteSpace(x.TradeId)).Distinct().ToList();
                    if (request.Trades.Count > 0)
                    {
                        // 添加
                        var add = request.Trades.Where(x => !material.Trades.Any(y => y.TradeId == x.TradeId)).ToList();
                        if (add.Count > 0)
                        {
                            var trade = new List<QueryStack.Models.Trade>();
                            foreach (var one in add)
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

                        // 删除
                        var delete = material.Trades.Where(x => !request.Trades.Any(y => y.TradeId == x.TradeId)).ToList();
                        if (delete.Count > 0)
                        {
                            await _materialRepository.DeleteTradeAsync(delete.Select(x => x.Id).ToArray());
                        }
                    }
                    else
                    {
                        await _materialRepository.DeleteTradeAsync(material.Trades.Select(x => x.Id).ToArray());
                    }

                    await _materialRepository.UpdateAsync(material, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
