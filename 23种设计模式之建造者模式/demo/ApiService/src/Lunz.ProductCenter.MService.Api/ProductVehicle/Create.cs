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

namespace Lunz.ProductCenter.ApiService.Api.ProductVehicle
{
    public class Create
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 关联产品id
            /// </summary>
            public string ProductId { get; set; }

            public List<Item> Vehicles { get; set; }

            /// <summary>
            /// 用户id
            /// </summary>
            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Item
        {
            /// <summary>
            /// 车系
            /// </summary>
            public string Abbreviation { get; set; }

            /// <summary>
            /// 车系名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// id -> VehicleId（车型或者车系）
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 品牌名称 -> BrandId
            /// </summary>
            public string RootBrandId { get; set; }

            /// <summary>
            /// 品牌名称 ->  BrandName
            /// </summary>
            public string RootBrandName { get; set; }

            /// <summary>
            /// 子品牌Id -> ChildBrandId
            /// </summary>
            public string BrandId { get; set; }

            /// <summary>
            /// 子品牌名称 -> ChildBrandName
            /// </summary>
            public string BrandName { get; set; }

            /// <summary>
            /// 车系Id
            /// </summary>
            public string SeriesId { get; set; }

            /// <summary>
            /// 车系名称
            /// </summary>
            public string SeriesName { get; set; }

            /// <summary>
            /// 车型Id -> ModelId
            /// </summary>
            public string VehicleInfoId { get; set; }

            /// <summary>
            /// 车型名称 -> ModelName
            /// </summary>
            public string VehicleName { get; set; }

            /// <summary>
            /// 年份
            /// </summary>
            public int? Year { get; set; }

            /// <summary>
            /// 排量
            /// </summary>
            public decimal? Displacement { get; set; }

            /// <summary>
            /// 排量
            /// </summary>
            public string DisplacementText { get; set; }

            /// <summary>
            /// 马力
            /// </summary>
            public int? Horsepower { get; set; }

            /// <summary>
            /// 进气形式
            /// </summary>
            public string IntakeType { get; set; }

            /// <summary>
            /// 变速箱
            /// </summary>
            public string GearboxName { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductVehicleRepository _productVehicleRepository;

            public Handler(
                IProductVehicleRepository productVehicleRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productVehicleRepository = productVehicleRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.ProductId))
                {
                    return ResponseResult.Error("产品Id不能为空.");
                }

                List<QueryStack.Models.ProductVehicle> entities = new List<QueryStack.Models.ProductVehicle>();
                foreach (var data in request.Vehicles)
                {
                    QueryStack.Models.ProductVehicle entity = null;

                    // 全车型
                    if (!string.IsNullOrEmpty(data.Abbreviation))
                    {
                        entity = new QueryStack.Models.ProductVehicle()
                        {
                            ProductId = request.ProductId,
                            BrandId = data.BrandId,
                            BrandName = data.BrandName,
                            SeriesId = data.Id,
                            SeriesName = data.Name,
                            ModelName = "全车型",
                        };

                        using (var scope = _databaseScopeFactory.CreateReadOnly())
                        {
                            if (!await _productVehicleRepository.HasSameSeries(request.ProductId, entity.SeriesId)
                                 && !entities.Contains(entity))
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        entity = new QueryStack.Models.ProductVehicle()
                        {
                            VehicleId = data.Id,
                            ProductId = request.ProductId,
                            BrandId = data.RootBrandId,
                            BrandName = data.RootBrandName,
                            ChildBrandId = data.BrandId,
                            ChildBrandName = data.BrandName,
                            SeriesId = data.SeriesId,
                            SeriesName = data.SeriesName,
                            ModelId = data.VehicleInfoId,
                            ModelName = data.VehicleName,
                            Year = data.Year,
                            Displacement = data.Displacement,
                            DisplacementText = data.DisplacementText,
                            Horsepower = data.Horsepower,
                            IntakeType = data.IntakeType,
                            GearboxName = data.GearboxName,
                        };

                        using (var scope = _databaseScopeFactory.CreateReadOnly())
                        {
                            if (!await _productVehicleRepository.HasSameModel(request.ProductId, entity.VehicleId)
                                 && !entities.Contains(entity))
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                }

                if (!entities.Any())
                {
                    goto RETURN_GOTO;
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _productVehicleRepository.AddAsync(entities);
                    scope.SaveChanges();
                }

            RETURN_GOTO:
                return ResponseResult.Ok();
            }
        }
    }
}
