using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.ApiService.Api.HearFroms;
using Lunz.ProductCenter.ApiService.Models.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.ReferenceData
{
    [Route("api/v1/reference-data")]
    public class ReferenceDataController : MService.Api.ControllerBase
    {
        public ReferenceDataController(IMediator mediator, ILogger<ReferenceDataController> logger)
            : base(mediator, logger)
        {
        }

        [Route("/api/v1/reference-data")]
        [HttpPost]
        [SwaggerTag("基础数据")]
        public IActionResult Index()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取从哪里听说全部数据集合。
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>返回从哪里听说数据集合。</returns>
        [Route("hear-froms")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HearFromDetails>), 200)]
        [SwaggerTag("基础数据")]
        public async Task<IEnumerable<HearFromDetails>> Get(HearFrom.Command query)
        {
            var result = await Mediator.Send(query);

            return result;
        }
    }
}