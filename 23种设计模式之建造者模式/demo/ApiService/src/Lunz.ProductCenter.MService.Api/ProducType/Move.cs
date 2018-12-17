using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProducType.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.ProducType.Api
{
    public class Move
    {
        public class Position : IRequest<ResponseResult>
        {
            /// <summary>
            /// 移动后的位置
            /// </summary>
            public int MoveAfterPosition { get; set; }

            /// <summary>
            /// 当前Id
            /// </summary>
            public string MoveId { get; set; }

            /// <summary>
            /// 父级Id
            /// </summary>
            public string ParentId { get; set; }
        }

        public class Handler : IRequestHandler<Position, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMovePositionRepository _movePositionRepository;

            public Handler(
                IMovePositionRepository movePositionRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _movePositionRepository = movePositionRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Position request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var result = await _movePositionRepository
                        .MovePosition<QueryStack.Models.ProducType>(
                        request.MoveAfterPosition, request.MoveId, request.ParentId, "tb_productype");

                    scope.SaveChanges();

                    if (result <= 0)
                    {
                        return ResponseResult.Error($"同级移动失败，请重试。");
                    }
                }

                return ResponseResult.Ok();
            }
        }
    }
}