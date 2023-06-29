using FluentValidation;
using Mediator;

namespace MyApp.Infrastructure.Validation;

public class ValidationBehaviour<TCommand, TResult> : IPipelineBehavior<TCommand, TResult>
    where TCommand : IBaseCommand
{
    private readonly IValidator<TCommand> _validator;

    public ValidationBehaviour(IValidator<TCommand> validator)
    {
        _validator = validator;
    }

    public async ValueTask<TResult> Handle(TCommand command, CancellationToken cancellationToken, MessageHandlerDelegate<TCommand, TResult> next)
    {
        var result = await _validator.ValidateAsync(command);
        if (!result.IsValid)
            return default(TResult);

        return await next(command, cancellationToken);
    }
}
