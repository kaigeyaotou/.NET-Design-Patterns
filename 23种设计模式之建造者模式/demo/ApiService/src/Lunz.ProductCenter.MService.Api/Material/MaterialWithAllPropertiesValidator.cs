using FluentValidation;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class MaterialWithAllPropertiesValidator : AbstractValidator<MaterialWithAllProperties.Command>
    {
        public MaterialWithAllPropertiesValidator()
        {
            RuleFor(x => x.TypeId).NotNull().NotEmpty()
                .WithMessage("物料类型Id不能为空。");
            RuleFor(x => x.TypeName).NotNull().NotEmpty()
                .WithMessage("物料类型名称不能为空。");
        }
    }
}
