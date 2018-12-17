using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.PictureResource
{
    public class Query
    {
        /// <summary>
        /// 上传产品图片列表
        /// </summary>
        public class Command : IRequest<IEnumerable<ProductPicture>>
        {
            /// <summary>
            /// 产品Id
            /// </summary>
            public string ProductId { get; set; }
        }

        public class Handler : IRequestHandler<Command, IEnumerable<ProductPicture>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IPictureResourceRepository _repository;

            public Handler(
                IPictureResourceRepository repository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _repository = repository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<IEnumerable<ProductPicture>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _repository.FindPictureResourcesAsync<ProductPicture>(request.ProductId);

                    return result;
                }
            }
        }
    }
}
