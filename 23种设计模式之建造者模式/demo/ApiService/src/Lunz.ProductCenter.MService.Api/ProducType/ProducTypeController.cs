using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.ApiService.ProducType.Api;
using Lunz.ProductCenter.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.ProducType
{
    [Route("api/v1/ProducType")]
    public class ProducTypeController : Lunz.ProductCenter.MService.Api.ControllerBase
    {
        public ProducTypeController(IMediator mediator, ILogger<ProducTypeDetials> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 添加顶级类型
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("create/top")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品类型管理")]
        public async Task<IActionResult> Top([FromBody]CreateTheTop.Command command)
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
        /// 添加同级分类
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品类型管理")]
        public async Task<IActionResult> Post([FromBody]CreateTheSame.Command command)
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
        /// 添加子级分类
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("create/child")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品类型管理")]
        public async Task<IActionResult> Child([FromBody]CreateTheChild.Command command)
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
        /// 更新
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPut]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品类型管理")]
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
        [SwaggerTag("产品类型管理")]
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
        /// 产品类型树
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回类型树。</returns>
        [Route("tree/{parentId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProducTypeDetials>), 200)]
        [SwaggerTag("产品类型管理")]
        public async Task<IEnumerable<ProducTypeDetials>> Get(Tree.Command command)
        {
            var result = await Mediator.Send(command);
            return result.Data.Data;
        }

        /// <summary>
        /// 产品类型导航
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回类型导航。</returns>
        [Route("navigation/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [SwaggerTag("产品类型管理")]
        public async Task<string> Get(Navigation.Command command)
        {
            var result = await Mediator.Send(command);
            return result.Data.Data;
        }

        /// <summary>
        /// 产品类别树同级移动
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("move/position/{moveafterposition}/{moveid}/{parentid}")]
        [HttpGet]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品类型管理")]
        public async Task<IActionResult> Move(Move.Position command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await Mediator.Send(command);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        /// <summary>
        /// 产品类别查询
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回类型树。</returns>
        [Route("searchtree/{parentId}/{typename}/{level}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProducTypeDetials>), 200)]
        [SwaggerTag("产品类型管理")]
        public async Task<IEnumerable<ProducTypeDetials>> Get(Search.Command command)
        {
            var result = await Mediator.Send(command);
            return result.Data.Data;
        }
    }
}
