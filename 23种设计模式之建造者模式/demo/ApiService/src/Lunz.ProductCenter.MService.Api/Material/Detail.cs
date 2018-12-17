using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lunz.Data;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Detail
    {
        public class Command : IRequest<MaterialDetails>
        {
            /// <summary>
            /// 要获取物料数据的 Id
            /// </summary>
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, MaterialDetails>
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

            public async Task<MaterialDetails> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _materialRepository.FindAsync(request.Id);
                    if (result == null)
                    {
                        return null;
                    }

                    return Mapper.Map<MaterialDetails>(result);
                }
            }
        }
    }
}
