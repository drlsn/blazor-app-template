using Corelibs.Basic.DDD;
using MyApp.Entities.Sessions;
using MyApp.Entities.SessionsAims;
using MyApp.Entities.Shared;

namespace MyApp.Entities.SessionAimControls;

public record SessionAimControlId(string Value) : EntityId(Value);

public class SessionAimControl : AimControl<Session, SessionId, SessionAim, SessionAimId, SessionAimControlId>
{
    protected override SessionAim CreateAim(Session session, DateTime startedTime) =>
        new SessionAim(session.Id, startedTime);
}
