using Corelibs.Basic.DDD;
using MyApp.Entities.ExercisesAimsControls;
using MyApp.Entities.PlanAimControls;
using MyApp.Entities.SessionAimControls;

namespace MyApp.Entities.Users;

public record UserId(string Value) : EntityId(Value);

public class User : Entity<UserId>
{
    public User(
        UserId id, 
        PlanAimControlId planAimControlId, 
        SessionAimControlId sessionAimControlId, 
        ExerciseAimControlId exerciseAimControlId) : base(id)
    {
        PlanAimControlId = planAimControlId;
        SessionAimControlId = sessionAimControlId;
        ExerciseAimControlId = exerciseAimControlId;
    }

    public PlanAimControlId PlanAimControlId { get; private set; }
    public SessionAimControlId SessionAimControlId { get; private set; }
    public ExerciseAimControlId ExerciseAimControlId { get; private set; }

}
