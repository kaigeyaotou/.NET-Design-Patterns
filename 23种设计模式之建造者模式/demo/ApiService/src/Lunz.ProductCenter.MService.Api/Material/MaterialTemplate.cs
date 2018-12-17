using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.FileSystem.Aliyun;
using Lunz.Kernel;
using Lunz.ProductCenter.Core.Models;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;

namespace Lunz.ProductCenter.MService.Api.Material
{
    /// <summary>
    /// 导出物料模板(包含基本属性和自定义属性)
    /// </summary>
    public class MaterialTemplate
    {
        public class Command : IRequest<ResponseResult<Response>>
        {
            /// <summary>
            /// 物料类型Id
            /// </summary>
            public string TypeId { get; set; }

            /// <summary>
            /// 物料类型名称
            /// </summary>
            public string TypeName { get; set; }

            /// <summary>
            /// 物料类型编码
            /// </summary>
            public string TypeCode { get; set; }

            /// <summary>
            /// 用户GUID
            /// </summary>
            [NSwag.Annotations.SwaggerIgnore]
            public string UserId { get; set; }

            /// <summary>
            /// 操作人Id
            /// </summary>
            [NSwag.Annotations.SwaggerIgnore]
            public string UserName { get; set; }
        }

        /// <summary>
        /// Url地址
        /// </summary>
        public class Response
        {
            public Response(string url)
            {
                Url = url;
            }

            /// <summary>
            /// Url地址
            /// </summary>
            public string Url { get; }
        }

        public class Handler : IRequestHandler<Command, ResponseResult<Response>>
        {
            private const string _tradCode = "01";
            private readonly ILogger<Handler> _logger;
            private readonly IDatabaseScopeFactory _databaseScopeFactory;
            private readonly IMaterialRepository _repository;
            private readonly IHostingEnvironment _hostingEnvironment;

            public Handler(
                IDatabaseScopeFactory databaseScopeFactory,
                ILogger<Handler> logger,
                IMaterialRepository repository,
                IHostingEnvironment hostingEnvironment)
            {
                _databaseScopeFactory = databaseScopeFactory;
                _logger = logger;
                _repository = repository;

                _hostingEnvironment = hostingEnvironment;
            }

            public async Task<ResponseResult<Response>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                var queryResult = new MaterialTypeExport();
                IEnumerable<MaterialPropertyExport> basicProperties;
                IEnumerable<Lunz.ProductCenter.MService.QueryStack.Models.Trade> trades;

                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    queryResult = await _repository.FindCustomPropertiesByTypeAsync<MaterialTypeExport>(request.TypeId);

                    basicProperties = await _repository.FindBasicPropertiesAsync<MaterialPropertyExport>();

                    trades = await _repository.FindAllTradePropertiesAsync<QueryStack.Models.Trade>();
                }

