using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.PropertiesOption
{
    public class UpOrDown
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 关联属性Id
            /// </summary>
            public string PropId { get; set; }

            /// <summary>
            /// 向上者的主键
            /// </summary>
            public string UpDataId { get; set; }

            /// <summary>
            /// 向上者的排序
            /// </summary>
            public int UpSequenceValue { get; set; }

            /// <summary>
            /// 下降者主键
            /// </summary>
            public string DownDataId { get; set; }

            /// <summary>
            /// 下降者排序值
            /// </summary>
            public int DownSequenceValue { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IPropertiesOptionResponsitory _propertiesOptionResponsitory;

            public Handler(
                IPropertiesOptionResponsitory propertyRespository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _propertiesOptionResponsitory = propertyRespository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.PropId))
                {
                    return ResponseResult.Error($"关联属性Id不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.UpDataId) && string.IsNullOrWhiteSpace(request.DownDataId))
                {
                    return ResponseResult.Error($"请选择 ↑ 或者 ↓ 按钮.");
                }

                // 有且只有一条数据 或 排序值相同
                if (string.IsNullOrWhiteSpace(request.UpDataId)
                    || string.IsNullOrWhiteSpace(request.DownDataId)
                    || request.UpDataId == request.DownDataId)
                {
                    return ResponseResult.Ok();
                }

                request.PropId = request.PropId.Trim();
                request.UpDataId = request.UpDataId.Trim();
                request.DownDataId = request.DownDataId.Trim();

                // 交换排序值
                Dictionary<string, int> changeSequence = new Dictionary<string, int>();
                changeSequence.Add(request.UpDataId, request.DownSequenceValue);
                changeSequence.Add(request.DownDataId, request.UpSequenceValue);

                foreach (var dic in changeSequence)
                {
                    using (var scope = _databaseScopeFactory.CreateWithTransaction())
                    {
                        await _propertiesOptionResponsitory.UpOrDownAsync(dic.Key, dic.Value, request.UserId);
                        scope.SaveChanges();
                    }
                }

                return ResponseResult.Ok();
            }
        }
    }
}
