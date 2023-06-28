using Corelibs.Basic.DDD;
using MyApp.Entities.Users;

namespace MyApp.Entities.Plans;

public record PlanId(string Value) : EntityId(Value);

public class Plan : Entity<PlanId>, IAggregateRoot<PlanId>
{
    public const string DefaultCollectionName = "plans";

    public UserId OwnerId { get; init; }
    public string Name { get; set; }
}