                string url = string.Empty;
                if (queryResult != null)
                {
                    var createResult = await CreateFolder();
                    var path = createResult.Item1;
                    var fileName = $"物料模板-{request.TypeName}{request.UserName}-{createResult.Item2}.xlsx";
                    FileInfo file = new FileInfo($@"{path}\{fileName}");
                    if (file?.Exists ?? false)
                    {
                        file.Delete();
                    }

                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        // 物料模板sheet
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("物料");

                        // 物料属性sheet
                        ExcelWorksheet propertysheet = package.Workbook.Worksheets.Add("属性");

                        // 第一行
                        worksheet.Cells[1, 1].Value = $"物料类型Id:{queryResult.Id}";
                        worksheet.Cells[1, 2].Value = $"物料类型:{queryResult.TypeName}";
                        worksheet.Cells[1, 3].Value = $"物料编码:{request.TypeCode}";
                        worksheet.Cells[1, 4].Value = $"模板导出时间:{string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now)}";

                        // row 2, col 1, 物料名称列
                        worksheet.Cells[2, 1].IsRichText = true;
                        ExcelRichText ert = worksheet.Cells[2, 1].RichText.Add("物料名称");
                        ert = worksheet.Cells[2, 1].RichText.Add("*");
                        ert.Color = Color.Red;

                        // 从row 3到maxRows, col 1, 物料名称列验证
                        var minLength = 1;
                        var maxLength = 40;
                        int maxRows = ExcelPackage.MaxRows;
                        var textValidation = worksheet.DataValidations.AddTextLengthValidation($"A3:A{maxRows}");
                        textValidation.ShowErrorMessage = true;
                        textValidation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                        textValidation.ErrorTitle = "物料名称";
                        textValidation.Error = string.Format("物料名称不能为空，且最大长度不能超过{0}个字符以内的汉字、字母、数字、符号的组合", maxLength);
                        textValidation.Formula.Value = minLength;
                        textValidation.Formula2.Value = maxLength;

                        // 属性sheet
                        var optionRow = 2;
                        var optionCol = 1;

                        // 物料sheet, row 2, col 2, col 3, DT0000000015: 物料单位, DT0000000031: 物料规格
                        if (basicProperties?.Count() == 2)
                        {
                            optionCol = 1;

                            // 物料sheet, 从row 3到maxRows, col 2, col 3, 物料规格列下拉列表, 物料单位列下拉列表
                            var col = 2;
                            foreach (var property in basicProperties)
                            {
                                worksheet.Cells[2, col].IsRichText = true;
                                ert = worksheet.Cells[2, col].RichText.Add(property.DisplayName);
                                ert = worksheet.Cells[2, col].RichText.Add("*");
                                ert.Color = Color.Red;

                                var basicPropertiesRange = ExcelRange.GetAddress(3, col, ExcelPackage.MaxRows, col);

                                // 属性值
                                var options = property.Options;
                                if (options?.Any() ?? false)
                                {
                                    var basicPropertiesValidation = worksheet.DataValidations.AddListValidation(basicPropertiesRange);
                                    basicPropertiesValidation.ShowErrorMessage = true;
                                    basicPropertiesValidation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                                    basicPropertiesValidation.ErrorTitle = "选择基本属性";
                                    basicPropertiesValidation.Error = "请从属性列表中选择一项";

                                    var valueRange = ExcelRange.GetAddress(2, optionCol, options.Count + 1, optionCol, true);
                                    basicPropertiesValidation.Formula.ExcelFormula = $@"属性!{valueRange}";

                                    // 属性sheet, 属性名
                                    propertysheet.Cells[1, optionCol].Value = property.DisplayName;
                                    optionRow = 2;
                                    foreach (var option in options)
                                    {
                                        // 属性sheet, 所有属性选项值
                                        propertysheet.Cells[optionRow, optionCol].Value = $"{option.OptionId}:{option.OptionName}";
                                        optionRow++;
                                    }
                                }

                                optionCol++;
                                col++;
                            }
                        }
                        else
                        {
                            worksheet.Cells[2, 2].IsRichText = true;
                            ert = worksheet.Cells[2, 2].RichText.Add("物料规格");
                            ert = worksheet.Cells[2, 2].RichText.Add("*");
                            ert.Color = Color.Red;

                            worksheet.Cells[2, 3].IsRichText = true;
                            ert = worksheet.Cells[2, 3].RichText.Add("物料单位");
                            ert = worksheet.Cells[2, 3].RichText.Add("*");
                            ert.Color = Color.Red;
                        }

                        // row 2, col 4, 是否为生产物料列, 【是否为生产物料】选择为“是”时，【是否有独立编码】默认为“否”且不可修改。
                        worksheet.Cells[2, 4].IsRichText = true;
                        ert = worksheet.Cells[2, 4].RichText.Add("是否为生产物料");
                        ert = worksheet.Cells[2, 4].RichText.Add("*");
                        ert.Color = Color.Red;

                        // 是否为生产物料下拉列表
                        var range = ExcelRange.GetAddress(3, 4, ExcelPackage.MaxRows, 4);
                        var listValidation = worksheet.DataValidations.AddListValidation(range);
                        listValidation.ShowErrorMessage = true;
                        listValidation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                        listValidation.ErrorTitle = "选择基本属性";
                        listValidation.Error = "请从属性列表中选择一项";
                        listValidation.Formula.Values.Add("0:否");
                        listValidation.Formula.Values.Add("1:是");

                        // row 2, col 5, 是否有独立编号列
                        worksheet.Cells[2, 5].IsRichText = true;
                        ert = worksheet.Cells[2, 5].RichText.Add("是否有独立编号");
                        ert = worksheet.Cells[2, 5].RichText.Add("*");
                        ert.Color = Color.Red;

                        // 是否有独立编号下拉列表
                        range = ExcelRange.GetAddress(3, 5, ExcelPackage.MaxRows, 5);
                        listValidation = worksheet.DataValidations.AddListValidation(range);
                        listValidation.ShowErrorMessage = true;
                        listValidation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                        listValidation.ErrorTitle = "选择基本属性";
                        listValidation.Error = "请从属性列表中选择一项";
                        listValidation.Formula.Values.Add("0:否");
                        listValidation.Formula.Values.Add("1:是");

                        // row 2, col 6, 采购主体列
                        StringBuilder tradeValue = new StringBuilder();
                        tradeValue.Append(Environment.NewLine);
                        tradeValue.Append("(多个采购主体用英文逗号隔开)");
                        if (trades?.Any() ?? false)
                        {
                            tradeValue.Append(Environment.NewLine);
                            foreach (var trade in trades)
                            {
                                tradeValue.Append($"{trade.Id}:{trade.TradeName},");
                            }
                        }

                        var value = tradeValue.ToString();
                        worksheet.Cells[2, 6].IsRichText = true;
                        ert = worksheet.Cells[2, 6].RichText.Add("采购主体");

                        if (!string.IsNullOrEmpty(request.TypeCode) && request.TypeCode.StartsWith(_tradCode))
                        {
                            ert = worksheet.Cells[2, 6].RichText.Add("*");
                            ert.Color = Color.Red;
                        }

                        ert = worksheet.Cells[2, 6].RichText.Add(value.Remove(value.LastIndexOf(',')));
                        ert.Color = Color.Black;
                        worksheet.Cells[2, 6].Style.WrapText = true;

                        // row 2, col 7, 建议采购价列
                        worksheet.Cells[2, 7].Value = "建议采购价(5位以内的正数,两位小数)";
                        worksheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        // 从row 3到maxRows, col 7, 建议采购价列验证
                        range = ExcelRange.GetAddress(3, 7, ExcelPackage.MaxRows, 7);
                        var decimalValidation = worksheet.DataValidations.AddDecimalValidation(range);
                        decimalValidation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                        decimalValidation.ErrorTitle = "建议采购价";
                        decimalValidation.Error = "建议采购价是5位以内的正数";
                        decimalValidation.ShowErrorMessage = true;
                        decimalValidation.Operator = ExcelDataValidationOperator.between;
                        decimalValidation.Formula.Value = 0.00;
                        decimalValidation.Formula2.Value = 99999.99;

                        // 属性sheet
                        optionRow = 2;

                        // row 2, 从col 8开始, 自定义属性列
                        var properties = queryResult.Properties;
                        if (properties?.Any() ?? false)
                        {
                            optionCol = 3;
                            var col = 8;
                            foreach (var property in properties)
                            {
                                // 从row 3开始，添加自定义属性选项值下拉列表
                                range = ExcelRange.GetAddress(3, col, ExcelPackage.MaxRows, col);

                                // 属性选项值
                                var options = property.Options;
                                if (options?.Any() ?? false)
                                {
                                    listValidation = worksheet.DataValidations.AddListValidation(range);
                                    listValidation.ShowErrorMessage = true;
                                    listValidation.ErrorTitle = "选择自定义属性";
                                    listValidation.Error = "请从属性列表中选择一项";

                                    var customerOptionRange = ExcelRange.GetAddress(2, optionCol, options.Count + 1, optionCol, true);
                                    listValidation.Formula.ExcelFormula = $@"属性!{customerOptionRange}";

                                    // 属性sheet, 属性名
                                    propertysheet.Cells[1, optionCol].Value = $"{property.PropId}:{property.DisplayName}";
                                    optionRow = 2;
                                    foreach (var option in options)
                                    {
                                        // 属性sheet, 所有属性选项值
                                        propertysheet.Cells[optionRow, optionCol].Value = $"{option.OptionId}:{option.OptionName}";
                                        optionRow++;
                                    }
                                }

                                // 自定义属性是否必填
                                if (property.IsNecessary)
                                {
                                    worksheet.Cells[2, col].IsRichText = true;
                                    ert = worksheet.Cells[2, col].RichText.Add($"{property.PropId}:{property.DisplayName}");
                                    ert = worksheet.Cells[2, col].RichText.Add("*");
                                    ert.Color = Color.Red;

                                    if (options?.Any() ?? false)
                                    {
                                        listValidation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                                    }
                                }
                                else
                                {
                                    worksheet.Cells[2, col].Value = $"{property.PropId}:{property.DisplayName}";

                                    if (options?.Any() ?? false)
                                    {
                                        listValidation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                                    }
                                }

                                optionCol++;
                                col++;
                            }
                        }

                        worksheet.View.FreezePanes(3, 1);
                        worksheet.Cells.AutoFitColumns();
                        propertysheet.Cells.AutoFitColumns();

                        worksheet.Column(6).Width = 32;

                        package.Save();
                    }

