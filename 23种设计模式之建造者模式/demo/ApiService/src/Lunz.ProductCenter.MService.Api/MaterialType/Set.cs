using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.MaterialType
{
    public class Set
    {
        public class Status : IRequest<ResponseResult>
        {
            /// <summary>
            /// 要设置状态的物料类型属性 Id
            /// </summary>
            [Required]
            public string Id { get; set; }

            /// <summary>
            /// 是否禁用
            /// </summary>
            [Required]
            public bool IsDisable { get; set; }

            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Necessary : IRequest<ResponseResult>
        {
            /// <summary>
            /// 要设置是否必填的物料类型属性 Id
            /// </summary>
            [Required]
            public string Id { get; set; }

            /// <summary>
            /// 是否必填
            /// </summary>
            [Required]
            public bool IsNecessary { get; set; }

            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Status, ResponseResult>, IRequestHandler<Necessary, ResponseResult>
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

            public async Task<ResponseResult> Handle(Status request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _materialTypeRepository.SetPropertyStatus(
                        request.Id,
                        request.IsDisable,
                        string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);

                    scope.SaveChanges();

                    return ResponseResult.Ok();
                }
            }

            public async Task<ResponseResult> Handle(Necessary request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _materialTypeRepository.SetPropertyNecessary(
                        request.Id,
                        request.IsNecessary,
                        string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);

                    scope.SaveChanges();

                    return ResponseResult.Ok();
                }
            }
        }
    }
}
