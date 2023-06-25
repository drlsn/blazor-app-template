using Corelibs.Basic.DDD;
using MyApp.Entities.Shared;

namespace MyApp.Entities.Plans;

public record PlanId(string Value) : EntityId(Value);

public class Plan : Entity<PlanId>
{

}
