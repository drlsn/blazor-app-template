using Corelibs.Basic.DDD;
using MyApp.Entities.Shared;
using MyApp.Entities.Users;

namespace MyApp.Entities.Plans;

public record PlanId(string Value) : EntityId(Value);

public class Plan : Entity<PlanId>
{
    public UserId UserId { get; init; }
    public string Name { get; set; }
}
