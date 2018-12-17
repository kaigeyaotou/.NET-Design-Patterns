using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.AspNetCore;
using Lunz.ProductCenter.MService.QueryStack.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.MService.Api.PictureResource
{
   [Route("api/v1/pictureResource")]
   public class PictureResourceController : MService.Api.ControllerBase
    {
        public PictureResourceController(IMediator mediator, ILogger<PictureResourceController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 获取上传图片资源
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>返回上传图片资源列表</returns>
        [Route("~/api/v1/pictureResources/{productId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductPicture>), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品上传图片管理")]
        public async Task<IEnumerable<ProductPicture>> Get(Query.Command query)
        {
            var result = await Mediator.Send(query);

            return result;
        }

        /// <summary>
        /// 添加图片资源
        /// </summary>
        /// <param name="command">上传图片信息集合</param>
        /// <returns>无返回值</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品上传图片管理")]
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
        /// 编辑图片资源
        /// </summary>
        /// <param name="command">上传图片信息集合</param>
        /// <returns>无返回值。</returns>
        [Route("")]
        [HttpPut]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品上传图片管理")]
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
        /// 删除图片资源
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值</returns>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("产品上传图片管理")]
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
