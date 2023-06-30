using Corelibs.Basic.DDD;

namespace MyApp.Entities.Sessions;

public record SessionId(string Value) : EntityId(Value);

public class Session : NamedEntity<SessionId>, IAggregateRoot<SessionId>
{
    public const string DefaultCollectionName = "sessions";

    public Session(string name) : base(name)
    {
    }
}
