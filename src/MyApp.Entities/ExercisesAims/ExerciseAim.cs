using Corelibs.Basic.DDD;
using MyApp.Entities.Exercises;
using MyApp.Entities.Shared;

namespace MyApp.Entities.ExercisesAims;

public record ExerciseAimId(string Value) : EntityId(Value);

public class ExerciseAim : Aim<ExerciseId, ExerciseAimId>, IAggregateRoot<ExerciseAimId>
{
    public const string DefaultCollectionName = "exerciseAims";

    public ExerciseAim(
        ExerciseId exerciseId,
        DateTime startedTime) : base(exerciseId, startedTime) {}
}
