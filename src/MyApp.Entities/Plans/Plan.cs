using Corelibs.Basic.Collections;
using Corelibs.Basic.DDD;
using Corelibs.Basic.Maths;
using MyApp.Entities.Sessions;
using MyApp.Entities.Shared;
using MyApp.Entities.Users;

namespace MyApp.Entities.Plans;

public record PlanId(string Value) : EntityId(Value);

public class Plan : NamedEntity<PlanId>, IAggregateRoot<PlanId>
{
    public const string DefaultCollectionName = "plans";

    public UserId OwnerId { get; init; }
    public List<Activity> Activities { get; private set; }

    public Plan(
        string name,
        UserId ownerId) : base(name)
    {
        OwnerId = ownerId;
    }

    public void AddSession(SessionId sessionId, int index = 0)
    {
        Activities ??= new();
        Activities.InsertClamped(new Activity(sessionId), index);
    }

    public void AddRest(int index)
    {
        Activities ??= new();
        Activities.InsertClamped(Activity.Rest, index);
    }

    public void RemoveActivity(SessionId sessionId, int index = 0)
    {
        Activities ??= new();
        Activities.InsertClamped(new Activity(sessionId), index);
    }
}

public class Activity
{
    public static readonly Activity Rest = new Activity();

    public Activity(SessionId sessionId = null) => SessionId = sessionId;

    public SessionId SessionId { get; init; }
    public ActivityType Type => SessionId is null ?
        new ActivityType.Rest() :
        new ActivityType.Session();
}
