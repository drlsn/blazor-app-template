using Corelibs.Basic.DDD;
using MyApp.Entities.ExercisesAims;
using MyApp.Entities.Plans;
using MyApp.Entities.Shared;

namespace MyApp.Entities.PlanAimControls;

public record PlanAimControlId(string Value) : EntityId(Value);

public class PlanAimControl : AimControl<Plan, PlanId, PlanAim, PlanAimId, PlanAimControlId>, IAggregateRoot<PlanAimControlId>
{
    public const string DefaultCollectionName = "planAimControls";

    protected override PlanAim CreateAim(Plan plan, DateTime startedTime) =>
        new PlanAim(plan.Id, startedTime);
}
