using Corelibs.Basic.Blocks;
using Corelibs.Basic.UseCases.DTOs;
using Mediator;

namespace MyApp.UseCases.Plans;

public record GetPlanDetailsQuery(string PlanId) : IQuery<Result<GetPlanDetailsQueryResponse>>;

public record GetPlanDetailsQueryResponse(PlanDetailsDTO PlanDetails);

public record PlanDetailsDTO(
    IdentityDTO Identity,
    IdentityDTO Author,
    SessionDTO[] Sessions);

public record SessionDTO(
    string Id,
    string Type,
    string Name);
