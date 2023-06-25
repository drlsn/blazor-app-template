using Corelibs.Basic.Blocks;
using Corelibs.Basic.DDD;

namespace MyApp.Entities.Shared;

public abstract class Aim<TAimSourceId, TAimId> : Entity<TAimId>
    where TAimId : EntityId
{
    public TAimSourceId AimSourceId { get; private set; }
    public DateTime StartedTime { get; private set; }
    public DateTime FinishTime { get; private set; }

    public Aim(
        TAimSourceId aimSourceId,
        DateTime startedTime)
    {
        AimSourceId = aimSourceId;
        StartedTime = startedTime;
    }

    public Result Finish(DateTime finishTime)
    {
        var result = Result.Success();

        if (FinishTime == default)
            return result.Fail("Can't finish an aim if already finished.");

        if (finishTime <= StartedTime)
            return result.Fail("Can't finish an aim if finish time is earlier than start.");

        FinishTime = finishTime;

        return result;
    }
}
