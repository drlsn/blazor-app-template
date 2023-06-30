using Corelibs.Basic.Auth;
using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using FluentValidation;
using Mediator;
using MyApp.Entities.Plans;
using MyApp.Entities.Users;
using System.Security.Claims;

namespace MyApp.UseCases.Plans;

public class CreatePlanCommandHandler : ICommandHandler<CreatePlanCommand, Result>
{
    private readonly IAccessorAsync<ClaimsPrincipal> _userAccessor;
    private readonly IRepository<Plan, PlanId> _planRepository;

    public CreatePlanCommandHandler(
        IAccessorAsync<ClaimsPrincipal> userAccessor,
        IRepository<Plan, PlanId> planRepository)
    {
        _userAccessor = userAccessor;
        _planRepository = planRepository;
    }

    public async ValueTask<Result> Handle(CreatePlanCommand cmd, CancellationToken ct)
    {
        var userId = await _userAccessor.GetUserID<UserId>();
        
        var plan = new Plan(cmd.Name, userId);
        
        return await _planRepository.Save(plan);
    }
}

public record CreatePlanCommand(string Name) : ICommand<Result>;

public class CreatePlanValidator : UserRequestValidator<CreatePlanCommand>
{
    public CreatePlanValidator(
        IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor)
    {
        RuleFor(person => person.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
    }
}
