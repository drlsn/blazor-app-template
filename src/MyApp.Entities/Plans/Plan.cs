using Corelibs.Basic.DDD;
using MyApp.Entities.Users;

namespace MyApp.Entities.Plans;

public record PlanId(string Value) : EntityId(Value);

public class Plan : NamedEntity<PlanId>, IAggregateRoot<PlanId>
{
    public const string DefaultCollectionName = "plans";

    public Plan(
        string name,
        UserId ownerId) : base(name)
    {
        OwnerId = ownerId;
    }

    public UserId OwnerId { get; init; }
}
