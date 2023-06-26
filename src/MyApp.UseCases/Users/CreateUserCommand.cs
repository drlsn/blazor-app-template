using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Functional;
using Corelibs.Basic.Repository;
using Mediator;
using MyApp.Entities.ExercisesAimsControls;
using MyApp.Entities.PlanAimControls;
using MyApp.Entities.SessionAimControls;
using MyApp.Entities.Users;

namespace MyApp.UseCases.Users
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result>
    {
        private readonly IRepository<User, UserId> _userRepository;
        private readonly IRepository<PlanAimControl, PlanAimControlId> _planAimControlRepository;
        private readonly IRepository<SessionAimControl, SessionAimControlId> _sessionAimControlRepository;
        private readonly IRepository<ExerciseAimControl, ExerciseAimControlId> _exerciseAimControlRepository;

        private readonly IAccessorAsync<CurrentUser> _currentUserAccessor;

        public CreateUserCommandHandler(
            IRepository<User, UserId> userRepository,
            IRepository<PlanAimControl, PlanAimControlId> planAimControlRepository,
            IRepository<SessionAimControl, SessionAimControlId> sessionAimControlRepository,
            IRepository<ExerciseAimControl, ExerciseAimControlId> exerciseAimControlRepository,
            IAccessorAsync<CurrentUser> currentUserAccessor)
        {
            _userRepository = userRepository;
            _currentUserAccessor = currentUserAccessor;
            _planAimControlRepository = planAimControlRepository;
            _sessionAimControlRepository = sessionAimControlRepository;
            _exerciseAimControlRepository = exerciseAimControlRepository;
        }

        public async ValueTask<Result> Handle(CreateUserCommand command, CancellationToken ct)
        {
            var result = Result.Success();

            var currentUser = await _currentUserAccessor.Get();
            if (currentUser is null)
                return result.Fail();

            var userId = new UserId(currentUser.Id);
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
}
