using FluentValidation;
using FluentValidation.Validators;
using Lunz.Data;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Common;

namespace Lunz.ProductCenter.ApiService.Api.HearFroms
{
    public class UpdateValidator : AbstractValidator<Update.Command>
    {
        private readonly IDatabaseScopeFactory _databaseScopeFactory;
        private readonly IHearFromRepository _hearFromRepository;

        public UpdateValidator(
            IDatabaseScopeFactory databaseScopeFactory,
            IHearFromRepository hearFromRepository)
        {
            _databaseScopeFactory = databaseScopeFactory;
            _hearFromRepository = hearFromRepository;

            RuleFor(x => x.Id).NotEmpty()
                .WithMessage("Id 不能为空。");
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("名称不能为空。");

            RuleFor(x => x).Custom(Validate);
        }

        private void Validate(Update.Command message, CustomContext context)
        {
            using (var scope = _databaseScopeFactory.CreateReadOnly())
            {
                var model = _hearFromRepository.FindAsync(message.Id).GetAwaiter().GetResult();
                if (model == null)
                {
                    context.AddNotFoundError("数据不存在");
                }
                else
                {
                    if (message.Name.Equals(model.Name))
                    {
                        context.AddNoContentError("数据未更改");
                    }
                }
            }
        }
    }
}
