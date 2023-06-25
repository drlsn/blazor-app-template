using Corelibs.Basic.DDD;
using MyApp.Entities.Exercises;
using MyApp.Entities.ExercisesAims;
using MyApp.Entities.Shared;

namespace MyApp.Entities.ExercisesAimsControls;

public record ExerciseAimControlId(string Value) : EntityId(Value);

public class ExerciseAimControl : AimControl<Exercise, ExerciseId, ExerciseAim, ExerciseAimId, ExerciseAimControlId>
{
    protected override ExerciseAim CreateAim(Exercise exercise, DateTime startedTime) =>
        new ExerciseAim(exercise.Id, startedTime);
}
