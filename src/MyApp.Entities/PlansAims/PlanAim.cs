using Corelibs.Basic.DDD;
using MyApp.Entities.Shared;

namespace MyApp.Entities.Plans;

public record PlanAimId(string Value) : EntityId(Value);

public class PlanAim : Aim<PlanId, PlanAimId>
{
    public PlanAim(
        PlanId planId,
        DateTime startedTime) : base(planId, startedTime) {}
}
