using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.AspNetCore;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.MService.Api.MaterialType
{
    [Route("api/v1/materialType")]
    public class MaterialTypeController : ControllerBase
    {
        public MaterialTypeController(IMediator mediator, ILogger<MaterialTypeController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 获取物料类型树。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回物料类型树。</returns>
        [Route("tree")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MaterialTypeTreeDetails>), 200)]
        [SwaggerTag("物料类型")]
        public async Task<IEnumerable<MaterialTypeTreeDetails>> Get(Tree.Command command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        /// <summary>
        /// 获取顶级物料类型数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回顶级物料类型数据。</returns>
        [Route("root")]
        [HttpGet]
        [ProducesResponseType(typeof(MaterialTypeDetails), 200)]
        [SwaggerTag("物料类型")]
        public async Task<IEnumerable<MaterialTypeDetails>> Get(Tree.Root command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        /// <summary>
        /// 获取子级物料类型数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回子级物料类型数据。</returns>
        [Route("child/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(MaterialTypeDetails), 200)]
        [SwaggerTag("物料类型")]
        public async Task<IEnumerable<MaterialTypeDetails>> Get(Tree.Child command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        /// <summary>
        /// 获取物料类型的属性列表。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回物料类型的属性列表。</returns>
        [Route("properties/{materialTypeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MaterialTypePropertyDetails>), 200)]
        [SwaggerTag("物料类型")]
        public async Task<IEnumerable<MaterialTypePropertyDetails>> Get(Properties.Command command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        /// <summary>
        /// 获取物料类型的属性列表(含属性值集合)。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回物料类型的属性列表。</returns>
        [Route("propertylist/{materialTypeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MaterialTypeProperty>), 200)]
        [SwaggerTag("物料类型")]
        public async Task<IEnumerable<MaterialTypeProperty>> Get(Properties.List command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        /// <summary>
        /// 创建物料类型。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料类型")]
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
        /// 添加物料类型属性。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("property")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料类型")]
        public async Task<IActionResult> Post([FromBody]AddProperties.Command command)
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
        /// 设置物料类型属性的状态。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("property/setStatus")]
        [HttpPatch]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料类型")]
        public async Task<IActionResult> Patch(Set.Status command)
        {
            if (string.IsNullOrWhiteSpace(command.Id))
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

        /// <summary>
        /// 设置物料类型属性为必填/选填。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("property/setNecessary")]
        [HttpPatch]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料类型")]
        public async Task<IActionResult> Patch(Set.Necessary command)
        {
            if (string.IsNullOrWhiteSpace(command.Id))
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

        /// <summary>
        /// 物料类别树同级移动
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
        [ProducesResponseType(typeof(IEnumerable<QueryStack.Models.MaterialType>), 200)]
        [SwaggerTag("产品类型管理")]
        public async Task<IEnumerable<QueryStack.Models.MaterialType>> Get(Search.Command command)
        {
            var result = await Mediator.Send(command);
            return result.Data.Data;
        }
    }
}
