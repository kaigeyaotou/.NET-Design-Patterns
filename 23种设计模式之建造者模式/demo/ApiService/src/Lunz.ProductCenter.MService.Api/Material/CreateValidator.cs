using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using Lunz.Data;
using Lunz.ProductCenter.Common;
using Lunz.ProductCenter.MService.QueryStack.Repositories;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class CreateValidator : AbstractValidator<Create.Command>
    {
        private const string _tradCode = "01";
        private readonly IDatabaseScopeFactory _databaseScopeFactory;
        private readonly IMaterialRepository _materialRepository;

        public CreateValidator(
            IMaterialRepository materialRepository,
            IDatabaseScopeFactory databaseScopeFactory)
        {
            _materialRepository = materialRepository;
            _databaseScopeFactory = databaseScopeFactory;

            RuleFor(x => x.MateName).NotNull().NotEmpty()
                .WithMessage("物料名称不能为空。");
            RuleFor(x => x.MateTypeId).NotNull().NotEmpty()
                .WithMessage("物料类型不能为空。");
            RuleFor(x => x.MaterialSpecId).NotNull().NotEmpty()
                .WithMessage("物料规格不能为空。");
            RuleFor(x => x.MaterialUnitId).NotNull().NotEmpty()
                .WithMessage("物料单位不能为空。");
            RuleFor(x => x).Custom(Validate);
        }

        private void Validate(Create.Command message, CustomContext context)
        {
            using (var scope = _databaseScopeFactory.CreateReadOnly())
            {
                // 采购主体
                if (!string.IsNullOrEmpty(message.MateTypeCode) && message.MateTypeCode.StartsWith(_tradCode))
                {
                    if (message.Trades.Count == 0)
                    {
                        context.AddBadRequestError("采购主体不能为空");
                    }
                }

                var nameValidate = _materialRepository.ExistNameAsync(message.MateName, message.MateTypeId).GetAwaiter().GetResult();
                if (nameValidate.Any())
                {
                    context.AddBadRequestError("物料名称不能重复");
                }

                var list = _materialRepository.BasicExistAsync(message).GetAwaiter().GetResult().ToList();
                if (list.Any())
                {
                    var listQuery = list.Select(x => new
                    {
                        x.Id,
                        Tradestr = string.Join(",", x.Trades.OrderBy(t => t.TradeId).Select(y => y.TradeId).ToArray()),
                        ProStr = string.Join(",", x.Properties.OrderBy(y => y.PropId)
                             .Select(z => string.Concat(z.PropId, z.OptionId)).ToArray()),
                    }).ToList();

                    var trades = message.Trades.Where(x => !string.IsNullOrWhiteSpace(x.TradeId))
                        .Select(x => x.TradeId).Distinct().OrderBy(y => y);
                    var tradestr = string.Join(",", trades);

                    // 自定义属性
                    var properties = message.Properties.Where(x => (!string.IsNullOrWhiteSpace(x.PropId)) &&
                       (!string.IsNullOrWhiteSpace(x.OptionId))).OrderBy(y => y.PropId);
                    var proStr = string.Join(",", properties.Select(z => string.Concat(z.PropId, z.OptionId)).Distinct());
                    if (listQuery.Any(y => y.Tradestr.Equals(tradestr) && y.ProStr.Equals(proStr)))
                    {
                        context.AddBadRequestError("该物料已存在。");
                    }
                }
            }
        }
    }
}
