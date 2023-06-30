using Corelibs.Basic.Auth;
using Corelibs.Basic.Blocks;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Mediator;
using MyApp.Entities.ExercisesAimsControls;
using MyApp.Entities.PlanAimControls;
using MyApp.Entities.SessionAimControls;
using MyApp.Entities.Users;
using System.Security.Claims;

namespace MyApp.UseCases.Users;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result>
{
    private readonly IAccessorAsync<ClaimsPrincipal> _userAccessor;
    private readonly IRepository<User, UserId> _userRepository;
    private readonly IRepository<PlanAimControl, PlanAimControlId> _planAimControlRepository;
    private readonly IRepository<SessionAimControl, SessionAimControlId> _sessionAimControlRepository;
    private readonly IRepository<ExerciseAimControl, ExerciseAimControlId> _exerciseAimControlRepository;

    public CreateUserCommandHandler(
        IAccessorAsync<ClaimsPrincipal> userAccessor,
        IRepository<User, UserId> userRepository,
        IRepository<PlanAimControl, PlanAimControlId> planAimControlRepository,
        IRepository<SessionAimControl, SessionAimControlId> sessionAimControlRepository,
        IRepository<ExerciseAimControl, ExerciseAimControlId> exerciseAimControlRepository)
    {
        _userAccessor = userAccessor;
        _userRepository = userRepository;
        _planAimControlRepository = planAimControlRepository;
        _sessionAimControlRepository = sessionAimControlRepository;
        _exerciseAimControlRepository = exerciseAimControlRepository;
    }

    public async ValueTask<Result> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var userId = await _userAccessor.GetUserID<UserId>();
        var user = await _userRepository.Get(userId, result);
        if (user != null)
            return result;

        var planAimControl = new PlanAimControl();
        var sessionAimControl = new SessionAimControl();
        var exerciseAimControl = new ExerciseAimControl();
        user = new User(
            userId,
            planAimControl.Id,
            sessionAimControl.Id,
            exerciseAimControl.Id);

        await _userRepository.Save(user, result);
        await _planAimControlRepository.Save(planAimControl, result);
        await _sessionAimControlRepository.Save(sessionAimControl, result);
        await _exerciseAimControlRepository.Save(exerciseAimControl, result);

        return result;
    }
}

public record CreateUserCommand() : ICommand<Result>;

public class CreateUserValidator : UserRequestValidator<CreateUserCommand>
{
    public CreateUserValidator(IAccessorAsync<ClaimsPrincipal> userAccessor) : base(userAccessor) {}
}
