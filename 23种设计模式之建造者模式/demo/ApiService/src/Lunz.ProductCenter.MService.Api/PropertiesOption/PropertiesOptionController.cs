using System.Threading.Tasks;
using Lunz.ProductCenter.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.PropertiesOption
{
    [Route("api/v1/PropertiesOption")]
    public class PropertiesOptionController : Lunz.ProductCenter.MService.Api.ControllerBase
    {
        public PropertiesOptionController(IMediator mediator, ILogger<ApiService.PropertiesOption.Api.PropertiesOptionDetials> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性选项管理")]
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
        /// 启用
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("enable")]
        [HttpPatch]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性选项管理")]
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
        [SwaggerTag("属性选项管理")]
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
        /// 上/下
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("updown")]
        [HttpPatch]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性选项管理")]
        public async Task<IActionResult> UpOrDown([FromBody]UpOrDown.Command command)
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
        /// 属性选项列表
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>属性选项列表数据。</returns>
        [Route("~/api/v1/propertiesoptions/{propId}")]
        [HttpGet]
        [ProducesResponseType(typeof(Tree.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("属性选项管理")]
        public async Task<ActionResult<Tree.Response>> Get(Tree.Command query)
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
