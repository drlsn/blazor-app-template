using Corelibs.Basic.Blocks;
using Mediator;

namespace MyApp.UseCases.Plans;

public record GetPlanDetailsQuery(string PlanId) : IQuery<Result<GetPlanDetailsQueryResponse>>;

public record GetPlanDetailsQueryResponse(PlanDetailsDTO PlanDetails);

public record PlanDetailsDTO(
    string Id,
    string Name,
    string Author,
    SessionsInfoDTO SessionsInfo);

public record SessionsInfoDTO(
    int ActivePlusRestOverall,
    int ActiveOnlyUnique,
    int ActiveOnlyOverall,
    int RestOnly,
    SessionDTO[] Sessions);

public record SessionDTO(
    string Id,
    string Type,
    string Name);
