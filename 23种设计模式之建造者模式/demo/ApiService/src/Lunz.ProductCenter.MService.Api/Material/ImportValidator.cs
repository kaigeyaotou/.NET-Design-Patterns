using FluentValidation;

namespace Lunz.ProductCenter.MService.Api.Material
{
    public class ImportValidator : AbstractValidator<Import.Command>
    {
        public ImportValidator()
        {
            RuleFor(x => x.Url).NotNull().NotEmpty()
                .WithMessage("Url地址不能为空。");
        }
    }
}