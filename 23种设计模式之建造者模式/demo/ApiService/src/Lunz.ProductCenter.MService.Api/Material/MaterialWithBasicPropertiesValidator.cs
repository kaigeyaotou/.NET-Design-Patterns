using FluentValidation;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class MaterialWithBasicPropertiesValidator : AbstractValidator<MaterialWithBasicProperties.Command>
    {
        public MaterialWithBasicPropertiesValidator()
        {
            RuleFor(x => x.TypeId).NotNull().NotEmpty()
                .WithMessage("物料类型Id不能为空。");
            RuleFor(x => x.TypeName).NotNull().NotEmpty()
                .WithMessage("物料类型名称不能为空。");
        }
    }
}
