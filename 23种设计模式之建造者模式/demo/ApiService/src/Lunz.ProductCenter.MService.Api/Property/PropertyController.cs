using System.Threading.Tasks;
using Lunz.ProductCenter.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.Property
{
   [Route("api/v1/property")]
   public class PropertyController : Lunz.ProductCenter.MService.Api.ControllerBase
    {
        public PropertyController(IMediator mediator, ILogger<ApiService.Property.Api.PropertyDetails> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性管理")]
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
        /// 编辑属性
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPut]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性管理")]
        public async Task<IActionResult> Put([FromBody]Update.Command command)
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
        /// 启用
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("enable")]
        [HttpPatch]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性管理")]
        public async Task<IActionResult> Enable([FromBody]Enable.Command command)
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
        /// 禁用
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("disable")]
        [HttpPatch]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性管理")]
        public async Task<IActionResult> Disable([FromBody]Disable.Command command)
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
        /// 属性列表
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>属性列表数据。</returns>
        [Route("~/api/v1/properties")]
        [HttpGet]
        [ProducesResponseType(typeof(Query.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性管理")]
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
        /// 有效属性
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>有效属性列表。</returns>
        [Route("~/api/v1/real-properties")]
        [HttpGet]
        [ProducesResponseType(typeof(QueryReality.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性管理")]
        public async Task<ActionResult<QueryReality.Response>> Real(QueryReality.Command query)
        {
            var result = await Mediator.Send(query);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return result.Data;
        }
    }
}
