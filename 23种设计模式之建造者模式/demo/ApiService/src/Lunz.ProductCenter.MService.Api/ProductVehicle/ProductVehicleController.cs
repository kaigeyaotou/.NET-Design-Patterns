using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.ApiService.QueryStack.Models;
using Lunz.ProductCenter.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.ProductVehicle
{
    [Route("api/v1/productvehicle")]
    public class ProductVehicleController : Lunz.ProductCenter.MService.Api.ControllerBase
    {
        public ProductVehicleController(IMediator mediator, ILogger<ApiService.ProductInfo.Api.ProductInfoDetails> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>列表数据。</returns>
        [Route("~/api/v1/productvehicles")]
        [HttpGet]
        [ProducesResponseType(typeof(Query.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("车型管理")]
        public async Task<ActionResult<Query.Response>> Get(Query.Command query)
        {
            var result = await Mediator.Send(query);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return result.Data;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(string), 201)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("车型管理")]
        public async Task<IActionResult> Post([FromBody]Create.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetails = Request.UserDetails();
            if (userDetails != null && userDetails.Id.HasValue)
            {
                command.UserId = userDetails.Id.ToString();
            }

            var result = await Mediator.Send(command);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("车型管理")]
        public async Task<IActionResult> Delete(Delete.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetails = Request.UserDetails();
            if (userDetails != null && userDetails.Id.HasValue)
            {
                command.UserId = userDetails.Id.ToString();
            }

            var result = await Mediator.Send(command);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }
    }
}
