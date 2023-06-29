using Corelibs.Basic.Blocks;
using Mediator;

namespace MyApp.UseCases.Plans;

public record GetOwnPlansQuery : IQuery<Result<GetOwnPlansQueryResponse>>;

public record GetOwnPlansQueryResponse(PlanVM[] Plans);

public record PlanVM(string Id, string Name);
