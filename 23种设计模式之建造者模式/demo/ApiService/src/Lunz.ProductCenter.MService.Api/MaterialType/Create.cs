using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.Core.Models.BasicProducts;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.MaterialType
{
    public class Create
    {
        public class Command : MaterialTypeDetails, IRequest<ResponseResult>
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
            private readonly IMaterialTypeRepository _materialTypeRepository;

            public Handler(
                IMaterialTypeRepository materialTypeRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _materialTypeRepository = materialTypeRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                request.TypeName = request.TypeName.Trim();
                if (string.IsNullOrWhiteSpace(request.TypeName))
                {
                    return ResponseResult.Error("物料类型名称不能为空。");
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var typeList = await _materialTypeRepository.AllAsync();

                    var code = string.Empty;
                    if (string.IsNullOrWhiteSpace(request.ParentId))
                    {
                        if (typeList.Any(x => x.ParentId == null && x.Deleted == false && x.TypeName == request.TypeName))
                        {
                            return ResponseResult.Error($"物料类型名称 {request.TypeName} 已存在。");
                        }

                        request.ParentId = null;

                        var model = typeList.Where(x => x.ParentId == null).OrderByDescending(x => x.TypeCode).FirstOrDefault();
                        if (model == null)
                        {
                            code = "01";
                        }
                        else
                        {
                            code = (int.Parse(model.TypeCode) + 1).ToString().PadLeft(model.TypeCode.Length, '0');
                        }
                    }
                    else
                    {
                        if (typeList.Any(x => x.ParentId == request.ParentId && x.Deleted == false && x.TypeName == request.TypeName))
                        {
                            return ResponseResult.Error($"物料类型名称 {request.TypeName} 已存在。");
                        }

                        var parent = typeList.SingleOrDefault(x => x.Deleted == false && x.Id == request.ParentId);
                        if (parent == null)
                        {
                            return ResponseResult.Error($"未找到 Id={request.ParentId} 父级物料类型。");
                        }

                        var model = typeList.Where(x => x.ParentId == request.ParentId).OrderByDescending(x => x.TypeCode).FirstOrDefault();
                        if (model == null)
                        {
                            code = parent.TypeCode + "01";
                        }
                        else
                        {
                            code = (int.Parse(model.TypeCode) + 1).ToString().PadLeft(model.TypeCode.Length, '0');
                        }
                    }

                    var maxSortOrder = 0;
                    if (typeList.Any())
                    {
                        maxSortOrder = typeList.Max(x => x.SortOrder.Value);
                    }

                    await _materialTypeRepository.AddAsync(new QueryStack.Models.MaterialType()
                    {
                        ParentId = request.ParentId,
                        TypeCode = code,
                        TypeName = request.TypeName,
                        LevelCode = request.LevelCode,
                        SortOrder = maxSortOrder + 1,
                        CreatedById = string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId,
                    });

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
