using Corelibs.Basic.DDD;
using MyApp.Entities.Shared;

namespace MyApp.Entities.Exercises;

public record ExerciseId(string Value) : EntityId(Value);

public class Exercise : Entity<ExerciseId>
{

}
