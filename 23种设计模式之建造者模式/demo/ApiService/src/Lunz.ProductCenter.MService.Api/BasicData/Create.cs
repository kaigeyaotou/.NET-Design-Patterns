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
    public class Create
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 顶级 or 选项
            /// </summary>
            public string NameValue { get; set; }

            /// <summary>
            /// 顶级：'' ；选项：父级id
            /// </summary>
            public string ParentId { get; set; }

            /// <summary>
            /// 父级编码
            /// </summary>
            public string ParentCode { get; set; }

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

                request.NameValue = request.NameValue.Trim();

                string maxCode = string.Empty;

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    bool hasName = await _basicDataRepository.HasSameChildrenNameAsync(request.ParentId, request.NameValue);
                    if (hasName)
                    {
                        return ResponseResult.Error($"名称: {request.NameValue} 已存在.");
                    }

                    maxCode = await _basicDataRepository.GetMaxCodeValue(request.ParentId);
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    int maxSortValue = await _basicDataRepository.GetMaxSortOrderValue(request.ParentId);

                    await _basicDataRepository.AddAsync(
                        new QueryStack.Models.BasicData()
                        {
                            Code = Common.FormatDataCode.GetNewMaxCode(maxCode, request.ParentCode),
                            Name = request.NameValue,
                            ParentId = request.ParentId,
                            Enabled = true,
                            SortOrder = maxSortValue + 1,
                            CreatedAt = DateTime.Now,
                        }, request.UserId);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