                    string ossEndpoint = "http://oss-cn-hangzhou.aliyuncs.com";
                    string ossAccessKeyId = "LTAIFAiipRXidbYT";
                    string ossAccessKeySecret = "62zh9kaAd60NAqMxNPCqVcvxhNxH0H";
                    string ossBucketName = "basichz";
                    string ossRootFolder = "productcenter";
                    string ossAccessUrl = "//oss.lunz.cn/";
                    var fileManager = new OssFileManager(ossEndpoint, ossAccessKeyId, ossAccessKeySecret, ossBucketName, ossRootFolder, ossAccessUrl);

                    // 上传至阿里云
                    var uploadResult = fileManager.Upload(file.Name.Trim(), $@"{path}\{file.Name.Trim()}");

                    // 返回阿里云Url地址
                    url = $@"http://basichz.lunz.cn/{uploadResult.Key}";

                    // 删除文件夹
                    DirectoryInfo directory = new DirectoryInfo(path);
                    if (directory.Exists)
                    {
                        directory.Delete(true);
                    }
                }

                return ResponseResult<Response>.Ok(new Response(url));
            }

            private async Task<(string, string)> CreateFolder()
            {
                var rootPath = _hostingEnvironment.WebRootPath;
                var path = $@"{rootPath}\export";
                DirectoryInfo folder = new DirectoryInfo(path);
                if (!folder.Exists)
                {
                    await Task.Factory.StartNew(() => folder.Create());
                }

                var dateTime = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
                path = $@"{rootPath}\export\{dateTime}";
                folder = new DirectoryInfo(path);
                if (!folder.Exists)
                {
                    await Task.Factory.StartNew(() => folder.Create());
                }

                return (path, dateTime);
            }
        }
    }
}
