using System.Linq;
using FluentValidation;

namespace Lunz.ProductCenter.MService.Api.PictureResource
{
    public class UpdateValidator : AbstractValidator<Update.Command>
    {
        public UpdateValidator()
        {
            RuleFor(x => x.ProductId).NotNull().NotEmpty()
                .WithMessage("产品Id不能为空。");
            RuleFor(x => x.Pictures).Must((x, token) => x.Pictures.Any(t =>
                !string.IsNullOrWhiteSpace(t.FileName) &&
                !string.IsNullOrWhiteSpace(t.Remark) &&
                t.Remark.Trim().Length <= 50 &&
                !string.IsNullOrWhiteSpace(t.FileURL)))
                .WithMessage("上传图片和描述不能为空。");
        }
    }
}
