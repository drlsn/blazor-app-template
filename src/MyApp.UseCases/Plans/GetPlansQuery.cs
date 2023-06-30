using Corelibs.Basic.Blocks;
using Mediator;

namespace MyApp.UseCases.Plans;

public record GetOwnPlansQuery : IQuery<Result<GetOwnPlansQueryResponse>>;

public record GetOwnPlansQueryResponse(PlanDTO[] Plans);

public record PlanDTO(string Id, string Name);
