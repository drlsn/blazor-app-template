using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.MongoDB;
using Mediator;
using MyApp.UI.Common.Data;
using MyApp.UI.Server.Data;

namespace MyApp.UI.Server;

public static class Startup
{
    public static void InitializeApp(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddScoped<IAccessorAsync<CurrentUser>, CurrentUserAccessor>();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MongoDbCommandTransactionBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MongoDbQueryBehaviour<,>));

        services.AddScoped<IQueryExecutor, MediatorQueryExecutor>();
        services.AddScoped<ICommandExecutor, MediatorCommandExecutor>();

        services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);

        services.AddRepositories(environment);
    }

    public static void AddRepositories(this IServiceCollection services, IWebHostEnvironment environment)
    {
        var mongoConnectionString = Environment.GetEnvironmentVariable("MyAppDatabaseConn");
        var databaseName = environment.IsDevelopment() ? "MyApp_dev" : "MyApp_prod";

        MongoConventionPackExtensions.AddIgnoreConventionPack();

        services.AddMongoRepositories(typeof(MyApp.Entities.Users.User).Assembly, mongoConnectionString, databaseName);
    }
}
