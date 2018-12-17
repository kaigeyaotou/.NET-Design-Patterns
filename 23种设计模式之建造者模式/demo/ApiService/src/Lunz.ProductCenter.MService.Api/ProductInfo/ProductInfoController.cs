using System.Threading.Tasks;
using Lunz.ProductCenter.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.ProductInfo
{
    [Route("api/v1/product")]
    public class ProductInfoController : Lunz.ProductCenter.MService.Api.ControllerBase
    {
        public ProductInfoController(IMediator mediator, ILogger<ApiService.ProductInfo.Api.ProductInfoDetails> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>列表数据。</returns>
        [Route("~/api/v1/products")]
        [HttpGet]
        [ProducesResponseType(typeof(Query.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品管理")]
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
        /// 详情
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>列表数据。</returns>
        [Route("detail/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(Detail.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品管理")]
        public async Task<Detail.Response> Detail(Detail.Command query)
        {
            var result = await Mediator.Send(query);

            return result.Data;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>新增产品Id</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(typeof(IActionResult), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品管理")]
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

            return Content(result.Data);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPut]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品管理")]
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
        /// 删除
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品管理")]
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

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("enable")]
        [HttpPatch]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品管理")]
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
        [SwaggerTag("产品管理")]
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
        /// 发布
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("release")]
        [HttpPatch]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品管理")]
        public async Task<IActionResult> Release([FromBody]Release.Command command)
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
