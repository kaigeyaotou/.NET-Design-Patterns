using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.AspNetCore;
using Lunz.ProductCenter.MService.Api.ProductDetail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.ApiService.Api.ProductDetail
{
    [Route("api/v1/productdetail")]
    public class ProductDetailController : Lunz.ProductCenter.MService.Api.ControllerBase
    {
        public ProductDetailController(IMediator mediator, ILogger<ApiService.ProductDetail.Api.ProductMaterialDetails> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 产品-物料关系列表
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>列表数据。</returns>
        [Route("~/api/v1/productdetails")]
        [HttpGet]
        [ProducesResponseType(typeof(Query.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("[产品vs物料]关联管理")]
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
        [SwaggerTag("[产品vs物料]关联管理")]
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
        [Route("")]
        [HttpDelete]
        [ProducesResponseType(204)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("[产品vs物料]关联管理")]
        public async Task<IActionResult> Delete(Delete.Command command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (command.Ids == null || command.Ids.Length <= 0)
            {
                return BadRequest();
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
