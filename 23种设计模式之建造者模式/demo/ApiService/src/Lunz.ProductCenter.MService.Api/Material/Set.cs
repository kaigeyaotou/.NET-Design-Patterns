using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Set
    {
        public class Enable : IRequest<ResponseResult>
        {
            /// <summary>
            /// 要启用的物料 Ids
            /// </summary>
            public string[] Ids { get; set; }

            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Disable : IRequest<ResponseResult>
        {
            /// <summary>
            /// 要禁用的物料 Ids
            /// </summary>
            public string[] Ids { get; set; }

            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Published : IRequest<ResponseResult>
        {
            /// <summary>
            /// 要发布的物料 Id
            /// </summary>
            public string[] Ids { get; set; }

            /// <summary>
            /// 当前登录用户 Id
            /// </summary>
            public string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Enable, ResponseResult>, IRequestHandler<Disable, ResponseResult>,
            IRequestHandler<Published, ResponseResult>
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

            public async Task<ResponseResult> Handle(Enable request, CancellationToken cancellationToken)
            {
                request.Ids = request.Ids.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                if (request.Ids.Length > 0)
                {
                    using (var scope = _databaseScopeFactory.CreateWithTransaction())
                    {
                        await _materialRepository.SetStatusAsync(
                            request.Ids,
                            false,
                            string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);
                        scope.SaveChanges();
                    }
                }

                return ResponseResult.Ok();
            }

            public async Task<ResponseResult> Handle(Disable request, CancellationToken cancellationToken)
            {
                request.Ids = request.Ids.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                if (request.Ids.Length > 0)
                {
                    using (var scope = _databaseScopeFactory.CreateWithTransaction())
                    {
                        await _materialRepository.SetStatusAsync(
                            request.Ids,
                            true,
                            string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);
                        scope.SaveChanges();
                    }
                }

                return ResponseResult.Ok();
            }

            public async Task<ResponseResult> Handle(Published request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _materialRepository.PublishedAsync(
                        request.Ids,
                        string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId);
                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
