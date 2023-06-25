using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;

namespace MyApp.Entities.Shared;

public abstract class AimControl<TAimSource, TAimSourceId, TAim, TAimId, TAimControlId> : Entity<TAimControlId>
    where TAimId : EntityId
    where TAimControlId : EntityId
    where TAim : Aim<TAimSourceId, TAimId>
{
    public TAimId AimId { get; private set; }

    public bool IsAimActive => AimId != null;

    public Result<TAim> StartExercise(TAimSource aimSource, DateTime startedTime)
    {
        var result = new Result<TAim>();

        if (IsAimActive)
            return result.Fail("Can't start an aim if already doing one.");

        var aim = CreateAim(aimSource, startedTime);
        AimId = aim.Id;

        return result.With(aim);
    }

    public Result FinishExercise(TAim aim, DateTime finishTime)
    {
        var result = new Result<TAim>();

        if (!IsAimActive)
            return result.Fail("Can't finish an aim if not doing one.");

        if (aim.Id != AimId)
            return result.Fail("Given aim is not correct.");

        var finishResult = aim.Finish(finishTime);
        if (!finishResult.IsSuccess)
            return result.With(finishResult);

        AimId = null;

        return Result.Success();
    }

    protected abstract TAim CreateAim(TAimSource aimSource, DateTime startedTime);
}
