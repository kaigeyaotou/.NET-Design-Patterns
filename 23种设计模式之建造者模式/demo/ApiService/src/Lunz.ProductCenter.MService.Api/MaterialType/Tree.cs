using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.MService.Api.MaterialType
{
    public class Tree
    {
        public class Command : IRequest<IEnumerable<MaterialTypeTreeDetails>>
        {
        }

        public class Root : IRequest<IEnumerable<MaterialTypeDetails>>
        {
        }

        public class Child : IRequest<IEnumerable<MaterialTypeDetails>>
        {
            /// <summary>
            /// 要获取子级的物料类型 Id
            /// </summary>
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, IEnumerable<MaterialTypeTreeDetails>>,
            IRequestHandler<Root, IEnumerable<MaterialTypeDetails>>,
            IRequestHandler<Child, IEnumerable<MaterialTypeDetails>>
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

            public async Task<IEnumerable<MaterialTypeTreeDetails>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var typeList = await _materialTypeRepository.QueryAsync();

                    var fmodel = typeList.Where(o => o.ParentId == null).OrderBy(o => o.SortOrder).ToList();
                    var outputmodel = new List<MaterialTypeTreeDetails>();

                    ShowTree(fmodel, typeList.ToList(), ref outputmodel);

                    return outputmodel;
                }
            }

            public async Task<IEnumerable<MaterialTypeDetails>> Handle(Root request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    return await _materialTypeRepository.RootTypesAsync<MaterialTypeDetails>();
                }
            }

            public async Task<IEnumerable<MaterialTypeDetails>> Handle(Child request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return new List<MaterialTypeDetails>();
                }

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    return await _materialTypeRepository.ChildTypesAsync<MaterialTypeDetails>(request.Id);
                }
            }

            private void ShowTree(
                List<QueryStack.Models.MaterialType> fmodel,
                List<QueryStack.Models.MaterialType> totalmodel,
                ref List<MaterialTypeTreeDetails> outputmodel)
            {
                foreach (var foo in fmodel)
                {
                    var cm = totalmodel.Where(o => o.ParentId == foo.Id).OrderBy(x => x.SortOrder).ToList();
                    var c = new MaterialTypeTreeDetails
                    {
                        Id = foo.Id,
                        ParentId = foo.ParentId,
                        TypeCode = foo.TypeCode,
                        TypeName = foo.TypeName,
                        LevelCode = foo.LevelCode.Value,
                        HasChild = cm.Any(),
                    };
                    var newout = new List<MaterialTypeTreeDetails>();
                    if (c.HasChild)
                    {
                        ShowTree(cm, totalmodel, ref newout);
                        c.TreeChildren = newout;
                        outputmodel.Add(c);
                    }
                    else
                    {
                        c.TreeChildren = new List<MaterialTypeTreeDetails>();
                        outputmodel.Add(c);
                    }
                }
            }
        }
    }
}
