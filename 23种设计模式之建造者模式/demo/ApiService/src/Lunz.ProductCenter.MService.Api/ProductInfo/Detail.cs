﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Data.Extensions.Query;
using Lunz.Data.Extensions.Sort;
using Lunz.Data.Extensions.Sql;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.ProductInfo.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Core.Models;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.ApiService.Api.ProductInfo
{
    public class Detail
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            /// <summary>
            /// 产品主键
            /// </summary>
           public string Id { get; set; }
        }

        /// <summary>
        /// 详细信息
        /// </summary>
        public class Response
        {
            public Response(QueryStack.Models.ProductInfo data)
            {
                Data = data;
            }

            /// <summary>
            /// 详情
            /// </summary>
            public QueryStack.Models.ProductInfo Data { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IProductInfoRepository _productInfoRepository;

            public Handler(
                IProductInfoRepository productInfoRepository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger)
            {
                _productInfoRepository = productInfoRepository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
            }

            public async Task<ResponseResult<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var result = await _productInfoRepository.DetailAsync(request.Id);
                    return ResponseResult<Response>.Ok(new Response(result));
                }
            }
        }
    }
}
