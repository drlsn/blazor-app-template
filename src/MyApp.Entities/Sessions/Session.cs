using Corelibs.Basic.DDD;
using MyApp.Entities.SessionAimControls;

namespace MyApp.Entities.Sessions;

public record SessionId(string Value) : EntityId(Value);

public class Session : Entity<SessionId>, IAggregateRoot<SessionId>
{
    public const string DefaultCollectionName = "sessions";
}
