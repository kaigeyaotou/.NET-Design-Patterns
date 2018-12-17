using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.ProductCenter.ApiService.Models.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ReferenceData
{
    public class HearFrom
    {
        public class Command : IRequest<IEnumerable<HearFromDetails>>
        {
        }

        public class Handler : IRequestHandler<Command, IEnumerable<HearFromDetails>>
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

            public async Task<IEnumerable<HearFromDetails>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var result = await _hearFromRepository.QueryAsync<HearFromDetails>(null, null, null, "Name", false);

                    return result.Data;
                }
            }
        }
    }
}
