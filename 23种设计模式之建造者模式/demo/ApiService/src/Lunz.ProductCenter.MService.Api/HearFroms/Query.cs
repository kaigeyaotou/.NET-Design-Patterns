﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.Models.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.HearFroms
{
    public class Query
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            public Command()
            {
                Sort = new List<PagingSort>();
                Filter = new QueryGroup() { Op = "and" };
            }

            /// <summary>
            /// 过滤条件
            /// </summary>
            public QueryGroup Filter { get; set; }

            /// <summary>
            /// 页号，如 1
            /// </summary>
            public int? PageIndex { get; set; }

            /// <summary>
            /// 每页行数，如 10
            /// </summary>
            public int? PageSize { get; set; }

            /// <summary>
            /// 排序规则
            /// </summary>
            public List<PagingSort> Sort { get; private set; }
        }

        /// <summary>
        /// 从哪里听说数据列表
        /// </summary>
        public class Response : IPaginationModel<HearFromDetails>
        {
            public Response(IEnumerable<HearFromDetails> data, long count, int? pageIndex, int? pageSize)
            {
                Data = data;
                Count = count;
                PageIndex = pageIndex;
                PageSize = pageSize;
            }

            /// <summary>
            /// 总行数
            /// </summary>
            public long Count { get; }

            /// <summary>
            /// 当前页号
            /// </summary>
            public int? PageIndex { get; }

            /// <summary>
            /// 每页行数
            /// </summary>
            public int? PageSize { get; }

            /// <summary>
            /// 从哪里听说列表
            /// </summary>
            public IEnumerable<HearFromDetails> Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
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

            public async Task<ResponseResult<Response>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var filter = request.Filter.ToSql<HearFromDetails>();
                    var result = await _hearFromRepository.QueryAsync<HearFromDetails>(
                        () => filter.ToTuble(), request.PageIndex, request.PageSize, request.Sort.ToSql(), true);

                    return ResponseResult<Response>.Ok(
                        new Response(result.Data, result.Count, request.PageIndex, request.PageSize));
                }
            }
        }
    }
}
