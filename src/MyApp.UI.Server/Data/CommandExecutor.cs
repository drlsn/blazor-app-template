using Corelibs.Basic.Blocks;
using Mediator;
using MyApp.UI.Common.Data;

namespace MyApp.UI.Server.Data
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IMediator _mediator;

        public CommandExecutor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand<Result>
        {
            var result = await _mediator.Send(command, cancellationToken);

            return result;
        }
    }
}
