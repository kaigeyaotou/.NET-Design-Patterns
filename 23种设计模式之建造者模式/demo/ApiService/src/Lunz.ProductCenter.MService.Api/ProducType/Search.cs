using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProducType.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProducType
{
    public class Search
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            public string ParentId { get; set; }

            public string TypeName { get; set; }

            public int Level { get; set; }
        }

        /// <summary>
        /// 类型树
        /// </summary>
        public class Response
        {
            public Response(IEnumerable<ProducTypeDetials> data)
            {
                Data = data;
            }

            /// <summary>
            /// 类型树.
            /// </summary>
            public IEnumerable<ProducTypeDetials> Data { get; }
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
                    var treeInfos = await _producTypeRepository.SearchByTypeName<ProducTypeDetials>(request.TypeName);

                    var allInfos = await _producTypeRepository.QueryAsync<ProducTypeDetials>("-1");

                    var result = TreeLikeSearch.GetTreesByName(treeInfos, allInfos, request.Level)
                       .Where(x => x.ParentId == (request.ParentId == "-1" ? null : request.ParentId))
                       .Distinct(new Compare())
                       .OrderBy(x => x.SortOrder).ToList();

                    foreach (var r in result)
                    {
                        r.HasChildren = false;
                        var res = GetTreesByParentId(r.Id, allInfos);
                        if (res.Where(x => x.TypeName.Contains(request.TypeName)).Any())
                        {
                            r.HasChildren = true;
                        }
                    }

                    return ResponseResult<Response>.Ok(new Response(result));
                }
            }

            public List<ProducTypeDetials> GetTreesByParentId(string parentId, IEnumerable<ProducTypeDetials> allInfos)
            {
                List<ProducTypeDetials> models = new List<ProducTypeDetials>();
                var infos = allInfos.Where(x => x.ParentId == parentId);
                foreach (var item in infos)
                {
                    models.Add(item);
                    models.AddRange(GetTreesByParentId(item.Id, allInfos));
                }

                return models;
            }

            public class Compare : IEqualityComparer<ProducTypeDetials>
            {
                public bool Equals(ProducTypeDetials x, ProducTypeDetials y)
                {
                    return x.Id == y.Id;
                }

                public int GetHashCode(ProducTypeDetials obj)
                {
                    return obj.Id.GetHashCode();
                }
            }
        }
    }
}