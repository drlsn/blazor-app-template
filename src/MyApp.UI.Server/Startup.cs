using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.Basic.UseCases;
using Corelibs.MongoDB;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mediator;
using MyApp.UI.Server.Data;
using System.Reflection;

namespace MyApp.UI.Server;

public static class Startup
{
    public static void InitializeApp(this IServiceCollection services, IWebHostEnvironment environment)
    {
        var entitiesAssembly = typeof(Entities.Users.User).Assembly;
        var useCasesAssembly = typeof(UseCases.Users.CreateUserCommand).Assembly;

        services.AddScoped<IAccessorAsync<CurrentUser>, CurrentUserAccessor>();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(useCasesAssembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MongoDbCommandTransactionBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MongoDbQueryBehaviour<,>));

        services.AddScoped<IQueryExecutor, MediatorQueryExecutor>();
        services.AddScoped<ICommandExecutor, MediatorCommandExecutor>();

        services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);

        services.AddRepositories(environment, entitiesAssembly);
    }

    public static void AddRepositories(this IServiceCollection services, IWebHostEnvironment environment, Assembly assembly)
    {
        var mongoConnectionString = Environment.GetEnvironmentVariable("MyAppDatabaseConn");
        var databaseName = environment.IsDevelopment() ? "MyApp_dev" : "MyApp_prod";

        MongoConventionPackExtensions.AddIgnoreConventionPack();

        services.AddMongoRepositories(assembly, mongoConnectionString, databaseName);
    }
}
