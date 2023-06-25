using Corelibs.Basic.DDD;
using Corelibs.Basic.Repository;
using Corelibs.MongoDB;
using Mediator;
using MongoDB.Driver;
using MyApp.Entities.ExercisesAimsControls;
using MyApp.Entities.PlanAimControls;
using MyApp.Entities.SessionAimControls;
using MyApp.Entities.Users;
using MyApp.UI.Common.Data;
using MyApp.UI.Server.Data;

namespace MyApp.UI.Server;

public static class Startup
{
    public static void InitializeApp(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddScoped<IAccessorAsync<CurrentUser>, CurrentUserAccessor>();
        services.AddMediator(opts => opts.ServiceLifetime = ServiceLifetime.Scoped);
        services.AddSingleton<ICommandExecutor, CommandExecutor>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MongoDbTransactionBehaviour<,>));
        services.AddRepositories(environment);
    }

    public static void AddRepositories(this IServiceCollection services, IWebHostEnvironment environment)
    {
        var cs = Environment.GetEnvironmentVariable("MyAppDatabaseConn");
        var databaseName = environment.IsDevelopment() ? "MyApp_dev" : "MyApp_prod";
        
        services.AddMongoRepository<User, UserId>(cs, databaseName, "users");
        services.AddMongoRepository<PlanAimControl, PlanAimControlId>(cs, databaseName, "planAims");
        services.AddMongoRepository<SessionAimControl, SessionAimControlId>(cs, databaseName, "sessionAims");
        services.AddMongoRepository<ExerciseAimControl, ExerciseAimControlId>(cs, databaseName, "exerciseAims");
    }

    public static void AddMongoRepository<TEntity, TEntityId>(
        this IServiceCollection services,
        string connectionString, string databaseName, string collectionName)
        where TEntity : IEntity<TEntityId>
    {
        var client = new MongoClient(connectionString);
        services.AddSingleton<MongoClient>();

        services.AddScoped<IClientSession>(sp =>
        {
            var client = sp.GetRequiredService<MongoClient>();
            var session = client.StartSession();

            return session;
        });

        services.AddScoped<IRepository<TEntity, TEntityId>>(sp =>
        {
            var client = sp.GetRequiredService<MongoClient>();
            var session = sp.GetRequiredService<IClientSessionHandle>();

            return new MongoDbRepository<TEntity, TEntityId>(client, session, databaseName, collectionName);
        });
    }
}
