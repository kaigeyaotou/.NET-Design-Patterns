using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.Kernel;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class Import
    {
        public class Command : IRequest<ResponseResult>
        {
            /// <summary>
            /// Url地址
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 用户GUID
            /// </summary>
            public string UserId { get; set; }

            /// <summary>
            /// 物料类型编码
            /// </summary>
            public string TypeCode { get; set; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
            private const string _tradCode = "01";
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMaterialRepository _repository;
            private readonly IMaterialTypeRepository _materialTypeRepository;
            private readonly IBasicDataRepository _basicDataRepository;
            private readonly IPropertyRespository _propertyRepository;
            private readonly IPropertiesOptionResponsitory _propertyOptionRepository;

            public Handler(
                IMaterialRepository repository,
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger,
                IMaterialTypeRepository materialTypeRepository,
                IBasicDataRepository basicDataRepository,
                IPropertyRespository propertyRepository,
                IPropertiesOptionResponsitory propertyOptionRepository)
            {
                _repository = repository;
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;

                _materialTypeRepository = materialTypeRepository;
                _basicDataRepository = basicDataRepository;
                _propertyRepository = propertyRepository;
                _propertyOptionRepository = propertyOptionRepository;
            }

            public async Task<ResponseResult> Handle(Command request, CancellationToken cancellationToken)
            {
                request.UserId = string.IsNullOrWhiteSpace(request.UserId) ? null : request.UserId;

                var req = System.Net.WebRequest.Create(request.Url);
                if (req != null)
                {
                    using (Stream requstStream = req.GetResponse().GetResponseStream())
                    {
                        var copyStream = new MemoryStream();
                        requstStream.CopyTo(copyStream);

                        using (copyStream)
                        {
                            var context = new ExcelPackage(copyStream);
                            var worksheet = context.Workbook.Worksheets[1];

                            int colStart = worksheet.Dimension.Start.Column;    // starts from 1
                            int colEnd = worksheet.Dimension.End.Column;
                            int rowStart = worksheet.Dimension.Start.Row;       // starts from 1
                            int rowEnd = worksheet.Dimension.End.Row;

                            // id
                            var typeId = worksheet.Cells[1, 1].Text.Trim();
                            var index = typeId?.IndexOf(':');
                            if ((index ?? 0) > 0)
                            {
                                typeId = typeId.Substring(index.Value + 1);
                            }
                            else
                            {
                                return ResponseResult.Error("物料类型Id不能为空。");
                            }

                            // typecode
                            var typeCode = worksheet.Cells[1, 3].Text.Trim();
                            var code = typeCode?.Substring(5);
                            if (code.StartsWith(_tradCode) && !code.Equals(request.TypeCode))
                            {
                                return ResponseResult.Error("导入模板错误");
                            }

                            // upload datetime
                            var templateDateTime = worksheet.Cells[1, 4].Text.Trim();
                            index = templateDateTime?.IndexOf(':');
                            if ((index ?? 0) > 0)
                            {
                                templateDateTime = templateDateTime.Substring(index.Value + 1);
                            }
                            else
                            {
                                return ResponseResult.Error("模板导出时间不能为空。");
                            }

                            var type = new QueryStack.Models.MaterialType();
                            using (var scope = _databaseScopeFactory.CreateReadOnly())
                            {
                                if (!await _repository.IsUploadDateTimeLatestAsync(typeId, templateDateTime))
                                {
                                    return ResponseResult.Error("导入模板不是最新的，请重新下载模板。");
                                }

                                type = await _materialTypeRepository.FindAsync(typeId);
                                if (type == null)
                                {
                                    return ResponseResult.Error("物料类型不正确，请重新下载模板。");
                                }
                            }

                            StringBuilder errorMessage = new StringBuilder();
                            for (int i = rowStart + 2; i <= rowEnd; i++)
                            {
                            newrow:
                                if (i > rowEnd)
                                {
                                    break;
                                }

                                try
                                {
                                    var material = new QueryStack.Models.Material();

                                    // 物料类型
                                    material.MateTypeId = typeId;
                                    material.MateTypeCode = type.TypeCode;
                                    material.MateTypeName = type.TypeName;

                                    // 物料名称
                                    var text = worksheet.Cells[i, 1].Text.Trim();
                                    if (string.IsNullOrWhiteSpace(text) || text.Length > 40)
                                    {
                                        errorMessage.Append($"{i.ToString()},");
                                        continue;
                                    }

                                    material.MateName = text;

                                    // 物料规格
                                    text = worksheet.Cells[i, 2].Text.Trim();
                                    if (string.IsNullOrWhiteSpace(text))
                                    {
                                        errorMessage.Append($"{i.ToString()},");
                                        continue;
                                    }
                                    else
                                    {
                                        index = text.IndexOf(':');
                                        if ((index ?? 0) > 0)
                                        {
                                            var spec = text.Split(':');
                                            material.MaterialSpecId = spec[0];
                                            material.MaterialSpec = spec[1];
                                        }
                                        else
                                        {
                                            errorMessage.Append($"{i.ToString()},");
                                            continue;
                                        }
                                    }

                                    // 物料单位
                                    text = worksheet.Cells[i, 3].Text.Trim();
                                    if (string.IsNullOrWhiteSpace(text))
                                    {
                                        errorMessage.Append($"{i.ToString()},");
                                        continue;
                                    }
                                    else
                                    {
                                        index = text.IndexOf(':');
                                        if ((index ?? 0) > 0)
                                        {
                                            var unit = text.Split(':');
                                            material.MaterialUnitId = unit[0];
                                            material.MaterialUnits = unit[1];
                                        }
                                        else
                                        {
                                            errorMessage.Append($"{i.ToString()},");
                                            continue;
                                        }
                                    }

                                    // 是否有独立编号
                                    text = worksheet.Cells[i, 5].Text.Trim();
                                    if (string.IsNullOrWhiteSpace(text))
                                    {
                                        errorMessage.Append($"{i.ToString()},");
                                        continue;
                                    }
                                    else
                                    {
                                        index = text.IndexOf(':');
                                        if ((index ?? 0) > 0)
                                        {
                                            var idCodeSingle = text.Split(':');
                                            if (idCodeSingle[0].Equals("0"))
                                            {
                                                material.IdCodeSingle = false;
                                            }
                                            else if (idCodeSingle[0].Equals("1"))
                                            {
                                                material.IdCodeSingle = true;
                                            }
                                            else
                                            {
                                                errorMessage.Append($"{i.ToString()},");
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            errorMessage.Append($"{i.ToString()},");
                                            continue;
                                        }
                                    }

                                    // 是否为生产物料
                                    text = worksheet.Cells[i, 4].Text.Trim();
                                    if (string.IsNullOrWhiteSpace(text))
                                    {
                                        errorMessage.Append($"{i.ToString()},");
                                        continue;
                                    }
                                    else
                                    {
                                        index = text.IndexOf(':');
                                        if ((index ?? 0) > 0)
                                        {
                                            var isSelfBuild = text.Split(':');
                                            if (isSelfBuild[0].Equals("0"))
                                            {
                                                material.IsSelfBuild = false;
                                            }
                                            else if (isSelfBuild[0].Equals("1"))
                                            {
                                                material.IsSelfBuild = true;
                                                material.IdCodeSingle = false; // 【是否为生产物料】选择为“是”时，【是否有独立编码】默认为“否”且不可修改。
                                            }
                                            else
                                            {
                                                errorMessage.Append($"{i.ToString()},");
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            errorMessage.Append($"{i.ToString()},");
                                            continue;
                                        }
                                    }

                                    // 建议采购价
                                    text = worksheet.Cells[i, 7].Text.Trim();
                                    if (!string.IsNullOrWhiteSpace(text))
                                    {
                                        decimal price = 0;
                                        try
                                        {
                                            price = decimal.Round(decimal.Parse(text), 2);
                                            material.MaterialPrice = price;
                                        }
                                        catch (Exception)
                                        {
                                            errorMessage.Append($"{i.ToString()},");
                                            continue;
                                        }
                                    }

                                    // 采购主体
                                    text = worksheet.Cells[i, 6].Text.Trim();
                                    if (!string.IsNullOrEmpty(request.TypeCode) && request.TypeCode.StartsWith(_tradCode))
                                    {
                                        if (string.IsNullOrWhiteSpace(text))
                                        {
                                            errorMessage.Append($"{i.ToString()},");
                                            i++;
                                            continue;
                                        }
                                    }

                                    var tradeList = text.Split(',');
                                    if (tradeList != null)
                                    {
                                        foreach (var item in tradeList)
                                        {
                                            if (!string.IsNullOrWhiteSpace(item.Trim()))
                                            {
                                                index = item.IndexOf(':');
                                                if ((index ?? 0) > 0)
                                                {
                                                    var trade = new QueryStack.Models.Trade();
                                                    var tradeSplit = item.Split(':');
                                                    trade.TradeId = tradeSplit[0];
                                                    trade.TradeName = tradeSplit[1];

                                                    material.Trades.Add(trade);
                                                }
                                                else
                                                {
                                                    errorMessage.Append($"{i.ToString()},");
                                                    i++;
                                                    goto newrow;
                                                }
                                            }
                                        }
                                    }

                                    // 自定义属性， 从第8列开始
                                    for (int m = 8; m <= colEnd; m++)
                                    {
                                        var property = new MaterialProperty();

                                        // 属性
                                        text = worksheet.Cells[2, m].Text.Trim();
                                        if (string.IsNullOrWhiteSpace(text))
                                        {
                                            errorMessage.Append($"{i.ToString()},");
                                            i++;
                                            goto newrow;
                                        }
                                        else
                                        {
                                            index = text.IndexOf(':');
                                            if ((index ?? 0) > 0)
                                            {
                                                var propertySplit = text.Split(':');
                                                property.PropId = propertySplit[0];

                                                var name = propertySplit[1];
                                                property.PropName = (name.EndsWith('*') == true) ? name.Remove(name.LastIndexOf('*')) : name;
                                            }
                                            else
                                            {
                                                errorMessage.Append($"{i.ToString()},");
                                                i++;
                                                goto newrow;
                                            }
                                        }

                                        // 选项
                                        text = worksheet.Cells[i, m].Text.Trim();

                                        // 自定义属性是否必填
                                        if (worksheet.Cells[2, m].IsRichText)
                                        {
                                            if (string.IsNullOrWhiteSpace(text))
                                            {
                                                errorMessage.Append($"{i.ToString()},");
                                                i++;
                                                goto newrow;
                                            }

                                            index = text.IndexOf(':');
                                            if ((index ?? 0) > 0)
                                            {
                                                var optionSplit = text.Split(':');
                                                property.OptionId = optionSplit[0];
                                            }
                                            else
                                            {
                                                errorMessage.Append($"{i.ToString()},");
                                                i++;
                                                goto newrow;
                                            }

                                            material.Properties.Add(property);
                                        }
                                        else if (!worksheet.Cells[2, m].IsRichText && !string.IsNullOrWhiteSpace(text))
                                        {
                                            index = text.IndexOf(':');
                                            if ((index ?? 0) > 0)
                                            {
                                                var optionSplit = text.Split(':');
                                                property.OptionId = optionSplit[0];
                                            }
                                            else
                                            {
                                                errorMessage.Append($"{i.ToString()},");
                                                i++;
                                                goto newrow;
                                            }

                                            material.Properties.Add(property);
                                        }
                                    }

                                    // 物料有效性验证
                                    if (!MaterialValidation(material))
                                    {
                                        errorMessage.Append($"{i.ToString()},");
                                        continue;
                                    }

                                    using (var scope = _databaseScopeFactory.CreateWithTransaction())
                                    {
                                        // INSERT INTO tb_materialinfo
                                        var result = await _repository.GetIdAndMaxCodeAsync();
                                        material.Id = result.Id;
                                        var c = 1;
                                        if (result.Data != null)
                                        {
                                            c = int.Parse(result.Data.MateCode.Substring(2)) + 1;
                                        }

                                        material.MateCode = "WL" + c.ToString().PadLeft(8, '0');

                                        await _repository.AddAsync(material, request.UserId);

                                        // INSERT INTO tb_materialproperties
                                        if (material.Properties?.Any() ?? false)
                                        {
                                            foreach (var item in material.Properties)
                                            {
                                                item.MaterialId = material.Id;
                                            }

                                            await _repository.AddPropertiesAsync(material.Properties, request.UserId);
                                        }

                                        // INSERT INTO tb_tradeinfo
                                        if (material.Trades?.Any() ?? false)
                                        {
                                            foreach (var item in material.Trades)
                                            {
                                                item.ProdMaterId = material.Id;
                                            }

                                            await _repository.AddTradeAsync(material.Trades, request.UserId);
                                        }

                                        scope.SaveChanges();
                                    }
                                }
                                catch (Exception)
                                {
                                    errorMessage.Append($"{i.ToString()},");
                                    continue;
                                }
                            }

                            if (errorMessage?.Length > 0)
                            {
                                string message = errorMessage.ToString();
                                message = (message.EndsWith(',') == true) ? message.Remove(message.LastIndexOf(',')) : message;

                                return ResponseResult.Error($"第{message}行导入出错。");
                            }
                        }
                    }

                    return ResponseResult.Ok();
                }

                return ResponseResult.Error($"文件导入出错，请重新导入");
            }

            private bool MaterialValidation(QueryStack.Models.Material material)
            {
                if (material == null)
                {
                    return false;
                }

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    var materialName = _repository.ExistNameAsync(material.MateName, material.MateTypeId).GetAwaiter().GetResult();
                    if (materialName?.Any() ?? false)
                    {
                        return false;
                    }

                    // 物料判重
                    var list = _repository.BasicExistAsync(material).GetAwaiter().GetResult().ToList();
                    if (list?.Any() ?? false)
                    {
                        var listQuery = list.Select(x => new
                        {
                            x.Id,
                            TradeStr = string.Join(",", x.Trades.OrderBy(t => t.TradeId).Select(y => y.TradeId).ToList()),
                            ProStr = string.Join(",", x.Properties.OrderBy(y => y.PropId)
                                 .Select(z => string.Concat(z.PropId, z.OptionId)).ToList()),
                        }).ToList();

                        // 采购主体
                        var tradeList = material.Trades.Where(x => !string.IsNullOrWhiteSpace(x.TradeId))
                            .Select(x => x.TradeId).Distinct().OrderBy(y => y);
                        var tradeStr = string.Join(",", tradeList);

                        // 自定义属性
                        var propertyList = material.Properties.Where(x => (!string.IsNullOrWhiteSpace(x.PropId)) &&
                           (!string.IsNullOrWhiteSpace(x.OptionId))).OrderBy(y => y.PropId);
                        var proStr = string.Join(",", propertyList.Select(z => string.Concat(z.PropId, z.OptionId)).Distinct());

                        // 该物料已存在
                        if (listQuery.Any(y => y.TradeStr.Equals(tradeStr) && y.ProStr.Equals(proStr)))
                        {
                            return false;
                        }
                    }

                    // Id有效性验证

                    // 物料规格 Id
                    var basicDataresult = _basicDataRepository.FindAsync(material.MaterialSpecId).GetAwaiter().GetResult();
                    if (basicDataresult == null)
                    {
                        return false;
                    }
                    else
                    {
                        material.MaterialSpec = basicDataresult.Name;
                    }

                    // 物料单位 Id
                    basicDataresult = _basicDataRepository.FindAsync(material.MaterialUnitId).GetAwaiter().GetResult();
                    if (basicDataresult == null)
                    {
                        return false;
                    }
                    else
                    {
                        material.MaterialUnits = basicDataresult.Name;
                    }

                    // 属性 Id
                    foreach (var property in material.Properties)
                    {
                        var propertyResult = _propertyRepository.FindPropertyAsync(property.PropId).GetAwaiter().GetResult();
                        if (propertyResult == null)
                        {
                            return false;
                        }
                        else
                        {
                            property.PropName = propertyResult.PropName;

                            // 属性值 Id
                            var optionResult = _propertyOptionRepository.FindAsync(property.OptionId).GetAwaiter().GetResult();
                            if (optionResult == null)
                            {
                                return false;
                            }
                        }
                    }

                    // 采购主体 Id
                    foreach (var trade in material.Trades)
                    {
                        basicDataresult = _basicDataRepository.FindAsync(trade.TradeId).GetAwaiter().GetResult();
                        if (basicDataresult == null)
                        {
                            return false;
                        }
                        else
                        {
                            trade.TradeName = basicDataresult.Name;
                        }
                    }
                }

                return true;
            }
        }
    }
}
