using System;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.Models.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.HearFroms
{
    public class Update
    {
        public class Command : HearFromDetails, IRequest<ResponseResult>
        {
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
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    await _hearFromRepository.UpdateAsync(request.Id, new QueryStack.Models.HearFrom()
                    {
                        Id = request.Id,
                        Name = request.Name,
                    });

                    scope.SaveChanges();
                }

                return ResponseResult.Ok();
            }
        }
    }
}
