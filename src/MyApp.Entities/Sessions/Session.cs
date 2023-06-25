using Corelibs.Basic.DDD;

namespace MyApp.Entities.Sessions;

public record SessionId(string Value) : EntityId(Value);

public class Session : Entity<SessionId>
{

}
