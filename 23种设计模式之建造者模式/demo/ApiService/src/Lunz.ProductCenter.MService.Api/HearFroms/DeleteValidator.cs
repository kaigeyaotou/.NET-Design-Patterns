using FluentValidation;
using FluentValidation.Validators;
using Lunz.Data;
using Lunz.ProductCenter.ApiService.Models.Api;
using Lunz.ProductCenter.ApiService.QueryStack.Repositories;
using Lunz.ProductCenter.Common;

namespace Lunz.ProductCenter.ApiService.Api.HearFroms
{
    public class DeleteValidator : AbstractValidator<Delete.Command>
    {
        private readonly IDatabaseScopeFactory _databaseScopeFactory;
        private readonly IHearFromRepository _hearFromRepository;

        public DeleteValidator(
            IDatabaseScopeFactory databaseScopeFactory,
            IHearFromRepository hearFromRepository)
        {
            _databaseScopeFactory = databaseScopeFactory;
            _hearFromRepository = hearFromRepository;

            RuleFor(x => x).Custom(Validate);
        }

        private void Validate(Delete.Command message, CustomContext context)
        {
            using (var scope = _databaseScopeFactory.CreateReadOnly())
            {
                var model = _hearFromRepository.FindAsync<HearFromDetails>(message.Id.Value).GetAwaiter().GetResult();
                if (model == null)
                {
                    context.AddNotFoundError("数据不存在");
                }
            }
        }
    }
}
