using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProducType.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProducType
{
    public class Navigation
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            public string Id { get; set; }
        }

        /// <summary>
        /// 类型树
        /// </summary>
        public class Response
        {
            public Response(string data)
            {
                Data = data;
            }

            /// <summary>
            /// 类型树.
            /// </summary>
            public string Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProducTypeRepository _producTypeRepository;

            public Handler(
                IProducTypeRepository producTypeRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _producTypeRepository = producTypeRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var allData = await _producTypeRepository.QueryAsync<ProducTypeDetials>("-1");

                    allData = allData.OrderBy(q => q.ParentId).ToList();
                    var thisTypeData = allData.Where(q => q.Id == request.Id).FirstOrDefault();

                    string nvg = GetNvg(allData, thisTypeData?.ParentId, thisTypeData?.TypeName);

                    return ResponseResult<Response>.Ok(new Response(nvg));
                }
            }

            private string GetNvg(IEnumerable<ProducTypeDetials> source, string parentId, string nvg)
            {
                var parent = source.Where(q => q.Id == parentId).FirstOrDefault();

                if (parent != null)
                {
                    nvg = $"{parent.TypeName} > {nvg}";
                    nvg = GetNvg(source, parent.ParentId, nvg);
                    return nvg;
                }

                return nvg;
            }
        }
    }
}
