using Corelibs.Basic.DDD;
using MyApp.Entities.ExercisesAimsControls;
using MyApp.Entities.PlanAimControls;
using MyApp.Entities.SessionAimControls;

namespace MyApp.Entities.Users;

public record UserId(string Value) : EntityId(Value);

public class User : Entity<UserId>, IAggregateRoot<UserId>
{
    public const string DefaultCollectionName = "users";

    public User(
        UserId userId,
        PlanAimControlId planAimControlId,
        SessionAimControlId sessionAimControlId, 
        ExerciseAimControlId exerciseAimControlId) : base(userId)
    {
        PlanAimControlId = planAimControlId;
        SessionAimControlId = sessionAimControlId;
        ExerciseAimControlId = exerciseAimControlId;
    }

    public PlanAimControlId PlanAimControlId { get; init; }
    public SessionAimControlId SessionAimControlId { get; init; }
    public ExerciseAimControlId ExerciseAimControlId { get; init; }
}
