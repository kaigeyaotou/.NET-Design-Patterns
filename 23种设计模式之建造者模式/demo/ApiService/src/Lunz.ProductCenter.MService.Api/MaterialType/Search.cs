using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProducType.Api;
using Lunz.ProductCenter.Common;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.MaterialType
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
            public Response(IEnumerable<QueryStack.Models.MaterialType> data)
            {
                Data = data;
            }

            /// <summary>
            /// 类型树.
            /// </summary>
            public IEnumerable<QueryStack.Models.MaterialType> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
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

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var treeInfos = await _materialTypeRepository.SearchByTypeName<QueryStack.Models.MaterialType>(request.TypeName);

                    var allInfos = await _materialTypeRepository.QueryAsync();

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

            public List<QueryStack.Models.MaterialType> GetTreesByParentId(string parentId, IEnumerable<QueryStack.Models.MaterialType> allInfos)
            {
                List<QueryStack.Models.MaterialType> models = new List<QueryStack.Models.MaterialType>();
                var infos = allInfos.Where(x => x.ParentId == parentId);
                foreach (var item in infos)
                {
                    models.Add(item);
                    models.AddRange(GetTreesByParentId(item.Id, allInfos));
                }

                return models;
            }

            public class Compare : IEqualityComparer<QueryStack.Models.MaterialType>
            {
                public bool Equals(QueryStack.Models.MaterialType x, QueryStack.Models.MaterialType y)
                {
                    return x.Id == y.Id;
                }

                public int GetHashCode(QueryStack.Models.MaterialType obj)
                {
                    return obj.Id.GetHashCode();
                }
            }
        }
    }
}
