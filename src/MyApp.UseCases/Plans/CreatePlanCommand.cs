using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using FluentValidation;
using Mediator;
using MyApp.Entities.Plans;

namespace MyApp.UseCases.Plans;

public class CreatePlanCommandHandler : ICommandHandler<CreatePlanCommand, Result>
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

    public async ValueTask<Result> Handle(CreatePlanCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var currentUser = await _currentUserAccessor.Get();
        if (currentUser is null)
            return result.Fail();

        return result;
    }
}

public record CreatePlanCommand(string Name) : ICommand<Result>;

public class CreatePlanValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanValidator()
    {
        RuleFor(person => person.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
    }
}
