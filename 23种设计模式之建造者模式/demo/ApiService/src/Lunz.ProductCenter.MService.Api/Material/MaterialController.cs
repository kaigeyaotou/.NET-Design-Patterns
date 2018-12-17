using System.Collections.Generic;
using System.Threading.Tasks;
using Lunz.ProductCenter.AspNetCore;
using Lunz.ProductCenter.MService.Models.Api;
using Lunz.ProductCenter.MService.QueryStack.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Lunz.ProductCenter.MService.Api.Material
{
    [Route("api/v1/material")]
    public class MaterialController : ControllerBase
    {
        public MaterialController(IMediator mediator, ILogger<MaterialController> logger)
            : base(mediator, logger)
        {
        }

        /// <summary>
        /// 获取物料数据列表。
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>返回物料数据列表。</returns>
        [Route("~/api/v1/materials")]
        [HttpGet]
        [ProducesResponseType(typeof(Query.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
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
        /// 获取物料数据列表(产品模块使用)。
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>返回物料数据列表。</returns>
        [Route("~/api/v1/multi-trade-materials")]
        [HttpGet]
        [ProducesResponseType(typeof(QueryForProduct.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<ActionResult<QueryForProduct.Response>> Get(QueryForProduct.Command query)
        {
            var result = await Mediator.Send(query);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return result.Data;
        }

        /// <summary>
        /// 获取物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回物料数据。</returns>
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(MaterialDetails), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerResponse(404, null, Description = "参数 Id 无效。")]
        [SwaggerTag("物料")]
        public async Task<ActionResult<MaterialDetails>> Get(Detail.Command command)
        {
            if (string.IsNullOrWhiteSpace(command.Id))
            {
                return BadRequest();
            }

            var result = await Mediator.Send(command);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// 获取字典表数据。
        /// </summary>
        /// <param name="query">参数</param>
        /// <returns>返回字典表数据。</returns>
        [Route("dictionary/{needAll}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DataDictionary>), 200)]
        [SwaggerTag("物料")]
        public async Task<IEnumerable<DataDictionary>> Get(Dictionary.Command query)
        {
            var result = await Mediator.Send(query);
            return result;
        }

        /// <summary>
        /// 获取物料的自定义属性列表。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>返回物料的自定义属性列表。</returns>
        [Route("properties/{materialId}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MaterialTypeProperty>), 200)]
        [SwaggerTag("物料")]
        public async Task<IEnumerable<MaterialTypeProperty>> Get(Properties.Command command)
        {
            var result = await Mediator.Send(command);
            return result;
        }

        /// <summary>
        /// 添加物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("")]
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
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
        /// 编辑物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("")]
        [HttpPut]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
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
        /// 发布后编辑物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("")]
        [HttpPatch]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> Patch([FromBody]Edit.Command command)
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
        /// 删除物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> Delete(Delete.Command command)
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
        /// 启用物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("setEnable")]
        [HttpPatch]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> Patch([FromBody]Set.Enable command)
        {
            if (command.Ids == null || command.Ids.Length == 0)
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
        /// 禁用物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("setDisable")]
        [HttpPatch]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> Patch([FromBody]Set.Disable command)
        {
            if (command.Ids == null || command.Ids.Length == 0)
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
        /// 发布物料数据。
        /// </summary>
        /// <param name="command">参数</param>
        /// <returns>无返回值。</returns>
        [Route("published")]
        [HttpPatch]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> Patch([FromBody]Set.Published command)
        {
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

        /// <summary>
        /// 导出物料和物料的所有属性(基本属性和自定义属性)
        /// </summary>
        /// <param name="query">物料类型</param>
        /// <returns>返回运行结果</returns>
        [Route("export/materialWithAllProperties")]
        [HttpGet]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> ExportMaterialWithAllProperties(MaterialWithAllProperties.Command query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetails = Request.UserDetails();
            if (userDetails != null)
            {
                query.UserId = userDetails.Id.ToString();
                query.UserName = userDetails.Username;
            }

            var result = await Mediator.Send(query);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        /// <summary>
        /// 导出物料和物料的基本属性
        /// </summary>
        /// <param name="query">物料类型</param>
        /// <returns>返回运行结果</returns>
        [Route("export/materialWithBasicProperties")]
        [HttpGet]
        [ProducesResponseType(200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> ExportMaterialWithBasicProperties(MaterialWithBasicProperties.Command query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetails = Request.UserDetails();
            if (userDetails != null)
            {
                query.UserId = userDetails.Id.ToString();
                query.UserName = userDetails.Username;
            }

            var result = await Mediator.Send(query);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        /// <summary>
        /// 导出物料模板
        /// </summary>
        /// <param name="query">物料类型</param>
        /// <returns>返回文件地址</returns>
        [Route("export/materialTemplate")]
        [HttpGet]
        [ProducesResponseType(typeof(MaterialTemplate.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<ActionResult<MaterialTemplate.Response>> ExportMaterialTemplate(MaterialTemplate.Command query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestTemplate(ModelState);
            }

            var userDetails = Request.UserDetails();
            if (userDetails != null)
            {
                query.UserId = userDetails.Id.ToString();
                query.UserName = userDetails.Username;
            }

            var result = await Mediator.Send(query);

            return result.Data;
        }

        /// <summary>
        /// 导入物料和物料的所有属性(基本属性和自定义属性)
        /// </summary>
        /// <param name="query">file path /{path}</param>
        /// <returns>返回运行结果</returns>
        [Route("import/materials")]
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<IActionResult> ImportMaterialsWithAllProperties(Import.Command query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDetails = Request.UserDetails();
            if (userDetails != null)
            {
                query.UserId = userDetails.Id.ToString();
            }

            var result = await Mediator.Send(query);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        /// <summary>
        /// 获取导出资源
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>返回导出资源列表</returns>
        [Route("excelResources")]
        [HttpGet]
        [ProducesResponseType(typeof(ExcelResources.Response), 200)]
        [SwaggerResponse(400, typeof(string[]), Description = "参数无效。")]
        [SwaggerTag("物料")]
        public async Task<ActionResult<ExcelResources.Response>> GetExcelResources(ExcelResources.Command query)
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
