using Corelibs.Basic.DDD;
using MyApp.Entities.ExercisesAimsControls;

namespace MyApp.Entities.Users;

public record UserId(string Value) : EntityId(Value);

public class User : Entity<UserId>
{
    public User(UserId id) : base(id) { }

    public ExerciseAimControlId ExerciseAimControlId { get; private set; }

}
