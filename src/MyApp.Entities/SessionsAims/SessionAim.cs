using Corelibs.Basic.DDD;
using MyApp.Entities.Sessions;
using MyApp.Entities.Shared;

namespace MyApp.Entities.SessionsAims;

public record SessionAimId(string Value) : EntityId(Value);

public class SessionAim : Aim<SessionId, SessionAimId>
{
    public SessionAim(
        SessionId sessionId,
        DateTime startedTime) : base(sessionId, startedTime) { }
}
