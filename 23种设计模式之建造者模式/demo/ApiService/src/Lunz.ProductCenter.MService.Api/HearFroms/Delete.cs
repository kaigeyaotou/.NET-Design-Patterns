using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.Models.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.HearFroms
{
    public class Delete
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// 要删除从哪里听说数据的 Id。
            /// </summary>
            [Required]
            public Guid? Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IHearFromRepository _hearFromRepository;

            public Handler(
                IHearFromRepository hearFromRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _hearFromRepository = hearFromRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!request.Id.HasValue || request.Id.Value == Guid.Empty)
                {
                    return ResponseResult.Error("Id 不能为空。");
                }

                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _hearFromRepository.DeleteAsync(request.Id.Value);

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
