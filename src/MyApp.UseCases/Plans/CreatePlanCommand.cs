using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using FluentValidation;
using FluentValidation.Results;
using Mediator;
using MyApp.Entities.Plans;

namespace MyApp.UseCases.Plans;

public class CreatePlanCommandHandler : ICommandHandler<CreateUserCommand, Result>
{
    private readonly IRepository<Plan, PlanId> _planRepository;

    private readonly IAccessorAsync<CurrentUser> _currentUserAccessor;

    public CreatePlanCommandHandler(
        IRepository<Plan, PlanId> planRepository,
        IAccessorAsync<CurrentUser> currentUserAccessor)
    {
        _planRepository = planRepository;
        _currentUserAccessor = currentUserAccessor;
    }

    public async ValueTask<Result> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var currentUser = await _currentUserAccessor.Get();
        if (currentUser is null)
            return result.Fail();

        return result;
    }
}

public record CreateUserCommand() : ICommand<Result>;

public class CreatePlanValidator : AbstractValidator<CreateUserCommand>
{
    public override ValidationResult Validate(ValidationContext<CreateUserCommand> context)
    {
        RuleFor<>

        return base.Validate(context);
    }
}
