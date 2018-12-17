using FluentValidation;

namespace Lunz.ProductCenter.ApiService.Api.HearFroms
{
    public class CreateValidator : AbstractValidator<Create.Command>
    {
        public CreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("名称不能为空。");
        }
    }
}
