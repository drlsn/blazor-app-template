using Corelibs.Basic.DDD;

namespace MyApp.Entities.Exercises;

public record ExerciseId(string Value) : EntityId(Value);

public class Exercise : Entity<ExerciseId>, IAggregateRoot<ExerciseId>
{
    public const string DefaultCollectionName = "exercises";
}
