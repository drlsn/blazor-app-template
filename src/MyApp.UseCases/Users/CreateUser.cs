using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Mediator;
using MyApp.Entities.Users;

namespace MyApp.UseCases.Users
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result>
    {
        private IRepository<User, UserId> _userRepository;
        private IAccessor<CurrentUser> _currentUserAccessor;

        public CreateUserCommandHandler(
            IRepository<User, UserId> userRepository,
            IAccessor<CurrentUser> currentUserAccessor)
        {
            _userRepository = userRepository;
            _currentUserAccessor = currentUserAccessor;
        }

        public async ValueTask<Result> Handle(CreateUserCommand command, CancellationToken ct)
        {
            var result = Result.Success();

            var currentUser = _currentUserAccessor.Get();

            var userId = new UserId(currentUser.ID);
            var user = await _userRepository.Get(userId, result);
            if (!result.IsSuccess || user != null)
                return result;

            user = new User(userId);

            await _userRepository.Save(user, result);

            return result;
        }
    }

    public record CreateUserCommand() : ICommand<Result>;
}
