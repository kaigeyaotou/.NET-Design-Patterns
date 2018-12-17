using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lunz.Data;
using Lunz.FileSystem.Aliyun;
using Lunz.Kernel;
using Lunz.ProductCenter.MService.QueryStack.Models;
using Lunz.ProductCenter.MService.QueryStack.Repositories;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class MaterialWithBasicProperties
    {
        /// <summary>
        /// 导出物料和物料的基本属性
        /// </summary>
        public class Command : IRequest<ResponseResult>
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

        public class Handler : IRequestHandler<Command, ResponseResult>
        {
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

            public async Task<ResponseResult> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                IEnumerable<MaterialExport> queryResult;
                using (var scope = _databaseScopeFactory.CreateReadOnly())
                {
                    queryResult = await _repository.FindMaterialsWithBasicPropertiesByTypeAsync<MaterialExport>(request.TypeId);
                }

                if (queryResult?.Any() ?? false)
                {
                    var list = queryResult.ToList();

                    var createResult = await CreateFolder();
                    var path = createResult.Item1;
                    var fileName = $"{request.TypeName}{request.UserName}-{createResult.Item2}.xlsx";
                    FileInfo file = new FileInfo($@"{path}\{fileName}");
                    if (file?.Exists ?? false)
                    {
                        file.Delete();
                    }

                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("物料");

                        worksheet.Cells[1, 1].Value = "物料编号";
                        worksheet.Cells[1, 2].Value = "物料名称";
                        worksheet.Cells[1, 3].Value = "物料类型";
                        worksheet.Cells[1, 4].Value = "物料规格";
                        worksheet.Cells[1, 5].Value = "物料单位";
                        worksheet.Cells[1, 6].Value = "是否为生产物料";
                        worksheet.Cells[1, 7].Value = "是否有独立编号";
                        worksheet.Cells[1, 8].Value = "采购主体";

                        worksheet.Cells[1, 9].Value = "建议采购价";
                        worksheet.Cells[1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        worksheet.Cells[1, 10].Value = "是否已发布";
                        worksheet.Cells[1, 11].Value = "发布时间";
                        worksheet.Cells[1, 12].Value = "是否启用";

                        int row = 2;
                        foreach (var item in list)
                        {
                            worksheet.Cells[row, 1].Value = item.MateCode;
                            worksheet.Cells[row, 2].Value = item.MateName;
                            worksheet.Cells[row, 3].Value = item.TypeName;
                            worksheet.Cells[row, 4].Value = item.MaterialSpec;
                            worksheet.Cells[row, 5].Value = item.MaterialUnits;
                            worksheet.Cells[row, 6].Value = item.IsSelfBuild ? "是" : "否";
                            worksheet.Cells[row, 7].Value = item.IdCodeSingle ? "是" : "否";
                            worksheet.Cells[row, 8].Value = item.TradeName;

                            worksheet.Cells[row, 9].Value = string.Format("{0:#,##0.00}", item.MaterialPrice);
                            worksheet.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            worksheet.Cells[row, 10].Value = (item.AuditState == 1) ? "是" : "否";
                            worksheet.Cells[row, 11].Value = (item.AuditState == 1) ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.AuditTime) : string.Empty;
                            worksheet.Cells[row, 12].Value = item.IsDisable ? "禁用" : "启用";
                            row++;
                        }

                        worksheet.View.FreezePanes(2, 1);
                        worksheet.Cells.AutoFitColumns();

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
                    var url = $@"http://basichz.lunz.cn/{uploadResult.Key}";

                    file.Refresh();
                    var resource = new ResourceItem
                    {
                        FileName = fileName,
                        FileType = file.Extension, // .xlsx
                        FileLength = Convert.ToInt32(Math.Ceiling(file.Length / 1024.0)),
                        FileURL = url,
                    };

                    using (var scope = _databaseScopeFactory.CreateWithTransaction())
                    {
                        await _repository.AddResourceAsync(resource, request.UserId);
                        scope.SaveChanges();
                    }

                    // 删除文件夹
                    DirectoryInfo directory = new DirectoryInfo(path);
                    if (directory.Exists)
                    {
                        directory.Delete(true);
                    }
                }

                return ResponseResult.Ok();
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