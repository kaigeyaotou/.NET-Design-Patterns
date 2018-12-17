using System;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.BasicData
{
    public class UpdateOption
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 主键
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// 选项
            /// </summary>
            public string NameValue { get; set; }

            /// <summary>
            /// 父级id
            /// </summary>
            public string ParentId { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IBasicDataRepository _basicDataRepository;

            public Handler(
                IBasicDataRepository basicDataRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _basicDataRepository = basicDataRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.NameValue))
                {
                    return ResponseResult.Error("名称不能为空.");
                }

                if (string.IsNullOrWhiteSpace(request.ParentId))
                {
                    return ResponseResult.Error("父级不能为空.");
                }

                request.NameValue = request.NameValue.Trim();
                request.ParentId = request.ParentId.Trim();

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    bool hasName = await _basicDataRepository.HasSameChildrenNameAsync(request.ParentId, request.NameValue, request.Id);
                    if (hasName)
                    {
                        return ResponseResult.Error($"名称: {request.NameValue} 已存在.");
                    }
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _basicDataRepository.UpdateOptionAsync(
                        new QueryStack.Models.BasicData()
                    {
                        Id = request.Id,
                        ParentId = request.ParentId,
                        Name = request.NameValue,
                    }, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
